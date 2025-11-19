using MyExtension.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#endif

using UnityEngine;

namespace MyExtension.Editor
{
#if UNITY_EDITOR
    public class EventManagerCodeGenerator
    {
        private List<string> _generatedPaths = new();

        private Type _type;
        public EventManagerCodeGenerator(Type type)
        {
            _type = type;
            _nestedTypes = GetAllNestedClass(_type);
        }
        void SaveGeneratedCode(string path, string code)
        {
            try
            {
                _generatedPaths.Add(path);
                string text = code;
                File.WriteAllText(path, text);

            }
            catch (Exception e)
            {
                Debug.LogError($"Fail to save file: {path}.\n{e.Message}");
                throw;
            }
        }

        private List<string> _usingNamespaces = new();
        private List<Type> _nestedTypes = new();
        public List<Type> NestedType => _nestedTypes;
        public void GenCode(List<bool> checkList = null)
        {
            if (_type == null)
            {
                Debug.LogError($"Type not found!");
                return;
            }

            if (checkList != null)
            {
                if (checkList.Count == 0)
                {
                    Debug.LogError($"There is no class to generate code!");
                    return;
                }

                if (_nestedTypes.Count != checkList.Count)
                {
                    Debug.Log("Invalid checkList value");
                    return;
                }
            }


            var folderPath = EditorUtility.SaveFolderPanel("Generate code",
                String.Empty,
                String.Empty);
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }
            _usingNamespaces.Clear();
            List<Type> nestedTypes = new List<Type>();

            if (checkList != null)
            {
                int count = 0;
                foreach (var check in checkList)
                {
                    if (check)
                    {
                        nestedTypes.Add(_nestedTypes[count]);
                    }

                    count++;
                }
            }
            else
            {
                nestedTypes = _nestedTypes;
            }

            _generatedPaths.Clear();
            GenSelectedTypes(_type, nestedTypes, folderPath, (success =>
            {
                if (success)
                {
                    //AssetDatabase.Refresh();
                    StringBuilder sb = new StringBuilder();
                    foreach (var path in _generatedPaths)
                    {
                        sb.AppendLine(path);
                    }
                    EditorUtility.DisplayDialog("Save Generated code", $"Files:\n {sb}Saved!", "OK");
                }
            }));


        }

        private List<Type> GetAllNestedClass(Type type)
        {
            List<Type> validTypes = new List<Type>();
            _nestedTypes.Clear();
            var nestedTypes = type.GetNestedTypes();

            foreach (var nestedType in nestedTypes)
            {
                var attribute = nestedType.GetCustomAttribute<EventManagerAutoGenAttribute>();
                if (attribute != null)
                {
                    validTypes.Add(nestedType);
                }
            }
            return validTypes;
        }

        void GenSelectedTypes(Type type, List<Type> nestedTypes, string folderPath, Action<bool> callback)
        {
            EditorCoroutineUtility.StartCoroutine(_Gen(), this);
            IEnumerator _Gen()
            {
                foreach (var nestedType in nestedTypes)
                {
                    var attribute = nestedType.GetCustomAttribute<EventManagerAutoGenAttribute>();
                    if (attribute != null)
                    {
                        string fileName = $"{type.Name}_{nestedType.Name}_Generated.cs";
                        var result = GenNestedClassEvent(nestedType, attribute);
                        StringBuilder sb = new StringBuilder();

                        if (!_usingNamespaces.Contains("System"))
                        {
                            _usingNamespaces.Add("System");
                        }
                        foreach (var usingNamespace in _usingNamespaces)
                        {
                            sb.AppendLine($"using {usingNamespace};");
                        }
                        string final = string.Format(
                            DataTemplate.StrFileTemplate,
                            sb,
                            type.Namespace,
                            type.Name,
                            nestedType.Name,
                            result.Item1,
                            result.Item2,
                            result.Item3,
                            fileName,
                            DateTime.Now);

                        string path = $"{folderPath}/{fileName}";
                        if (!string.IsNullOrEmpty(path))
                        {
                            SaveGeneratedCode(path, final);
                        }

                        yield return null;
                    }
                }
                callback?.Invoke(true);
            }
        }

        (string,string, string) GenNestedClassEvent(Type type, EventManagerAutoGenAttribute attribute)
        {
            var events = type.GetEvents();
            //string str2 = "if({0}!=null)\n{{\n\tforeach(var handler in {0}.GetInvocationList())\n\t{{\n\t\t{0} -= (Action<{1}>)handler;\n\t}}\n}}";

            StringBuilder sbClearEventHandlers = new StringBuilder();
            StringBuilder sbInvokeEvents = new StringBuilder();
            StringBuilder sbRemoveEventHandler = new StringBuilder();
            int eventCount = 0;
            foreach (var eventInfo in events)
            {
                if (!eventInfo.GetAddMethod().IsStatic)
                {
                    continue;
                }
                if (eventInfo.GetCustomAttribute(typeof(EventAutoGenAttribute)) is EventAutoGenAttribute customAttribute)
                {
                    eventCount++;

                    string[] paramNames = customAttribute.ParamNames;
                    StringBuilder sbInvokeEvent = new StringBuilder();
                    StringBuilder sbInvokeParams = new StringBuilder();
                    StringBuilder sbGenericParams = new StringBuilder();

                    int count = 0;
                    if (eventInfo.EventHandlerType.GenericTypeArguments.Length > 0)
                    {
                        sbGenericParams.Append("<");
                    }
                    foreach (var gt in eventInfo.EventHandlerType.GenericTypeArguments)
                    {
                        //process param name
                        string paramName;
                        if (paramNames != null && count < paramNames.Length)
                        {
                            paramName = paramNames[count];
                        }
                        else
                        {
                            paramName = $"param{count}";
                        }

                        //process using namespaces
                        if (!string.IsNullOrEmpty(gt.Namespace) &&!_usingNamespaces.Contains(gt.Namespace))
                        {
                            _usingNamespaces.Add(gt.Namespace);
                        }

                        string paramTypeName = string.Empty;
                        StringBuilder finalSb = new StringBuilder();
                        ProcessArguments(gt, ref finalSb);
                        paramTypeName = finalSb.ToString();

                        sbInvokeEvent.Append($"{paramTypeName} {paramName}, ");
                        sbInvokeParams.Append($"{paramName}, ");
                        sbGenericParams.Append($"{paramTypeName}, ");

                        count++;
                    }

                    if (sbInvokeEvent.Length > 1)
                    {
                        sbInvokeEvent.Remove(sbInvokeEvent.Length - 2, 2);
                        sbInvokeParams.Remove(sbInvokeParams.Length - 2, 2);
                        sbGenericParams.Remove(sbGenericParams.Length - 2, 2);
                    }
                    if (eventInfo.EventHandlerType.GenericTypeArguments.Length > 0)
                    {
                        sbGenericParams.Append(">");
                    }
                    var infoInvoke = string.Format(DataTemplate.StrInvoke, eventInfo.Name, sbInvokeEvent, sbInvokeParams);

                    sbInvokeEvents.Append(infoInvoke);
                    //sbInvokeEvents.AppendLine();

                    if (customAttribute.CreateRemoveMethod)
                    {
                        var infoRemove = string.Format(DataTemplate.StrRemoveHandlerMethod, eventInfo.Name, sbGenericParams);
                        sbRemoveEventHandler.Append(infoRemove);
                    }



                    var infoClear = string.Format(DataTemplate.StrRemoveHandlers, eventInfo.Name, sbGenericParams);

                    sbClearEventHandlers.Append($"{infoClear}");
                    sbClearEventHandlers.AppendLine();
                    //sbClearEventHandlers.AppendLine();
                }

            }



            string clearEventHandlerName = "ClearEventHandlers";
            if (!string.IsNullOrEmpty(attribute.ClearEventHandlerName))
            {
                clearEventHandlerName = attribute.ClearEventHandlerName;
            }
            string final = String.Format(DataTemplate.StrClearHandlerMethod, clearEventHandlerName, sbClearEventHandlers);

            if (eventCount == 0)
            {
                EditorUtility.DisplayDialog("Gen code", "There is no 'static event to generate code!", "OK");
            }

            return (sbInvokeEvents.ToString(),sbRemoveEventHandler.ToString(), final);
        }

        public void ProcessArguments(Type gt, ref StringBuilder finalSb)
        {

            if (gt.IsGenericType)
            {
                int indexOf = gt.Name.IndexOf('`');
                var paramTypeName = gt.Name.Substring(0, indexOf);

                finalSb.Append(paramTypeName);
                finalSb.Append("<");
                foreach (var typeArgument in gt.GenericTypeArguments)
                {
                    //process using namespaces
                    if (!_usingNamespaces.Contains(typeArgument.Namespace))
                    {
                        _usingNamespaces.Add(typeArgument.Namespace);
                    }

                    if (typeArgument.IsGenericType)
                    {
                        ProcessArguments(typeArgument, ref finalSb);
                    }
                    else
                    {
                        finalSb.Append(DataUtils.ConvertTypeName(typeArgument.Name));
                    }
                    if (gt.GenericTypeArguments.Length > 1)
                    {
                        finalSb.Append(",");
                    }

                }
                if (gt.GenericTypeArguments.Length > 1)
                {
                    finalSb.Remove(finalSb.Length - 1, 1);
                }
                finalSb.Append(">");
            }
            else
            {
                finalSb.Append(DataUtils.ConvertTypeName(gt.Name));
            }

        }
    }
#endif
}
