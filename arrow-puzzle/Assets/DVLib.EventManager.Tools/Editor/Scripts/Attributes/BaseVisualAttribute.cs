using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyExtension.Editor
{
    public abstract class BaseVisualAttribute : Attribute
    {
        public string Text;

        public Color color = Color.white;

        public BaseVisualAttribute()
        {
        }
        public BaseVisualAttribute(string text)
        {
            Text = text;
        }

        public BaseVisualAttribute(string text,Color color)
        {
            Text = text;
            this.color = color;
        }
        public abstract VisualElement GetVisual();
    }
}

