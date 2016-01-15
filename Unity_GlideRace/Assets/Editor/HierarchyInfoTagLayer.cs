using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad]
public class HierarchyInfoTagLayer
{
    static HierarchyInfoTagLayer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }

    static void OnGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        var rect = selectionRect;
        rect.x += rect.width - 50;

        var str = Cut(obj.tag) + "/" + Cut(LayerMask.LayerToName(obj.layer));
        var style = new GUIStyle();
        style.fontStyle = FontStyle.Italic;
        GUI.Label(rect, str, style);
    }

    private static string Cut(string s, int length = 3)
    {
        if (s.Length >= length) return s.Substring(0, length);
        return s;
    }
}
