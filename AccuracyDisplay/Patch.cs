using Assets.Scripts.GameCore.HostComponent;
using Assets.Scripts.PeroTools.Commons;
using FormulaBase;
using GameLogic;
using System.Collections.Generic;
using HarmonyLib;

namespace AccuracyDisplay
{
    [HarmonyPatch(typeof(StageBattleComponent), "GameStart")]
    internal static class StageBattleComponentPatch
    {
        private static void Prefix(StageBattleComponent __instance)
        {
            ModMain.MissOffset = 0;
            SetPlayResultPatch.resolveDoubles = new HashSet<int>();
            SetPlayResultPatch.resolveFakeDoubles = new HashSet<int>();
            SetPlayResultPatch.resolveMashers = new HashSet<int>();
            SetPlayResultPatch.noteDoubleFix = new HashSet<int>();
            ModMain.realHoldCount = __instance.m_MusicTickData.Count(x => x.noteData.type == 3 && (x.isLongPressStart || x.isLongPressEnd));

            if (ModMain._task == null)
            {
                ModMain._task = Singleton<TaskStageTarget>.instance;
            }
        }
    }

    [HarmonyPatch(typeof(GameMissPlay), "MissCube")]
    internal class MissCubePatch
    {
        internal static void Postfix(int idx, decimal currentTick)
        {
            var result = Singleton<BattleEnemyManager>.instance.GetPlayResult(idx);
            var currentMusicData = Singleton<StageBattleComponent>.instance.GetMusicDataByIdx(idx);
            var type = currentMusicData.noteData.type;
            bool isNull = result == 0;
            if (type == 2 && isNull)
            {
                //handled here cuz the game froze when going through gears during I-frames lol
                return;
            }
            var doubleIdx = currentMusicData.doubleIdx;

            ModMain.IsDirty = true;
            if (result != 1 && !isNull)
            {
                return;
            }
            if (SetPlayResultPatch.noteDoubleFix.Contains(idx))
            {
                return;
            }
            switch (type)
            {
                case 1:
                    if (doubleIdx == -1)
                    {
                        if (isNull)
                        {
                            ModMain.MissOffset++;
                            SetPlayResultPatch.noteDoubleFix.Add(idx);
                        }
                    }
                    else
                    {
                        if (currentMusicData.isDouble)
                        {
                            if (!SetPlayResultPatch.resolveDoubles.Contains(idx) || !SetPlayResultPatch.resolveDoubles.Contains(doubleIdx))
                            {
                                ModMain.MissOffset += 2;
                                SetPlayResultPatch.resolveDoubles.Add(idx);
                                SetPlayResultPatch.resolveDoubles.Add(doubleIdx);
                                SetPlayResultPatch.noteDoubleFix.Add(idx);
                                SetPlayResultPatch.noteDoubleFix.Add(doubleIdx);
                            }
                        }
                        else
                        {
                            if (SetPlayResultPatch.resolveDoubles.Add(idx))
                            {
                                ModMain.MissOffset++;
                                SetPlayResultPatch.noteDoubleFix.Add(idx);
                            }
                        }
                    }
                    break;
                case 2:
                    ModMain.MissOffset++;
                    break;
                case 4:
                    if (doubleIdx == -1)
                    {
                        if (isNull)
                        {
                            ModMain.MissOffset++;
                            SetPlayResultPatch.noteDoubleFix.Add(idx);
                        }
                    }
                    else if (SetPlayResultPatch.resolveFakeDoubles.Add(idx))
                    {
                        ModMain.MissOffset++;
                        SetPlayResultPatch.noteDoubleFix.Add(idx);
                    }
                    break;

                case 6:
                case 7:
                    if (SetPlayResultPatch.resolveFakeDoubles.Add(idx))
                    {
                        ModMain.MissOffset++;
                    }
                    break;
                case 5:
                    if (isNull && SetPlayResultPatch.resolveFakeDoubles.Add(idx))
                    {
                        ModMain.MissOffset++;
                    }
                    break;
                case 8:
                    if (isNull && !SetPlayResultPatch.resolveMashers.Contains(idx))
                    {
                        ModMain.MissOffset++;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(BattleEnemyManager), "SetPlayResult")]
    internal class SetPlayResultPatch
    {
        internal static HashSet<int> resolveDoubles;

        internal static HashSet<int> resolveFakeDoubles;

        internal static HashSet<int> noteDoubleFix;

        internal static HashSet<int> resolveMashers;

        internal static byte longPressing;
        internal static void Postfix(int idx, byte result, bool isMulStart = false, bool isMulEnd = false, bool isLeft = false)
        {
            var currentMusicData = Singleton<StageBattleComponent>.instance.GetMusicDataByIdx(idx);

            var doubleIdx = currentMusicData.doubleIdx;

            var type = currentMusicData.noteData.type;

            ModMain.IsDirty = true;
            switch (type)
            {
                case 1:
                    if (!currentMusicData.isDouble)
                    {
                        resolveDoubles.Add(idx);
                        break;
                    }
                    if (doubleIdx == -1 || result != 1)
                    {
                        resolveDoubles.Add(idx);
                        break;
                    }
                    if (resolveDoubles.Contains(doubleIdx) && resolveDoubles.Contains(idx))
                    {
                        ModMain.MissOffset -= 2;
                        break;
                    }
                    var doubleData = Singleton<StageBattleComponent>.instance.GetMusicDataByIdx(doubleIdx);
                    var doubleResult = Singleton<BattleEnemyManager>.instance.GetPlayResult(doubleIdx);
                    if (doubleData.isAir && doubleResult != 0)
                    {
                        ModMain.MissOffset+=2;
                        resolveDoubles.Add(idx);
                    }
                    resolveDoubles.Add(doubleIdx);
                    break;
                case 3:
                    if (currentMusicData.isLongPressStart)
                    {
                        if (result == 1)
                        {
                            ModMain.MissOffset++;
                            break;
                        }
                        else
                        {
                            longPressing++;
                        }
                    }
                    else if (result == 1)
                    {
                        longPressing--;
                        ModMain.MissOffset++;
                        
                    }
                    else if (currentMusicData.isLongPressEnd)
                    {
                        longPressing--;
                    }
                    break;
                case 4:
                    resolveFakeDoubles.Add(idx);
                    break;
                case 6:
                case 7:
                    noteDoubleFix.Add(idx);
                    break;
                case 8:
                    if (!resolveMashers.Contains(idx) && result == 1)
                    {
                        ModMain.MissOffset--;
                    }
                    resolveMashers.Add(idx);
                    break;
                default:
                    break;
            }

            if (type == 3 && !currentMusicData.isLongPressStart && !currentMusicData.isLongPressEnd && result != 1 && result != 0)
            {
                ModMain.IsDirty = false;
                return;
            }
            
        }
    }
}
