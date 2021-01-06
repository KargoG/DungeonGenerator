using System.Collections.Generic;
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

        List<Gameplay> gameplayToDelete = new List<Gameplay>();

        EditorGUILayout.LabelField("Defined Gameplay:");

        for (int i = 0; i < DataAccess.GetGameplayContainer().GetAmountOfGameplay(); i++)
        {
            Gameplay toShow = DataAccess.GetGameplayContainer().GetGameplay(i);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(toShow.Action.Name);
            EditorGUILayout.LabelField(toShow.Entity.Name);

            if (toShow.Ability != null)
            {
                EditorGUILayout.LabelField("by " + toShow.Ability.Name);
            }
            else if (toShow.Consumable != null)
            {
                EditorGUILayout.LabelField("with a " + toShow.Consumable.Name);
            }


            if (GUILayout.Button("Edit"))
            {
                // TODO
            }

            if (GUILayout.Button("Delete"))
            {
                gameplayToDelete.Add(toShow);
            }

            EditorGUILayout.EndHorizontal();
        }

        // Deleting all that needs to go
        foreach (Gameplay gameplay in gameplayToDelete)
        {
            DataAccess.GetGameplayContainer().RemoveGameplay(gameplay);
        }
        

        if (GUILayout.Button("New Gameplay"))
        {
            GameplayCreator.ShowGameplayCreator();
        }

        AssetDatabase.SaveAssets();
    }
}
