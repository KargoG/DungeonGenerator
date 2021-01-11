using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//public class Node
//{
//    [SerializeField] private Vector2 _position;
//    [SerializeField] private GameplayRepresentation _gameplay;
//    [SerializeField] private List<GameplayRepresentation> _nextGameplay = new List<GameplayRepresentation>();

//    public Node(Vector2 pos, GameplayRepresentation gameplay)
//    {

//    }

//    public void Draw()
//    {

//    }
//}

public class GameplayGraphWindow : EditorWindow
{
    private List<GameplayGraph> _graphs = new List<GameplayGraph>();
    //private List<GameplayRepresentation> _graphNodes = new List<GameplayRepresentation>();
    private List<string> _graphNames = new List<string>();
    private GameplayGraphSettings _settings = null;
    private string _newGraphName = "";

    [MenuItem("Window/DungeonCreator/GameplayGraphEditor")]
    public static void ShowWindow()
    {
        GetWindow<GameplayGraphWindow>();
    }

    private void OnFocus()
    {
        ReloadGraphData();
    }

    void ReloadGraphData()
    {
        _graphs = DataAccess.GetGameplayGraphs();
        _graphNames = new List<string>();
        foreach (GameplayGraph graph in _graphs)
        {
            _graphNames.Add(graph.Name);
        }
    }

    private int _shownGraph = 0;

    private void OnGUI()
    {
        _shownGraph = EditorGUILayout.Popup(_shownGraph, _graphNames.ToArray());
        _newGraphName = EditorGUILayout.TextField("New Graph name", _newGraphName);
        _settings = EditorGUILayout.ObjectField(_settings, typeof(GameplayGraphSettings), false) as GameplayGraphSettings;

        HandleEvents(Event.current);

        if (_graphs.Count > 0)
            ShowGraphEditing();
    }

    private void ShowGraphEditing()
    {
        EditorGUILayout.Space();

        Vector2 lastPos = GUILayoutUtility.GetLastRect().position;
        GUI.BeginGroup(new Rect(0, lastPos.y + GUILayoutUtility.GetLastRect().height, Screen.width, Screen.height - lastPos.y - GUILayoutUtility.GetLastRect().height));

        for (int i = 0; i < _graphs[_shownGraph].GameplayInGraph.Count; i++)
        {
            GameplayRepresentation toDraw = _graphs[_shownGraph].GameplayInGraph[i];


            toDraw.Position = new Vector2(0, i * (50 + 20));
            toDraw.DrawConnections(new Vector2(150, 50));
            toDraw.DrawNode(new Vector2(150, 50));
        }


        GUI.EndGroup();
        //foreach (GameplayRepresentation gameplay in _graphs[_shownGraph].GameplayInGraph)
        //{
        //    gameplay.Position 
        //    gameplay.DrawNode(new Vector2(150, 50));
        //}

        Repaint();
    }

    void HandleEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if(e.button == 1)
                    OpenMenu();
                break;
        }

    }

    private void OpenMenu()
    {
        GenericMenu nodeMenu = new GenericMenu();
        
        nodeMenu.AddItem(new GUIContent("CreateStartGraph"), false, CreateStartGraph);

        nodeMenu.ShowAsContext();
    }

    private void CreateStartGraph()
    {
        if (_newGraphName.Length <= 0)
        {
            ErrorWindow.ShowWindow("You can not create a graph without a name", null);
            return;
        }

        if (_graphNames.Contains(_newGraphName))
        {
            ErrorWindow.ShowWindow("You can not create a graph with an already existing name", null);
            return;
        }

        GameplayGraphManager.CreateStartGraph(_newGraphName, _settings ? _settings : new GameplayGraphSettings());
        ReloadGraphData();
    }

    //private string _graphName = "";
    //private void ShowGraphCreation()
    //{
    //    EditorGUILayout.Space();

    //    _graphName = EditorGUILayout.TextField("Graph Name: ", _graphName);

    //    if (GUILayout.Button("Create"))
    //    {
    //        if (_graphName.Length == 0)
    //        {
    //            ErrorWindow.ShowWindow("Please enter a name for the graph!", null);
    //        }
    //        else if (_graphOptions.Contains(_graphName))
    //        {
    //            ErrorWindow.ShowWindow("A graph with this name already exists!", null);
    //        }
    //        else
    //        {
    //            GameplayGraph newGraph = DataAccess.CreateGameplayGraph(_graphName);
    //            _graphs.Add(newGraph);
    //            _graphOptions[_graphOptions.Count - 1] = newGraph.Name;
    //            _graphOptions.Add("new Graph");
    //            _graphName = "";
    //        }
    //    }
    //}
}
