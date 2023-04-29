using Assets.Scripts.PeroTools.Commons;
using FormulaBase;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace AccuracyDisplay

{
    internal class Indicator : MonoBehaviour
    {
        internal static bool accuracyUpdate = false;
        internal static Font font { get; set; }
        internal static bool accuracy_update { get; set; }
        private static GameObject Accuracy { get; set; }

        private static Color blue = new Color(0 / 255f, 136 / 255f, 255 / 255f, 255 / 255f);

        private static Color silver = new Color(192 / 255f, 192 / 255f, 192 / 255f, 255 / 255f);

        private static Color purple = new Color(192 / 255f, 0 / 255f, 192 / 255f, 255 / 255f);
        public Indicator(IntPtr intPtr) : base(intPtr)
        {

        }

        private void Start()
        {
            var asset = Addressables.LoadAssetAsync<Font>("Snaps Taste");
            font = asset.WaitForCompletion();
        }

        private void SetAccuracyValues()
        {
            Text textcomponent = Accuracy.GetComponent<Text>();
            double accuracy = ModMain.CalculatedAcc;
            accuracy = accuracy > 0.99 ? (Math.Truncate(accuracy * 10000) / 100) : (Math.Round(accuracy * 100, 2));
            accuracy = accuracy > 100 ? 100 : accuracy;
            textcomponent.text = $"{accuracy}%";

            switch (accuracy)
            {
                case var n when n == 100:
                    textcomponent.color = Color.yellow;
                    break;
                case var n when n >= 95:
                    textcomponent.color = silver;
                    break;
                case var n when n >= 90:
                    textcomponent.color = Color.magenta;
                    break;
                case var n when n >= 80:
                    textcomponent.color = purple;
                    break;
                case var n when n >= 70:
                    textcomponent.color = Color.cyan;
                    break;
                case var n when n >= 60:
                    textcomponent.color = Color.green;
                    break;
                default:
                    textcomponent.color = Color.gray;
                    break;
            }
        }

        private void Update()
        {


            if (GameObject.Find("PnlVictory_2D") != null)
            {
                if (!ModMain.EndDetectXD)
                {

                    ModMain.EndDetectXD = true;
                    ModMain.IsDirty = true;
                }
                if (Accuracy != null)
                {
                    Destroy(Accuracy);
                }
            }
            
            if (Accuracy != null)
            {
                if (ModMain.IsDirty)
                {
                    SetAccuracyValues();
                    ModMain.IsDirty = false;
                }
            }
            else if (Singleton<StageBattleComponent>.instance.isInGame)
            {

                SetCanvas();
                Accuracy = SetAccuracyObject("Accuracy", Color.yellow, "100.00%");
            }
            
        }


        private static GameObject SetAccuracyObject(string name, Color color, string text)
        {
            GameObject canvas = GameObject.Find("AccuracyDisplay Canvas");
            GameObject gameobject = new GameObject(name);
            gameobject.transform.SetParent(canvas.transform);
            Text gameobject_text = gameobject.AddComponent<Text>();
            gameobject_text.text = text;
            gameobject_text.alignment = TextAnchor.UpperLeft;
            gameobject_text.font = font;
            gameobject_text.color = color;
            gameobject_text.fontSize = 90;
            gameobject_text.transform.localPosition = new Vector3(-402.398f, 290.82912f, 0f);
            RectTransform rectTransform = gameobject_text.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(960, 216);
            rectTransform.localScale = new Vector3(1, 1, 1);
            return gameobject;
        }

        public static void SetCanvas()
        {
            GameObject canvas = new GameObject();
            canvas.name = "AccuracyDisplay Canvas";
            canvas.AddComponent<Canvas>();
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            canvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Camera_2D").GetComponent<Camera>();
            canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        }
    }
}