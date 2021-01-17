using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace DungeonGenerator.Editor
{
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
            _settings =
                EditorGUILayout.ObjectField(_settings, typeof(GameplayGraphSettings), false) as GameplayGraphSettings;

            Event e = Event.current;
            Vector2 originalMousePos = e.mousePosition;
            e.mousePosition -= new Vector2(0, GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect().height);
            HandleEvents(e);
            e.mousePosition = originalMousePos;

            if (GUILayout.Button("Save Stuff"))
            {
                EditorUtility.SetDirty(_graphs[_shownGraph]);
                AssetDatabase.SaveAssets();
            }

            if (_graphs.Count > 0)
            {
                if (GUILayout.Button("RunReplacementPass"))
                {
                    RunPass();
                }

                ShowGraphEditing();
            }
        }

        private void ShowGraphEditing()
        {
            EditorGUILayout.Space();

            Vector2 lastPos = GUILayoutUtility.GetLastRect().position;
            GUI.BeginGroup(new Rect(0, lastPos.y + GUILayoutUtility.GetLastRect().height, Screen.width,
                Screen.height - lastPos.y - GUILayoutUtility.GetLastRect().height));

            for (int i = 0; i < _graphs[_shownGraph].GameplayInGraph.Count; i++)
            {
                GameplayRepresentation toDraw = _graphs[_shownGraph].GameplayInGraph[i];

                toDraw.DrawConnections(new Vector2());
                toDraw.DrawNode(new Vector2());
            }


            GUI.EndGroup();
        }

        void HandleEvents(Event e)
        {

            bool repaintNeeded = false;

            if (_graphs.Count > 0)
            {
                foreach (GameplayRepresentation gameplayNode in _graphs[_shownGraph].GameplayInGraph)
                {
                    if (gameplayNode.HandleInput(e))
                        repaintNeeded = true;
                }
            }

            if (repaintNeeded)
                Repaint();

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                        OpenMenu();
                    break;
            }

            if (_graphs.Count <= 0)
                return;

            foreach (GameplayRepresentation gameplayNode in _graphs[_shownGraph].GameplayInGraph)
            {
                gameplayNode.HandleInput(e);
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

            GameplayGraphManager.CreateStartGraph(_newGraphName, _settings ? _settings : CreateInstance<GameplayGraphSettings>());
            ReloadGraphData();
        }

        void RunPass()
        {
            GameplayGraph graphToChange = _graphs[_shownGraph];

            List<GameplayGraph> replacableGraphs = DataAccess.GetGameplaySubgraphConnections().GetReplacableGraphs();

            foreach (GameplayGraph replacableGraph in replacableGraphs)
            {
                foreach (GameplayGraph replacement in DataAccess.GetGameplaySubgraphConnections().GetConnections(replacableGraph))
                {
                    graphToChange.InsertSubgraph(replacableGraph, replacement);
                }
            }
        }
    }
}