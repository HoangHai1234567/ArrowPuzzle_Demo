using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyExtension.Editor
{
    public class DataTemplate
    {
        public static string StrFileTemplate =
            @"//Generated code: {7}
//Date: {8}
{0}
namespace {1}
{{
    public partial class {2}
    {{
        public partial class {3}
        {{
{4}
{5}
{6}
        }}
    }}
}}";

        public static string StrInvoke =
            @"
            public static void {0}Event({1})
            {{
                {0}?.Invoke({2});
            }}";

        public static string StrRemoveHandlerMethod =
            @"
            public static void Remove{0}Handler(Action{1} handler)
            {{
                if({0} != null)
                {{
                    {0} -= handler;
                }}
            }}";

        public static string StrRemoveHandlers =
            @"
                if({0} != null)
                {{
                    foreach(var handler in {0}.GetInvocationList())
                    {{
                         {0} -= (Action{1})handler;
                    }}
                }}";

        public static string StrClearHandlerMethod =
            @"
            public static void {0}()
            {{
                {1}
            }}";
    }
}

