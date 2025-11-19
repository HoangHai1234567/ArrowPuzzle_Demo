//Generated code: EventManager_GamePlay_Generated.cs
//Date: 11/14/2025 9:38:26 AM
using ArrowsPuzzle;
using System;

namespace ArrowsPuzzle
{
    public partial class EventManager
    {
        public partial class GamePlay
        {

            public static void OnLevelLoadedEvent(Level level)
            {
                OnLevelLoaded?.Invoke(level);
            }
            public static void OnLevelCreatedEvent(GameManager gameManager, Level level)
            {
                OnLevelCreated?.Invoke(gameManager, level);
            }
            public static void OnGameStartedEvent(GameManager gameManager)
            {
                OnGameStarted?.Invoke(gameManager);
            }
            public static void OnGameInitEvent(GameManager gameManager)
            {
                OnGameInit?.Invoke(gameManager);
            }
            public static void OnGamePlayEvent()
            {
                OnGamePlay?.Invoke();
            }
            public static void OnGameCompletedEvent()
            {
                OnGameCompleted?.Invoke();
            }
            public static void OnGameOverEvent()
            {
                OnGameOver?.Invoke();
            }
            public static void OnGameReplayEvent()
            {
                OnGameReplay?.Invoke();
            }
            public static void OnGameReviveEvent()
            {
                OnGameRevive?.Invoke();
            }
            public static void OnGamePausedEvent()
            {
                OnGamePaused?.Invoke();
            }
            public static void OnNextLevelEvent()
            {
                OnNextLevel?.Invoke();
            }
            public static void OnNeedHintEvent()
            {
                OnNeedHint?.Invoke();
            }
            public static void OnHintedEvent(Arrow hintArrow)
            {
                OnHinted?.Invoke(hintArrow);
            }
            public static void OnLostLivesEvent(int lives, int maxLives)
            {
                OnLostLives?.Invoke(lives, maxLives);
            }
            public static void OnFillLivesEvent(int lives, int maxLives)
            {
                OnFillLives?.Invoke(lives, maxLives);
            }
            public static void OnArrowHighlightEvent(Arrow arrow)
            {
                OnArrowHighlight?.Invoke(arrow);
            }
            public static void OnArrowUnHighlightEvent(Arrow arrow)
            {
                OnArrowUnHighlight?.Invoke(arrow);
            }
            public static void OnEnableInteractionEvent(bool enable)
            {
                OnEnableInteraction?.Invoke(enable);
            }


            public static void ClearEventHandlers()
            {
                
                if(OnLevelLoaded != null)
                {
                    foreach(var handler in OnLevelLoaded.GetInvocationList())
                    {
                         OnLevelLoaded -= (Action<Level>)handler;
                    }
                }

                if(OnLevelCreated != null)
                {
                    foreach(var handler in OnLevelCreated.GetInvocationList())
                    {
                         OnLevelCreated -= (Action<GameManager, Level>)handler;
                    }
                }

                if(OnGameStarted != null)
                {
                    foreach(var handler in OnGameStarted.GetInvocationList())
                    {
                         OnGameStarted -= (Action<GameManager>)handler;
                    }
                }

                if(OnGameInit != null)
                {
                    foreach(var handler in OnGameInit.GetInvocationList())
                    {
                         OnGameInit -= (Action<GameManager>)handler;
                    }
                }

                if(OnGamePlay != null)
                {
                    foreach(var handler in OnGamePlay.GetInvocationList())
                    {
                         OnGamePlay -= (Action)handler;
                    }
                }

                if(OnGameCompleted != null)
                {
                    foreach(var handler in OnGameCompleted.GetInvocationList())
                    {
                         OnGameCompleted -= (Action)handler;
                    }
                }

                if(OnGameOver != null)
                {
                    foreach(var handler in OnGameOver.GetInvocationList())
                    {
                         OnGameOver -= (Action)handler;
                    }
                }

                if(OnGameReplay != null)
                {
                    foreach(var handler in OnGameReplay.GetInvocationList())
                    {
                         OnGameReplay -= (Action)handler;
                    }
                }

                if(OnGameRevive != null)
                {
                    foreach(var handler in OnGameRevive.GetInvocationList())
                    {
                         OnGameRevive -= (Action)handler;
                    }
                }

                if(OnGamePaused != null)
                {
                    foreach(var handler in OnGamePaused.GetInvocationList())
                    {
                         OnGamePaused -= (Action)handler;
                    }
                }

                if(OnNextLevel != null)
                {
                    foreach(var handler in OnNextLevel.GetInvocationList())
                    {
                         OnNextLevel -= (Action)handler;
                    }
                }

                if(OnNeedHint != null)
                {
                    foreach(var handler in OnNeedHint.GetInvocationList())
                    {
                         OnNeedHint -= (Action)handler;
                    }
                }

                if(OnHinted != null)
                {
                    foreach(var handler in OnHinted.GetInvocationList())
                    {
                         OnHinted -= (Action<Arrow>)handler;
                    }
                }

                if(OnLostLives != null)
                {
                    foreach(var handler in OnLostLives.GetInvocationList())
                    {
                         OnLostLives -= (Action<int, int>)handler;
                    }
                }

                if(OnFillLives != null)
                {
                    foreach(var handler in OnFillLives.GetInvocationList())
                    {
                         OnFillLives -= (Action<int, int>)handler;
                    }
                }

                if(OnArrowHighlight != null)
                {
                    foreach(var handler in OnArrowHighlight.GetInvocationList())
                    {
                         OnArrowHighlight -= (Action<Arrow>)handler;
                    }
                }

                if(OnArrowUnHighlight != null)
                {
                    foreach(var handler in OnArrowUnHighlight.GetInvocationList())
                    {
                         OnArrowUnHighlight -= (Action<Arrow>)handler;
                    }
                }

                if(OnEnableInteraction != null)
                {
                    foreach(var handler in OnEnableInteraction.GetInvocationList())
                    {
                         OnEnableInteraction -= (Action<bool>)handler;
                    }
                }

            }
        }
    }
}