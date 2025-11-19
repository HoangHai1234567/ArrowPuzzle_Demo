using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TestEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/TestEditor")]
    public static void ShowExample()
    {
        TestEditor wnd = GetWindow<TestEditor>();
        wnd.titleContent = new GUIContent("TestEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        TwoPaneSplitView myElement = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        //myElement.orientation = TwoPaneSplitViewOrientation.Horizontal;

        // Add two child elements to the TwoPaneSplitView
        VisualElement child1 = new VisualElement();
        VisualElement child2 = new VisualElement();
        VisualElement child3 = new VisualElement();
        TwoPaneSplitView subPaneSplitView = new TwoPaneSplitView(1, 250, TwoPaneSplitViewOrientation.Horizontal);

        myElement.Add(child1);
        myElement.Add(subPaneSplitView);
        //myElement.Add(child3);

        Button button1 = new Button();
        button1.text = "Button 1";

        Button button2 = new Button();
        button2.text = "Button 2";
        subPaneSplitView.Add(child2);
        subPaneSplitView.Add(child3);

        child1.Add(button1);
        child3.Add(button2);
        //child2.Add(button2);
        //child3.Add(button3);

        root.Add(myElement);
    }
}
