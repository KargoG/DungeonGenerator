using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;
using DungeonGenerator.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SubgraphConnector : EditorWindow
{
    [MenuItem("Window/DungeonCreator/SubgraphConnector")]
    public static void ShowWindow()
    {
        GetWindow<SubgraphConnector>();
    }

    private List<GameplayGraph> _allGraphs;
    private List<string> _graphNames = new List<string>();

    private void OnFocus()
    {
        ReloadGraphData();
    }

    void ReloadGraphData()
    {
        _allGraphs = DataAccess.GetGameplaySubGraphs();
        _graphNames = new List<string>();
        foreach (GameplayGraph graph in _allGraphs)
        {
            _graphNames.Add(graph.Name);
        }
    }


    Vector2 _leftScreenOffset = Vector2.zero;
    Vector2 _rightScreenOffset = Vector2.zero;
    private int _graphOnLeftSide;
    private int _graphOnRightSide;
    // Update is called once per frame
    void OnGUI()
    {
        Handles.color = Color.black;
        Handles.DrawLine(new Vector3(Screen.width / 2, 0), new Vector3(Screen.width / 2, Screen.height - 50) );
        Handles.color = Color.black;
        Handles.DrawLine(new Vector3(0, Screen.height - 50), new Vector3(Screen.width, Screen.height - 50));

        GUIStyle style = new GUIStyle();
        style.fixedHeight = Screen.height - 50;



        EditorGUILayout.Space();

        Vector2 lastPos = GUILayoutUtility.GetLastRect().position;
        //GUI.BeginGroup(new Rect(0, lastPos.y + GUILayoutUtility.GetLastRect().height, Screen.width,
        //    Screen.height - lastPos.y - GUILayoutUtility.GetLastRect().height));


        Rect lastRect = GUILayoutUtility.GetLastRect();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(style);

        _graphOnLeftSide = EditorGUILayout.Popup(_graphOnLeftSide, _graphNames.ToArray());

        Rect drawContainer = new Rect(0, lastPos.y + lastRect.height, Screen.width / 2,
            Screen.height - lastPos.y - lastRect.height);
        GUI.BeginGroup(drawContainer);
        ShowSide(_graphOnLeftSide, true);

        if(drawContainer.Contains(Event.current.mousePosition))
            HandleEventsInRect(Event.current, true);

        GUI.EndGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(style);
        _graphOnRightSide = EditorGUILayout.Popup(_graphOnRightSide, _graphNames.ToArray());


        GUI.BeginGroup(new Rect(Screen.width / 2, lastPos.y + lastRect.height, Screen.width / 2,
            Screen.height - lastPos.y - lastRect.height));

        ShowSide(_graphOnRightSide, false);

        if (drawContainer.Contains(Event.current.mousePosition))
            HandleEventsInRect(Event.current, false);

        GUI.EndGroup();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (_graphOnLeftSide == _graphOnRightSide)
        {
            GUILayout.Label("A graph can not connect to itself");
            return;
        }

        SubgraphConnections connections = DataAccess.GetGameplaySubgraphConnections();
        List<GameplayGraph> existingConnections = connections.GetConnections(_allGraphs[_graphOnLeftSide]);
        bool existingConnection = existingConnections?.Contains(_allGraphs[_graphOnRightSide]) ?? false;

        if (existingConnection)
        {
            if (GUILayout.Button("Remove Connection"))
            {
                connections.RemoveConnection(_allGraphs[_graphOnLeftSide], _allGraphs[_graphOnRightSide]);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            if (GUILayout.Button("Add Connection"))
            {
                connections.AddConnection(_allGraphs[_graphOnLeftSide], _allGraphs[_graphOnRightSide]);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

    }

    private void HandleEventsInRect(Event e, bool isLeftRect)
    {
        switch (e.type)
        {
            case EventType.MouseDrag:
                if (e.button == 2)
                {
                    if (isLeftRect)
                        _leftScreenOffset += e.delta;
                    else
                        _rightScreenOffset += e.delta;

                    Repaint();
                }
                break;
        }
    }

    void ShowSide(int graphNumber, bool leftSide)
    {
        if (_allGraphs.Count == 0)
            return;

        for (int i = 0; i < _allGraphs[graphNumber].GameplayInGraph.Count; i++)
        {
            GameplayRepresentation toDraw = _allGraphs[graphNumber].GameplayInGraph[i];

            toDraw.DrawConnections(leftSide ? _leftScreenOffset : _rightScreenOffset);
            toDraw.DrawNode(leftSide ? _leftScreenOffset : _rightScreenOffset);
        }
    }
}
