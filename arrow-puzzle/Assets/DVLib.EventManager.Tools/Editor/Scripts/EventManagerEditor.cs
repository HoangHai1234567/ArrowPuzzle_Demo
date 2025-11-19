using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyExtension.Editor
{
#if UNITY_EDITOR
    public class EventManagerEditor : EditorWindow
    {
        //[MenuItem("Tools/EventManagerEditor")]
        //public static void Show()
        //{
        //    EventManagerEditor wnd = GetWindow<EventManagerEditor>();
        //    wnd.titleContent = new GUIContent("EventManagerEditor");
        //    wnd.SetTarget(typeof(HexaSort.EventManager));
        //}

        public static void Show(Type target)
        {
            EventManagerEditor wnd = GetWindow<EventManagerEditor>();
            wnd.titleContent = new GUIContent("EventManagerEditor");
            wnd.SetTarget(target);
        }

        private ScrollView _nestedClassContainer;

        public Type Target;
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            InitUI(root);


        }

        public void SetTarget(Type target)
        {
            Target = target;
            _codeGenerator = new EventManagerCodeGenerator(Target);

            if (_nestedClassContainer == null)
            {
                _nestedClassContainer = new ScrollView();
                _nestedClassContainer.mode = ScrollViewMode.Vertical;
            }
            else
            {
                _nestedClassContainer.Clear();
            }
            foreach (var nestedType in _codeGenerator.NestedType)
            {
                Toggle toggle = new Toggle(nestedType.Name)
                {
                    value = true
                };
                _nestedClassContainer.Add(toggle);
            }

        }
        private void InitUI(VisualElement root)
        {
            Button buttonGen = new Button
            {
                text = "Gen code"
            };
            buttonGen.clicked += ButtonGenOnclicked;

            Button buttonSelectAll = new Button()
            {
                text = "Select All"
            };
            buttonSelectAll.clicked += ButtonSelectAll_clicked;

            Button buttonUnSelectAll = new Button()
            {
                text = "Unselect All"
            };
            buttonUnSelectAll.clicked += ButtonUnSelectAll_clicked;

            _nestedClassContainer = new ScrollView()
            {
                mode = ScrollViewMode.Vertical
            };

            root.Add(buttonGen);
            root.Add(buttonSelectAll);
            root.Add(buttonUnSelectAll);

            Label label1 = new Label("Nested classes");
            root.Add(label1);
            root.Add(_nestedClassContainer);

        }

        private void ButtonUnSelectAll_clicked()
        {
            foreach (var child in _nestedClassContainer.Children())
            {
                if (child is Toggle toggle)
                {
                    toggle.value = false;
                }
            }
        }

        private void ButtonSelectAll_clicked()
        {
            foreach (var child in _nestedClassContainer.Children())
            {
                if (child is Toggle toggle)
                {
                    toggle.value = true;
                }
            }
        }

        private EventManagerCodeGenerator _codeGenerator;
        private void ButtonGenOnclicked()
        {
            List<bool> _checkLits = new List<bool>(_nestedClassContainer.childCount);
            foreach (var item in _nestedClassContainer.Children())
            {
                if (item is Toggle toggle)
                {
                    _checkLits.Add(toggle.value);
                }
            }
            _codeGenerator.GenCode(_checkLits);
        }


    }
#endif
}

