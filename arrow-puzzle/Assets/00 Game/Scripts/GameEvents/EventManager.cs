using System.Collections;
using System.Collections.Generic;
using MyExtension.Attributes;
using UnityEngine;

namespace ArrowsPuzzle
{
    [EventManagerAutoGen]
    public partial class EventManager
    {
        public static void ClearAllEventHandlers()
        {
            GamePlay.ClearEventHandlers();
            UIEvents.ClearEventHandlers();
        }
    }
}
