//Generated code: EventManager_UIEvents_Generated.cs
//Date: 11/12/2025 9:55:57 AM
using System;

namespace ArrowsPuzzle
{
    public partial class EventManager
    {
        public partial class UIEvents
        {

            public static void OnRequestShowUILevelCompletedEvent(float delay,bool withAnim)
            {
                OnRequestShowUILevelCompleted?.Invoke(delay,withAnim);
            }
            public static void OnRequestShowUIGameOverEvent(float delay, bool withAnim)
            {
                OnRequestShowUIGameOver?.Invoke(delay, withAnim);
            }


            public static void ClearEventHandlers()
            {
                
                if(OnRequestShowUILevelCompleted != null)
                {
                    foreach(var handler in OnRequestShowUILevelCompleted.GetInvocationList())
                    {
                         OnRequestShowUILevelCompleted -= (Action<float,bool>)handler;
                    }
                }

                if(OnRequestShowUIGameOver != null)
                {
                    foreach(var handler in OnRequestShowUIGameOver.GetInvocationList())
                    {
                         OnRequestShowUIGameOver -= (Action<float,bool>)handler;
                    }
                }

            }
        }
    }
}