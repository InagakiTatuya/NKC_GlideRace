using UnityEditor;
using UnityEngine;

public static class HierarchyStatusBarColor
{
    private static readonly Color[] guiColor = new Color[]{
        new Color(0.9f, 0, 0, 0.1f),
        new Color(0, 0, 0.9f, 0.1f),
    };

    [InitializeOnLoadMethod]
    private static void Example()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }

    private static void OnGUI(int instanceID, Rect selectionRect)
    {
        var index = (int)(selectionRect.y - 4) / 16;

        var pos     = selectionRect;
        pos.x       = 0;
        pos.xMax    = selectionRect.xMax;
            
        var color   = GUI.color;
        GUI.color   = guiColor[index%2];
        GUI.Box(pos, string.Empty);
        GUI.color   = color;
    }
}