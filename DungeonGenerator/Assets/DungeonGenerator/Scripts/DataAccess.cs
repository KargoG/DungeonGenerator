using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataAccess : ScriptableObject
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

    public static GameplayGraph CreateGameplayGraph(string name)
    {
        GameplayGraph newGraph = GameplayGraph.CreateGraph(name);
        AssetDatabase.CreateAsset(newGraph, "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/"+name+".asset");

        return newGraph;
    }

    private static GameplayGraph LoadGameplayGraph(string guid)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<GameplayGraph>(assetPath);
    }
    #endregion

    #region AccessData

    public static GameplayContainer GetGameplayContainer()
    {
        //string assetsGUID = AssetDatabase.AssetPathToGUID("Assets/DungeonGenerator/ScriptableObjects/Gameplay.asset");

        if (_gameplay == null)
        {

            _gameplay = AssetDatabase.LoadAssetAtPath<GameplayContainer>("Assets/DungeonGenerator/ScriptableObjects/Gameplay.asset");

            if (!_gameplay)
                CreateGameplayContainer();
        }

        return _gameplay;
    }

    public static GameplayElementContainer GetGameplayElementContainer()
    {
        string assetsGUID = AssetDatabase.AssetPathToGUID("Assets/DungeonGenerator/ScriptableObjects/GameplayElements.asset");


        if (_gameplayElements == null)
        {
            _gameplayElements = AssetDatabase.LoadAssetAtPath<GameplayElementContainer>("Assets/DungeonGenerator/ScriptableObjects/GameplayElements.asset");
            
            if(!_gameplayElements)
                CreateGameplayElementContainer();
        }

        return _gameplayElements;
    }

    public static List<GameplayGraph> GetGameplayGraphs()
    {
        List<GameplayGraph> graphs = new List<GameplayGraph>();

        string[] assetsGUID = AssetDatabase.FindAssets("t:GameplayGraph");

        foreach (string guid in assetsGUID)
        {
            graphs.Add(LoadGameplayGraph(guid));
        }

        return graphs;
    }

    #endregion

}
