﻿using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;
using DungeonGenerator.Editor;
using UnityEditor;
using UnityEditor.AppleTV;
using UnityEngine;
using UnityEngine.Events;

namespace DungeonGenerator.Editor
{
    public class RoomGraphWindow : EditorWindow
    {
        private List<RoomGraph> _graphs = new List<RoomGraph>();
        private List<string> _graphNames = new List<string>();
        private GameplayGraph _graphToConvert = null;
        private string _newGraphName = "";

        private GameObject _roomCreator = null;

        private RoomCreationSettings _roomSettings = null;

        [MenuItem("Window/DungeonCreator/RoomGraphEditor")]
        public static void ShowWindow()
        {
            GetWindow<RoomGraphWindow>();
        }

        private void OnFocus()
        {
            ReloadGraphData();
        }

        void ReloadGraphData()
        {
            _graphs = DataAccess.GetRoomGraphs();
            _graphNames = new List<string>();
            foreach (RoomGraph graph in _graphs)
            {
                _graphNames.Add(graph.Name);
            }
        }

        private int _shownGraph = 0;

        private void OnGUI()
        {
            _shownGraph = EditorGUILayout.Popup(_shownGraph, _graphNames.ToArray());
            _newGraphName = EditorGUILayout.TextField("New Graph name", _newGraphName);
            _graphToConvert =
                EditorGUILayout.ObjectField(_graphToConvert, typeof(GameplayGraph), false) as GameplayGraph;
            _roomCreator = EditorGUILayout.ObjectField(_roomCreator, typeof(GameObject), false) as GameObject;
            _roomSettings = EditorGUILayout.ObjectField(_roomSettings, typeof(RoomCreationSettings), false) as RoomCreationSettings;


            HandleEvents(Event.current);

            if (_graphs.Count > 0)
            {
                if (GUILayout.Button("RunReplacementPass"))
                {
                    RunPass();
                }

                ShowGraphEditing();
            }
        }

        private void RunPass()
        {
            if (!_roomSettings)
            {
                ErrorWindow.ShowWindow("You can not run passes without selecting room settings", null);
                return;
            }


            RoomGraph graphToChange = _graphs[_shownGraph];

            foreach (KeyValuePair<Tuple<Gameplay, Gameplay>, float> connectable in _roomSettings.Connectables)
            {
                graphToChange.ApplyMerges(connectable.Key, connectable.Value);
            }
        }

        private void ShowGraphEditing()
        {
            EditorGUILayout.Space();

            Vector2 lastPos = GUILayoutUtility.GetLastRect().position;
            GUI.BeginGroup(new Rect(0, lastPos.y + GUILayoutUtility.GetLastRect().height, Screen.width,
                Screen.height - lastPos.y - GUILayoutUtility.GetLastRect().height));



            for (int i = 0; i < _graphs[_shownGraph].DungeonGraph.Count; i++)
            {
                DungeonRoom toDraw = _graphs[_shownGraph].DungeonGraph[i];

                toDraw.DrawConnections(new Vector2(150, 50));
                toDraw.DrawNode(new Vector2(150, 50));
            }

            GUI.EndGroup();

            Repaint();
        }

        void HandleEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                        OpenMenu();
                    break;
            }

        }

        private void OpenMenu()
        {
            GenericMenu nodeMenu = new GenericMenu();

            nodeMenu.AddItem(new GUIContent("CreateRoomGraph"), false, CreateRoomGraph);

            if (_graphs.Count > 0)
            {
                nodeMenu.AddItem(new GUIContent("Create rooms in Scene"), false, CreateRoomsInScene);
            }
            else
            {
                nodeMenu.AddDisabledItem(new GUIContent("Create rooms in Scene"));
            }

            nodeMenu.ShowAsContext();
        }

        private void CreateRoomGraph()
        {
            if (!_graphToConvert)
            {
                ErrorWindow.ShowWindow("You can not create a Room graph without selecting a Gameplay Graph", null);
                return;
            }

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

            RoomGraph newGraph = RoomGraph.CreateStartingGraph(_newGraphName, _graphToConvert);


            // Give some basic positions
            for (int i = 0; i < newGraph.DungeonGraph.Count; i++)
            {
                DungeonRoom toDraw = newGraph.DungeonGraph[i];

                toDraw.Position = new Vector2(0, i * (50 + 20));
            }

            ReloadGraphData();
        }

        private void CreateRoomsInScene()
        {
            if (_graphs.Count <= 0)
            {
                ErrorWindow.ShowWindow("Can not create rooms without having a room graph", null);
                return;
            }

            if (_roomCreator.GetComponent<IRoomCreator>() == null)
            {
                ErrorWindow.ShowWindow("The selected room creator has no Script that inherits from IRoomCreator.", null);
                return;
            }

            Dictionary<DungeonRoom, GameObject> roomInstancePairs =
                new Dictionary<DungeonRoom, GameObject>();

            // Creating the rooms
            foreach (DungeonRoom dungeonRoom in _graphs[_shownGraph].DungeonGraph)
            {
                dungeonRoom.PrepareConnections();

                Vector3 roomPosition =
                    new Vector3(dungeonRoom.Position.x, 0, dungeonRoom.Position.y) / 3; // TODO this value is hardcoded

                GameObject roomRoot = Instantiate(_roomCreator, roomPosition, Quaternion.identity);
                IRoomCreator roomCreator = roomRoot.GetComponent<IRoomCreator>();
                roomCreator.CreateRoom(dungeonRoom);

                roomInstancePairs.Add(dungeonRoom, roomRoot);
            }

            List<GameObject[]> connections = new List<GameObject[]>();

            // Set up connections and gameplay
            foreach (DungeonRoom dungeonRoom in _graphs[_shownGraph].DungeonGraph)
            {
                connections.Add(new GameObject[dungeonRoom.NextRoom.Count + dungeonRoom.PreviousRoom.Count]);
                for (int i = 0; i < dungeonRoom.NextRoom.Count; i++)
                {
                    connections[connections.Count - 1][i] = roomInstancePairs[dungeonRoom.NextRoom[i]];
                }
                for (int i = 0; i < dungeonRoom.PreviousRoom.Count; i++)
                {
                    connections[connections.Count - 1][dungeonRoom.NextRoom.Count + i] = roomInstancePairs[dungeonRoom.PreviousRoom[i]];
                }

                roomInstancePairs[dungeonRoom].GetComponent<IRoomCreator>().SetUpConnections(connections[connections.Count - 1]);

                foreach (GameplayRepresentation gameplayInRoom in dungeonRoom.GameplayInRoom)
                {
                    gameplayInRoom.SpawnGameplay(roomInstancePairs[dungeonRoom]);
                }
            }
        }
    }
}
