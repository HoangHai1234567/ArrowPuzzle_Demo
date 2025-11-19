using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle.Editor
{
    public partial class EditorEventManager
    {
        public static event Action<float, float> OnGridSizeChanged;

        public static event Action<int, int> OnLevelSizeChanged;

        public static event Action<string> OnPreviewImageOpened;

        public static event Action<float> OnPreviewImageOpacityChanged;

        public static event Action<string,LevelData> OnLevelOpened;

        public static event Action<EditorLevelData> OnNewLevelCreated;

        public static event Action<string, EditorLevelData> OnLevelSaved; 


        public static void OnGridSizeChangedEvent(float newValue, float oldValue)
        {
            OnGridSizeChanged?.Invoke(newValue,oldValue);
        }

        public static void OnLevelSizeChangedEvent(int width, int height)
        {
            OnLevelSizeChanged?.Invoke(width,height);
        }

        public static void OnPreviewImageOpenedEvent(string texturePath)
        {
            OnPreviewImageOpened?.Invoke(texturePath);
        }

        public static void OnLevelOpenedEvent(string path, LevelData levelData)
        {
            OnLevelOpened?.Invoke(path,levelData);
        }

        public static void OnLevelSavedEvent(string path, EditorLevelData levelData)
        {
            OnLevelSaved?.Invoke(path,levelData);
        }

        public static void OnNewLevelCreatedEvent(EditorLevelData levelData)
        {
            OnNewLevelCreated?.Invoke(levelData);
        }

        public static void OnPreviewImageOpacityChangedEvent(float value)
        {
            OnPreviewImageOpacityChanged?.Invoke(value);
        }
        public static void ClearAllEventHandlers()
        {
            if (OnGridSizeChanged != null)
            {
                foreach (var handler in OnGridSizeChanged.GetInvocationList())
                {
                    OnGridSizeChanged -= (Action<float, float>)handler;
                }
            }

            if (OnLevelSizeChanged != null)
            {
                foreach (var handler in OnLevelSizeChanged.GetInvocationList())
                {
                    OnLevelSizeChanged -= (Action<int, int>)handler;
                }
            }

            if (OnPreviewImageOpened != null)
            {
                foreach (var handler in OnPreviewImageOpened.GetInvocationList())
                {
                    OnPreviewImageOpened -= (Action<string>)handler;
                }
            }

            if (OnPreviewImageOpacityChanged != null)
            {
                foreach (var handler in OnPreviewImageOpacityChanged.GetInvocationList())
                {
                    OnPreviewImageOpacityChanged -= (Action<float>)handler;
                }
            }

            if (OnLevelOpened != null)
            {
                foreach (var handler in OnLevelOpened.GetInvocationList())
                {
                    OnLevelOpened -= (Action<string, LevelData>)handler;
                }
            }

            if (OnLevelSaved != null)
            {
                foreach (var handler in OnLevelSaved.GetInvocationList())
                {
                    OnLevelSaved -= (Action<string, EditorLevelData>)handler;
                }
            }

            if (OnNewLevelCreated != null)
            {
                foreach (var handler in OnNewLevelCreated.GetInvocationList())
                {
                    OnNewLevelCreated -= (Action<EditorLevelData>)handler;
                }
            }
        }
    }
}
