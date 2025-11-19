using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public class MainPanel : VisualElement
    {
        public event Action OnButtonNewLevel;

        public event Action OnButtonOpenLevel;

        public event Action OnButtonSaveLevel;

        public event Action OnButtonPlay;

        public event Action<int,int> OnGridSizeChanged; 

        private IntegerField intFieldWidth;

        private IntegerField intFieldHeight;

        private IntegerField intFieldGridSize;

        private Button buttonPlayLevel;
        public MainPanel()
        {
            var visualTreeAsset = Resources.Load<VisualTreeAsset>("UI/MainPanel");

            var child = visualTreeAsset.CloneTree();
            Add(child);

            #region Main Buttons

            var buttonNewLevel = child.Q<Button>("buttonNewLevel");
            buttonNewLevel.clicked += ButtonNewLevelOnclicked;

            var buttonOpenLevel = child.Q<Button>("buttonOpenLevel");
            buttonOpenLevel.clicked += ButtonOpenLevelOnclicked;

            var buttonSaveLevel = child.Q<Button>("buttonSaveLevel");
            buttonSaveLevel.clicked += ButtonSaveLevelOnclicked;

            buttonPlayLevel = child.Q<Button>("buttonPlay");
            buttonPlayLevel.clicked += ButtonPlayLevel_clicked;

            #endregion

            #region Level Size

            intFieldWidth = child.Q<IntegerField>("intFieldWidth");
            intFieldWidth.SetValueWithoutNotify(EditorConfig.LevelWidth);
            intFieldWidth.RegisterValueChangedCallback((evt =>
            {
                //EditorConfig.LevelWidth = evt.newValue;
            }));

            intFieldHeight = child.Q<IntegerField>("intFieldHeight");
            intFieldHeight.SetValueWithoutNotify(EditorConfig.LevelHeight);
            intFieldHeight.RegisterValueChangedCallback(evt =>
            {
                //EditorConfig.LevelHeight = evt.newValue;
            });

            //Button buttonLevelSizeChange = child.Q<Button>("buttonLevelSizeChange");
            //buttonLevelSizeChange.clicked += ButtonLevelSizeChange_clicked;
            #endregion

            #region Grid Size

            intFieldGridSize = child.Q<IntegerField>("intFieldGridSize");
            intFieldGridSize.SetValueWithoutNotify(EditorConfig.GridSize);
            //intFieldGridSize.RegisterValueChangedCallback(evt =>
            //{
            //    //EditorConfig.GridSize = evt.newValue;

            //    //EditorConfig.ArrowLineWidth = EditorConfig.GridSize * 5 / 25f;
            //    //OnGridSizeChanged?.Invoke(evt.newValue,evt.previousValue);
            //});

            Button buttonChangeGridSize = child.Q<Button>("buttonChangeGridSize");
            buttonChangeGridSize.clicked += ButtonGridSizeChangeOnclicked;


            Button buttonOpenImage = child.Q<Button>("buttonOpenImage");
            buttonOpenImage.clicked += ButtonOpenImage_clicked;

            Slider sliderImagePreviewOpacity = child.Q<Slider>("sliderImagePreviewOpacity");
            sliderImagePreviewOpacity.RegisterValueChangedCallback((evt =>
            {
                EditorEventManager.OnPreviewImageOpacityChangedEvent(evt.newValue/255f);
            }));
            sliderImagePreviewOpacity.value = sliderImagePreviewOpacity.highValue;

            #endregion

            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        ~MainPanel()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.EnteredEditMode)
            {
                buttonPlayLevel.text = "Play";
            }
            else if (playModeState == PlayModeStateChange.EnteredPlayMode)
            {
                buttonPlayLevel.text = "Stop";
            }
        }

        private void ButtonOpenImage_clicked()
        {
            string filePath = EditorUtility.OpenFilePanel("Open Image", string.Empty, "png,jpg");
            if (!string.IsNullOrEmpty(filePath))
            {
                EditorEventManager.OnPreviewImageOpenedEvent(filePath);
            }
        }

        public static Texture2D LoadTexture(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("not found: " + filePath);
                return null;
            }

            byte[] fileData = File.ReadAllBytes(filePath);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            return tex;
        }

        //private void ButtonLevelSizeChange_clicked()
        //{


        //    EditorEventManager.OnLevelSizeChangedEvent(intFieldWidth.value, intFieldHeight.value);
        //}

        private void ButtonGridSizeChangeOnclicked()
        {
            int newValue = intFieldGridSize.value;
            int oldValue = EditorConfig.GridSize;
            EditorConfig.GridSize = newValue;

            EditorEventManager.OnGridSizeChangedEvent(newValue,oldValue);
        }

        private void ButtonPlayLevel_clicked()
        {
            OnButtonPlay?.Invoke();
        }

        private void ButtonSaveLevelOnclicked()
        {
            OnButtonSaveLevel?.Invoke();
        }

        private void ButtonOpenLevelOnclicked()
        {
            OnButtonOpenLevel?.Invoke();
        }

        private void ButtonNewLevelOnclicked()
        {
            EditorConfig.LevelWidth = intFieldWidth.value;
            EditorConfig.LevelHeight = intFieldHeight.value;
            OnButtonNewLevel?.Invoke();
        }

        public void SetLevelData(LevelData levelData)
        {
            intFieldWidth.value = levelData.Width;
            intFieldHeight.value = levelData.Height;
        }
    }
}
