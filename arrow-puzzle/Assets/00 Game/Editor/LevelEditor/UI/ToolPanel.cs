using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public class ToolPanel : VisualElement
    {
        private EditorToolItem _currentItem;
        private Dictionary<EditorToolType, EditorToolItem> _dic = new();

        public event Action<EditorToolType, EditorToolType> OnToolTypeChanged; 
        public ToolPanel()
        {
            var visualTreeAsset = Resources.Load<VisualTreeAsset>("UI/ToolPanel");
            var child = visualTreeAsset.CloneTree();
            Add(child);

            var scrollViewItems = child.Q<ScrollView>("scrollViewItems");
            foreach (var toolType in Enum.GetValues(typeof(EditorToolType)))
            {
                EditorToolItem item = new EditorToolItem((EditorToolType)toolType);
                item.OnClicked += Item_OnClicked;
                scrollViewItems.Add(item);
                _dic.Add(item.Type,item);
            }

            SelectTool(EditorToolType.SelectArrow);
        }

        private void Item_OnClicked(EditorToolItem item)
        {
            //_currentItem?.Normal();
            //_currentItem = item;
            //_currentItem.Selected();
            SelectTool(item.Type);
        }

        public void SelectTool(EditorToolType toolType)
        {
            if (_dic.ContainsKey(toolType))
            {
                var lastToolType = EditorToolType.SelectArrow;
                if (_currentItem != null)
                {
                    lastToolType = _currentItem.Type;
                }

                _currentItem?.Normal();
                _currentItem = _dic[toolType];
                _currentItem.Selected();

                OnToolTypeChanged?.Invoke(toolType,lastToolType);
            }
        }
    }
}
