using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public class BottomBar : VisualElement
    {
        private Label _labelInfo;
        public BottomBar()
        {
            var visualTreeAsset = Resources.Load<VisualTreeAsset>("UI/BottomBar");
            var child = visualTreeAsset.CloneTree();
            Add(child);

            _labelInfo = child.Q<Label>("labelInfo");
            EditorEventManager.OnLevelOpened += EditorEventManager_OnLevelOpened;
            EditorEventManager.OnNewLevelCreated += EditorEventManager_OnNewLevelCreated;
            EditorEventManager.OnLevelSaved += EditorEventManager_OnLevelSaved;
        }

        private void EditorEventManager_OnLevelSaved(string path, EditorLevelData levelData)
        {
            _labelInfo.text = $"Level : {path}";
        }

        private void EditorEventManager_OnNewLevelCreated(EditorLevelData levelData)
        {
            _labelInfo.text = "Level :";
        }

        private void EditorEventManager_OnLevelOpened(string path, LevelData levelData)
        {
            _labelInfo.text = $"Level : {path}";
        }
    }
}
