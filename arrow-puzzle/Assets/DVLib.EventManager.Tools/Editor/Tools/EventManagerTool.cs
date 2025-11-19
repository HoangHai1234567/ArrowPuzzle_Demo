using MyExtension.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class EventManagerTool
    {
        [MenuItem("MyExtension/Show EventManager Tool")]
        static void ShowEventManagerTool()
        {
            EventManagerEditor.Show(typeof(EventManager));
        }

        //[InitializeOnLoadMethod]
        //static void OnProjectLoadedInEditor()
        //{
        //    Debug.Log("Project loaded in Unity Editor");
        //    //EventManagerEditor.Show(typeof(EventManager));
        //}

        //[DidReloadScripts(100)]
        //static void OnScriptsReloaded()
        //{
        //    Debug.Log("Reloaded.");
        //    EventManagerEditor.Show(typeof(EventManager));
        //}
    }
}

