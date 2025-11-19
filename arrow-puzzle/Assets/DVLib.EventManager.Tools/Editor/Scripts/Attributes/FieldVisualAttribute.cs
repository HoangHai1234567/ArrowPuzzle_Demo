using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyExtension.Editor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldVisualAttribute : BaseVisualAttribute
    {
        public FieldInfo fieldInfo;

        public object Target;
        public override VisualElement GetVisual()
        {
            VisualElement container = new VisualElement();

            if (fieldInfo != null)
            {
                var value = fieldInfo.GetValue(Target);

                if (fieldInfo.FieldType.IsEnum)
                {
                    EnumField field = new EnumField();

                    //field.value = value as Enum;
                    field.Init(value as Enum);
                    if (string.IsNullOrEmpty(Text))
                    {
                        field.label = fieldInfo.Name;
                    }
                    else
                    {
                        field.label = Text;
                    }
                    
                    field.RegisterValueChangedCallback((evt =>
                    {
                        fieldInfo.SetValue(Target, evt.newValue);
                    }));
                    container.Add(field);
                }
                else
                {
                    if (value is int i)
                    {
                        var field = CreateIntegerField(i);
                        container.Add(field);
                    }
                    else if (value is float f)
                    {
                        var field = CreateFloatField(f);
                        container.Add(field);
                    }
                    else if (value is string s)
                    {
                        var field = CreateStringField(s);
                        container.Add(field);
                    }
                }

            }

            return container;
        }


        VisualElement CreateIntegerField(int value)
        {
            IntegerField field = new IntegerField
            {
                value = value,
                label = string.IsNullOrEmpty(Text) ? fieldInfo.Name : Text
            };

            field.RegisterValueChangedCallback((evt =>
            {
                fieldInfo.SetValue(Target, evt.newValue);
            }));
            return field;
        }

        VisualElement CreateFloatField(float value)
        {
            FloatField field = new FloatField()
            {
                value = value,
                label = string.IsNullOrEmpty(Text) ? fieldInfo.Name : Text
            };

            field.RegisterValueChangedCallback((evt =>
            {
                fieldInfo.SetValue(Target, evt.newValue);
            }));
            return field;
        }

        VisualElement CreateStringField(string value)
        {
            TextField field = new TextField()
            {
                value = value,
                label = string.IsNullOrEmpty(Text) ? fieldInfo.Name : Text
            };

            field.RegisterValueChangedCallback((evt =>
            {
                fieldInfo.SetValue(Target, evt.newValue);
            }));
            return field;
        }
        public FieldVisualAttribute(string text) : base(text)
        {
        }

        public FieldVisualAttribute()
        {

        }
    }
}
