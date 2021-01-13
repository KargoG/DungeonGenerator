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
        [SerializeField] private UnityEvent<DungeonRoom> _roomCreator = new RoomCreator();
        private GameObject _roomPrefab = null;

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
            _roomPrefab = EditorGUILayout.ObjectField(_roomPrefab, typeof(GameObject), false) as GameObject;


            HandleEvents(Event.current);

            if (_graphs.Count > 0)
                ShowGraphEditing();
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


                toDraw.Position = new Vector2(0, i * (50 + 20));
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

            DataAccess.CreateRoomGraph(_newGraphName, _graphToConvert);

            ReloadGraphData();
        }

        private void CreateRoomsInScene()
        {
            if (_graphs.Count <= 0)
            {
                ErrorWindow.ShowWindow("Can not create rooms without having a room graph", null);
                return;
            }

            GameObject dungeonRoot = new GameObject("DungeonRoot");

            foreach (DungeonRoom dungeonRoom in _graphs[_shownGraph].DungeonGraph)
            {
                //_roomCreator.Invoke(dungeonRoom); TODO this shit just doesn't want to work

                Vector3 roomPosition =
                    new Vector3(dungeonRoom.Position.x, 0, dungeonRoom.Position.y) / 3; // TODO this value is hardcoded

                GameObject room = Instantiate(_roomPrefab, roomPosition, Quaternion.identity, dungeonRoot.transform);
                foreach (GameplayRepresentation gameplayInRoom in dungeonRoom.GameplayInRoom)
                {
                    gameplayInRoom.SpawnGameplay(room);
                }
            }
        }
    }
}
