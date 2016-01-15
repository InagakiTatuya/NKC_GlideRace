using System.Linq;
using UnityEditor;
using UnityEngine;

public static class InvalidComponents
{
    private const int WIDTH = 10;

    [InitializeOnLoadMethod]
    private static void Example()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }

    private static void OnGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        var isNothing = obj.GetComponents<MonoBehaviour>().Any(c => c == null);
        if (!isNothing) return;

        var rect = selectionRect;
        rect.x = rect.xMin - WIDTH;
        rect.width = WIDTH;

        var style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontSize = 14;
        style.fontStyle = FontStyle.Bold;
        GUI.Label(rect, "!", style);
    }
}