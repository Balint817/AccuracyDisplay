using Assets.Scripts.GameCore.HostComponent;
using MelonLoader;
using System;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace AccuracyDisplay
{

    public static class Extensions
    {
        public static int Count(this Il2CppSystem.Collections.IEnumerable collection, Func<object, bool> function)
        {
            int result = 0;
            foreach (var item in collection)
            {
                if (function.Invoke(item))
                {
                    result++;
                }
            }
            return result;
        }
        public static int Count<T>(this Il2CppSystem.Collections.Generic.List<T> collection, Func<T, bool> function)
        {
            int result = 0;
            foreach (var item in collection)
            {
                if (function.Invoke(item))
                {
                    result++;
                }
            }
            return result;
        }
    }
    public class ModMain : MelonMod {

        public static double RealAcc;

        public static bool EndDetectXD;

        public static bool IsDirty;

        private static int _mOff;

        public static int MissOffset
        {
            get
            {
                return _mOff;
            }
            set
            {
                _mOff = value;
                IsDirty = true;
            }
        }

        public static int realHoldCount;

        public static TaskStageTarget _task;

        public static int Num100
        {
            get
            {
                return
                    _task.GetHitCountByResult(2) +
                    _task.GetHitCountByResult(4) +
                    _task.GetHitCountByResult(5) +
                    _task.m_Block +
                    _task.m_MusicCount +
                    _task.m_EnergyCount;
            }
        }

        public static int Num50
        {
            get
            {
                return
                    _task.GetHitCountByResult(3);
            }
        }

        public static int Num0
        {
            get
            {
                return
                    _task.GetHitCountByResult(1) +
                    MissOffset;
            }
        }

        public static double CalculatedAcc
        {
            get
            {
                var n100 = Num100;
                var n50 = Num50;
                var n0 = Num0;
                return (n100 + n50 / 2d) / (n100 + n50 + n0);
            }
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "GameMain")
            {
                ClassInjector.RegisterTypeInIl2Cpp<Indicator>();
                GameObject gameObject = new GameObject("Indicator");
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.AddComponent<Indicator>();
            }
            else
            {
                ResetTrackers();
            }
        }
        public void ResetTrackers()
        {
            RealAcc = 0;
            MissOffset = 0;
            EndDetectXD = false;
            IsDirty = false;
            ModMain._task = null;
            SetPlayResultPatch.resolveDoubles = null;
            SetPlayResultPatch.resolveFakeDoubles = null;
            SetPlayResultPatch.resolveMashers = null;
            SetPlayResultPatch.noteDoubleFix = null;
            SetPlayResultPatch.longPressing = 0;
            realHoldCount = -1;
        }
    }
}