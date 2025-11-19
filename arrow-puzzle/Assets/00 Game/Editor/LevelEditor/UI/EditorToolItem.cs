using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public class EditorToolItem : VisualElement
    {
        private static VisualTreeAsset _treeAsset;

        public event Action<EditorToolItem> OnClicked;

        private Color _normalColor;

        private Color _selectedColor;

        public EditorToolType Type { get; private set; }
        public EditorToolItem(EditorToolType toolTypeType)
        {
            Type = toolTypeType;
            if (_treeAsset == null)
            {
                _treeAsset = Resources.Load<VisualTreeAsset>("UI/EditorToolItem");
            }

            var child = _treeAsset.CloneTree();
            var labelName = child.Q<Label>("labelName");
            labelName.text = ToolTypeToName(Type);
            Add(child);

            RegisterCallback<MouseDownEvent>(OnMouseDown);

            _normalColor = child.style.backgroundColor.value;
            _selectedColor = new Color(1, 1, 0);
        }

        private string ToolTypeToName(EditorToolType type)
        {
            switch (type)
            {
                case EditorToolType.SelectArrow:
                    return "Select Arrow";
                case EditorToolType.AddArrow:
                    return "Add Arrow";
                case EditorToolType.RemoveArrow:
                    return "Remove Arrow";
                //case EditorToolType.AddHeadNode:
                //    return "Add Head Arrow";
                //case EditorToolType.RemoveHeadNode:
                //    return "Remove Head Arrow";
                //case EditorToolType.AddTailNode:
                //    return "Add Tail Arrow";
                //case EditorToolType.RemoveTailNode:
                //    return "Remove Tail Arrow";
                default:
                    break;
            }
            return type.ToString();
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            OnClicked?.Invoke(this);
        }

        public void Normal()
        {
            style.backgroundColor = _normalColor;
        }

        public void Selected()
        {
            style.backgroundColor = _selectedColor;
        }
    }
}
