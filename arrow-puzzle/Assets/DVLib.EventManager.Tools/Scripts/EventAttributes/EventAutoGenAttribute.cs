using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyExtension.Attributes
{
    [AttributeUsage(AttributeTargets.Event)]
    public class EventAutoGenAttribute: Attribute
    {
        public string[] ParamNames;

        public bool CreateRemoveMethod = false;
        public EventAutoGenAttribute()
        {
            
        }

        public EventAutoGenAttribute(params string[] paramNames)
        {
            ParamNames = paramNames;
        }

        public EventAutoGenAttribute(bool createRemoveMethod = false, params string[] paramNames)
        {
            ParamNames = paramNames;
            CreateRemoveMethod = createRemoveMethod;
        }
    }
}
