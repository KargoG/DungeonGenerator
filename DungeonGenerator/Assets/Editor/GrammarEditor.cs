using UnityEngine;
using UnityEditor;

public class GrammarEditor : EditorWindow
{
    [MenuItem("Window/DungeonCreator/GrammarEditor")]
    public static void ShowWindow()
    {
        GetWindow<GrammarEditor>();
    }

    void OnGUI()
    {
        // Window Code
    }
}
