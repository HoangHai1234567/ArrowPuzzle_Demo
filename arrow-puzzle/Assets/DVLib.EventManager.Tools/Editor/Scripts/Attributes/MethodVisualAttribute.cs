using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyExtension.Editor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodVisualAttribute : BaseVisualAttribute
    {
        public MethodInfo methodInfo;

        public object Target;
        public MethodVisualAttribute(string text) : base(text)
        {

        }

        public MethodVisualAttribute(string text,Color c) : base(text,c)
        {
        }

        public MethodVisualAttribute()
        {

        }

        private object[] _paramFields;
        public override VisualElement GetVisual()
        {
            VisualElement container = new VisualElement();
            //GroupBox groupBox = new GroupBox();

            var ps = methodInfo.GetParameters();
            _paramFields = new object[ps.Length];
            foreach (var p in ps)
            {
                if (p.ParameterType == typeof(string))
                {
                    TextField field = new TextField
                    {
                        label = p.Name,
                        value = p.DefaultValue as string
                    };
                    field.RegisterValueChangedCallback((evt =>
                    {
                        _paramFields[p.Position] = evt.newValue;
                    }));
                    _paramFields[p.Position] = field.value;
                    container.Add(field);
                }
                else if (p.ParameterType == typeof(int))
                {
                    IntegerField field = new IntegerField
                    {
                        label = p.Name,
                        value = p.DefaultValue is int ? (int)p.DefaultValue : 0
                    };
                    field.RegisterValueChangedCallback((evt =>
                    {
                        _paramFields[p.Position] = evt.newValue;
                    }));
                    _paramFields[p.Position] = field.value;
                    container.Add(field);
                }

            }

            Button button = new Button();
            button.style.color = color;
            if (string.IsNullOrEmpty(Text))
            {
                button.text = methodInfo.Name;
            }
            else
            {
                button.text = Text;
            }


            button.clicked += () =>
            {
                methodInfo.Invoke(Target, _paramFields);
            };
            //groupBox.Add(button);
            container.Add(button);
            return container;
        }


    }
}
