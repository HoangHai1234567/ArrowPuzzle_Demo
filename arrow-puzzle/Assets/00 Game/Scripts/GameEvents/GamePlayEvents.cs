using System;
using System.Collections;
using System.Collections.Generic;
using MyExtension.Attributes;
using UnityEngine;

namespace ArrowsPuzzle
{
    public partial class EventManager
    {
        [EventManagerAutoGen]
        public partial class GamePlay
        {
            [EventAutoGen ("level")]
            public static event Action<Level> OnLevelLoaded;

            [EventAutoGen("gameManager","level")]
            public static event Action<GameManager,Level> OnLevelCreated;

            [EventAutoGen("gameManager")]
            public static event Action<GameManager> OnGameStarted;

            [EventAutoGen("gameManager")]
            public static event Action<GameManager> OnGameInit;

            [EventAutoGen]
            public static event Action OnGamePlay;

            [EventAutoGen]
            public static event Action OnGameCompleted;

            [EventAutoGen]
            public static event Action OnGameOver;

            [EventAutoGen]
            public static event Action OnGameReplay;

            [EventAutoGen]
            public static event Action OnGameRevive; 

            [EventAutoGen]
            public static event Action OnGamePaused;

            [EventAutoGen]
            public static event Action OnNextLevel;

            [EventAutoGen]
            public static event Action OnNeedHint;

            [EventAutoGen("hintArrow")]
            public static event Action<Arrow> OnHinted;

            [EventAutoGen("lives","maxLives")]
            public static event Action<int, int> OnLostLives;

            [EventAutoGen("lives", "maxLives")]
            public static event Action<int, int> OnFillLives;

            [EventAutoGen("arrow")]
            public static event Action<Arrow> OnArrowHighlight;

            [EventAutoGen("arrow")]
            public static event Action<Arrow> OnArrowUnHighlight;

            [EventAutoGen("enable")]
            public static event Action<bool> OnEnableInteraction;

        }
    }
}
