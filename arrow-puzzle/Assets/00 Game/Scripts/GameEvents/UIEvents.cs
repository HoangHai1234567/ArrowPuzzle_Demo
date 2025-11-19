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
        public partial class UIEvents
        {
            /// <summary>
            /// delay,
            /// withAnim
            /// </summary>
            [EventAutoGen("delay","withAnim")]
            public static event Action<float,bool> OnRequestShowUILevelCompleted;

            /// <summary>
            /// delay,
            /// withAnim
            /// </summary>
            [EventAutoGen("delay", "withAnim")]
            public static event Action<float,bool> OnRequestShowUIGameOver;


        }
    }

}
