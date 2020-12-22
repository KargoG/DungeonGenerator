using UnityEngine;
using UnityEditor;

public class GrammarEditor : EditorWindow
{
    private static GameplayElementContainer _gameplayElements;
    private static GameplayContainer _gameplay;


    [MenuItem("Window/DungeonCreator/GrammarEditor")]
    public static void ShowWindow()
    {
        string[] assetsGUID = AssetDatabase.FindAssets("t:GameplayElementContainer");

        if (_gameplayElements == null)
        {
            if (assetsGUID.Length > 0)
                LoadGameplayElementContainer(assetsGUID[0]);
            else
                CreateGameplayElementContainer();
        }
        
        assetsGUID = AssetDatabase.FindAssets("t:GameplayContainer");

        if (_gameplay == null)
        {
            if (assetsGUID.Length > 0)
                LoadGameplayContainer(assetsGUID[0]);
            else
                CreateGameplayContainer();
        }

        GetWindow<GrammarEditor>();
    }

    #region LoadingData
    private static void CreateGameplayContainer()
    {
        _gameplay = ScriptableObject.CreateInstance<GameplayContainer>();
        AssetDatabase.CreateAsset(_gameplay, "Assets/ScriptableObjects/Gameplay.asset");
    }

    private static void LoadGameplayContainer(string guid)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        _gameplay = AssetDatabase.LoadAssetAtPath<GameplayContainer>(assetPath);
    }
    private static void CreateGameplayElementContainer()
    {
        _gameplayElements = ScriptableObject.CreateInstance<GameplayElementContainer>();
        AssetDatabase.CreateAsset(_gameplayElements, "Assets/ScriptableObjects/GameplayElements.asset");
    }

    private static void LoadGameplayElementContainer(string guid)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        _gameplayElements = AssetDatabase.LoadAssetAtPath<GameplayElementContainer>(assetPath);
    }
    #endregion

    void OnGUI()
    {
        // Window Code

        EditorGUILayout.LabelField("Defined Gameplay:");

        for (int i = 0; i < _gameplay.GetAmountOfGameplay(); i++)
        {
            Gameplay toShow = _gameplay.GetGameplay(i);

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

            GUIStyle buttonStyle = GUIStyle.none;
            buttonStyle.alignment = TextAnchor.MiddleRight;

            if (GUILayout.Button("Edit", buttonStyle))
            {
                // TODO
            }

            if (GUILayout.Button("Delete", buttonStyle))
            {
                // TODO
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("New Gameplay"))
        {
            GameplayCreator.ShowGameplayCreator(_gameplayElements);
        }

    }
}
