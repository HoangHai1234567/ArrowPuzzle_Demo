using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [AttributeUsage(AttributeTargets.Event)]
    public class EventAutoGenAttribute : Attribute
    {
        public string[] ParamNames;
        public EventAutoGenAttribute(params string[] paramNames)
        {
            ParamNames = paramNames;
        }
        public EventAutoGenAttribute()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EventClassAutoGenAttribute : Attribute
    {
        public string ClearHandlerMethodName;
        public EventClassAutoGenAttribute(string clearHandlerMethodName)
        {
            ClearHandlerMethodName = clearHandlerMethodName;
        }

        public EventClassAutoGenAttribute()
        {

        }
    }
}
