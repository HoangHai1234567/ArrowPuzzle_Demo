using System;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public class LevelEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("ArrowPuzzle/LevelEditor")]
        public static void ShowEditor()
        {
            LevelEditor wnd = GetWindow<LevelEditor>();
            wnd.titleContent = new GUIContent("LevelEditor");
        }

        private MainPanel _mainPanel;

        private MainCanvas _mainCanvas;

        private PropertyPanel _propertyPanel;

        private ToolPanel toolPanel;

        public void CreateGUI()
        {
            EditorEventManager.ClearAllEventHandlers();
            VisualElement root = rootVisualElement;

            TwoPaneSplitView _twoPaneSplitView1 = new TwoPaneSplitView(0,300,TwoPaneSplitViewOrientation.Horizontal);
            _twoPaneSplitView1.style.minWidth = new StyleLength(200);
            root.Add(_twoPaneSplitView1);

            _mainPanel = new MainPanel();
            _mainPanel.OnButtonNewLevel += _mainPanel_OnButtonNewLevel;
            _mainPanel.OnButtonOpenLevel += _mainPanel_OnButtonOpenLevel;
            _mainPanel.OnButtonSaveLevel += _mainPanel_OnButtonSaveLevel;
            _mainPanel.OnButtonPlay += _mainPanel_OnButtonPlay;
            _mainPanel.OnGridSizeChanged += _mainPanel_OnGridSizeChanged;
            _twoPaneSplitView1.Add(_mainPanel);

            VisualElement _container1 = new VisualElement();
            _twoPaneSplitView1.Add(_container1);

            toolPanel = new ToolPanel();
            toolPanel.OnToolTypeChanged += ToolPanelOnToolTypeChanged;
            _container1.Add(toolPanel);

            TwoPaneSplitView _twoPaneSplitView2 = new TwoPaneSplitView(1,250,TwoPaneSplitViewOrientation.Horizontal);
            _container1.Add(_twoPaneSplitView2);

            ScrollView _scrollView1 = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            _twoPaneSplitView2.Add(_scrollView1);

            _mainCanvas = new MainCanvas();
            _scrollView1.Add(_mainCanvas);

            _propertyPanel = new PropertyPanel();
            _twoPaneSplitView2.Add(_propertyPanel);

            BottomBar bottomBar = new BottomBar();
            root.Add(bottomBar);
        }

        private void _mainPanel_OnGridSizeChanged(int newValue, int oldValue)
        {
            //_mainCanvas.ChangeGirdSize(newValue,oldValue);
        }

        private void _mainPanel_OnButtonPlay()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
            else
            {
                var text = _mainCanvas.GetTextLevel();
                PlayerPrefs.SetInt(Values.GameDataKeys.Test, 1);
                PlayerPrefs.SetString(Values.GameDataKeys.TestLevel, text);

                PlayerPrefs.Save();

                EditorApplication.isPlaying = true;
            }

        }

        private void _mainPanel_OnButtonSaveLevel()
        {
            var text = _mainCanvas.GetTextLevel();
            string path = EditorUtility.SaveFilePanel("Save Level", String.Empty, "level", "json");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path,text);
                EditorEventManager.OnLevelSavedEvent(path,_mainCanvas.LevelData);
                EditorUtility.DisplayDialog("Save Level", $"Level saved: {path}", "OK");
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("Save Level", $"Cannot save level: {path}", "OK");
            }
        }

        private void _mainPanel_OnButtonOpenLevel()
        {
            string path = EditorUtility.OpenFilePanel("Open Level", String.Empty, "json");
            if (!string.IsNullOrEmpty(path))
            {
                var text = File.ReadAllText(path);
                if (!string.IsNullOrEmpty(text))
                {
                    var levelData = JsonConvert.DeserializeObject<LevelData>(text);
                    if (levelData != null)
                    {
                        _mainCanvas.OpenLevel(levelData);
                        _mainPanel.SetLevelData(levelData);
                        EditorEventManager.OnLevelOpenedEvent(path,levelData);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Open Level", $"Invalid level: {path}", "OK");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Open Level", $"Cannot open level: {path}", "OK");
                }
            }
            //_mainCanvas.OpenLevel();
        }

        private void _mainPanel_OnButtonNewLevel()
        {
            _mainCanvas.CreateNewLevel();

        }

        private void ToolPanelOnToolTypeChanged(EditorToolType toolType, EditorToolType lastToolType)
        {
            _mainCanvas.ChangeToolType(toolType);
        }
    }
}

