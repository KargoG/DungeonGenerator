using System.Collections.Generic;
using DungeonGenerator;
using DungeonGenerator.Editor;
using UnityEngine;
using UnityEditor;

namespace DungeonGenerator.Editor
{
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

                EditorGUILayout.LabelField(toShow.Action.name);
                EditorGUILayout.LabelField(toShow.Entity.name);

                if (toShow.Ability != null)
                {
                    EditorGUILayout.LabelField("by " + toShow.Ability.name);
                }
                else if (toShow.Consumable != null)
                {
                    EditorGUILayout.LabelField("with a " + toShow.Consumable.name);
                }


                //if (GUILayout.Button("Edit"))
                //{
                //    // TODO
                //}

                if (GUILayout.Button("Delete"))
                {
                    gameplayToDelete.Add(toShow);
                }

                EditorGUILayout.EndHorizontal();
            }

            // Deleting all that needs to go
            foreach (Gameplay gameplay in gameplayToDelete)
            {
                //DataAccess.GetGameplayContainer().RemoveGameplay(gameplay);
                DataAccess.DeleteGameplay(gameplay);

            }


            if (GUILayout.Button("New Gameplay"))
            {
                GameplayCreator.ShowGameplayCreator();
            }

            AssetDatabase.SaveAssets();
        }
    }
}