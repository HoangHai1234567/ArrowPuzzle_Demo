using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyExtension.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventManagerAutoGenAttribute : Attribute
    {
        public string ClearEventHandlerName;

        public EventManagerAutoGenAttribute()
        {

        }

        public EventManagerAutoGenAttribute(string clearEventHandlerName)
        {
            ClearEventHandlerName = clearEventHandlerName;
        }
    }
}
