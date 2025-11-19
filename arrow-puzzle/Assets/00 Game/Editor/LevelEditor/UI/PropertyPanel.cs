using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public class PropertyPanel : VisualElement
    {
        public PropertyPanel()
        {
            var visualTreeAsset = Resources.Load<VisualTreeAsset>("UI/PropertyPanel");
            Add(visualTreeAsset.CloneTree());
        }
    }
}
