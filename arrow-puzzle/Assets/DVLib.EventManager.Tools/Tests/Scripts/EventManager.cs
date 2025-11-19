using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class EventManager
    {
        [EventClassAutoGen("ClearEventHandlers")]
        public partial class Game
        {
            [EventAutoGen("name")]
            public static event Action<string> OnStart;
            [EventAutoGen("number")]
            public static event Action<int> OnStop;

            private string t = $"public static void {0}Event({1} {2})/";
            public static void OnStartEvent(string name)
            {
                OnStart?.Invoke(name);
            }

            public static partial void ClearEventHandlers();

            public static partial void ClearEventHandlers()
            {
                if (OnStart != null)
                {
                    foreach (var handler in OnStart.GetInvocationList())
                    {
                        OnStart -= (Action<string>)handler;
                    }
                }
            }
        }
        [EventClassAutoGen]
        public partial class UI
        {
            [EventAutoGen("OnStart")]
            public static event Action<string> OnStart;
            [EventAutoGen("OnStop")]
            public static event Action<int> OnStop;

            public static void ClearEventHandlers()
            {

            }
        }

        public static void ClearAllEventHandlers()
        {

        }
    }
}
