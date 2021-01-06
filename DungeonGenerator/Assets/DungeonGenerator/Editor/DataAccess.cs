using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataAccess : MonoBehaviour
{
    private static GameplayElementContainer _gameplayElements;
    private static GameplayContainer _gameplay;

    #region LoadingData
    private static void CreateGameplayContainer()
    {
        _gameplay = ScriptableObject.CreateInstance<GameplayContainer>();
        AssetDatabase.CreateAsset(_gameplay, "Assets/DungeonGenerator/ScriptableObjects/Gameplay.asset");
    }

    private static void LoadGameplayContainer(string guid)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        _gameplay = AssetDatabase.LoadAssetAtPath<GameplayContainer>(assetPath);
    }
    private static void CreateGameplayElementContainer()
    {
        _gameplayElements = ScriptableObject.CreateInstance<GameplayElementContainer>();
        AssetDatabase.CreateAsset(_gameplayElements, "Assets/DungeonGenerator/ScriptableObjects/GameplayElements.asset");
    }

    private static void LoadGameplayElementContainer(string guid)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        _gameplayElements = AssetDatabase.LoadAssetAtPath<GameplayElementContainer>(assetPath);
    }
    #endregion

    #region AccessData

    public static GameplayContainer GetGameplayContainer()
    {
        //string[] assetsGUID = AssetDatabase.FindAssets("t:GameplayContainer");
        string assetsGUID = AssetDatabase.AssetPathToGUID("Assets/DungeonGenerator/ScriptableObjects/Gameplay.asset");

        if (_gameplay == null)
        {
            if (assetsGUID.Length > 0)
                LoadGameplayContainer(assetsGUID);
            else
                CreateGameplayContainer();
        }

        return _gameplay;
    }

    public static GameplayElementContainer GetGameplayElementContainer()
    {
        string assetsGUID = AssetDatabase.AssetPathToGUID("Assets/DungeonGenerator/ScriptableObjects/GameplayElements.asset");

        //print(assetsGUID);

        if (_gameplayElements == null)
        {
            _gameplayElements = AssetDatabase.LoadAssetAtPath<GameplayElementContainer>("Assets/DungeonGenerator/ScriptableObjects/GameplayElements.asset");
            //LoadGameplayElementContainer(assetsGUID);
            if(!_gameplayElements)
                CreateGameplayElementContainer();
        }

        return _gameplayElements;
    }

    #endregion
}
