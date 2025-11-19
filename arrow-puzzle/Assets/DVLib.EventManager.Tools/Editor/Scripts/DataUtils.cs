using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyExtension
{
    public class DataUtils
    {
        public static Dictionary<string, string> BuildInTypesConvert = new Dictionary<string, string>()
        {
            {"Boolean","bool"},
            {"Byte","byte"},
            {"SByte","sbyte"},
            {"Char","char"},
            {"Decimal","decimal"},
            {"Double","double"},
            {"Single","float"},
            {"Int32","int"},
            {"UInt32","uint"},
            {"IntPtr","nint"},
            {"UIntPtr","nuint"},
            {"Int64","long"},
            {"UInt64","ulong"},
            {"Int16","short"},
            {"UInt16","ushort"},
            {"Object","object"},
            {"String","string"},

            {"Boolean[]","bool[]"},
            {"Byte[]","byte[]"},
            {"SByte[]","sbyte[]"},
            {"Char[]","char[]"},
            {"Decimal[]","decimal[]"},
            {"Double[]","double[]"},
            {"Single[]","float[]"},
            {"Int32[]","int[]"},
            {"UInt32[]","uint[]"},
            {"IntPtr[]","nint[]"},
            {"UIntPtr[]","nuint[]"},
            {"Int64[]","long[]"},
            {"UInt64[]","ulong[]"},
            {"Int16[]","short[]"},
            {"UInt16[]","ushort[]"},
            {"Object[]","object[]"},
            {"String[]","string[]"},
        };

        public static string ConvertTypeName(string typeName)
        {
            if (DataUtils.BuildInTypesConvert.ContainsKey(typeName))
            {
                return DataUtils.BuildInTypesConvert[typeName];
            }
            return typeName;
        }
    }


}

