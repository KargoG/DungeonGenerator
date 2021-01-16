﻿using System.Collections.Generic;
using System.Linq;
using DungeonGenerator;
using DungeonGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace DungeonGenerator.Editor
{
    public class SubgraphCreator : EditorWindow
    {
        private List<GameplayGraph> _graphs = new List<GameplayGraph>();
        private List<string> _graphNames = new List<string>();
        private string _newGraphName = "";

        [MenuItem("Window/DungeonCreator/SubgraphCreator")]
        public static void ShowWindow()
        {
            GetWindow<SubgraphCreator>();
        }

        void OnFocus()
        {
            ReloadGraphData();
        }

        void ReloadGraphData()
        {
            _graphs = DataAccess.GetGameplaySubGraphs();
            _graphNames = new List<string>();
            foreach (GameplayGraph graph in _graphs)
            {
                _graphNames.Add(graph.Name);
            }
        }

        private GameplayRepresentation _connectingNode = null;
        private int _shownGraph;

        void OnGUI()
        {
            _shownGraph = EditorGUILayout.Popup(_shownGraph, _graphNames.ToArray());
            _newGraphName = EditorGUILayout.TextField("New Graph name", _newGraphName);

            if (GUILayout.Button("Create Gameplay"))
            {
                CreateNewSubGraph();
            }

            if (_graphs.Count > 0)
                ShowGraphEditing();

            HandleEvents(Event.current);

        }

        void CreateNewSubGraph()
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


            DataAccess.CreateGameplaySubGraph(_newGraphName);

            ReloadGraphData();
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

            if (_connectingNode != null)
            {
                Handles.DrawLine(_connectingNode.Position, Event.current.mousePosition);
                Repaint();
            }

            GUI.EndGroup();
        }

        void HandleEvents(Event e)
        {
            foreach (GameplayRepresentation gameplayNode in _graphs[_shownGraph].GameplayInGraph)
            {
                gameplayNode.HandleInput(e);
            }


            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (_connectingNode != null)
                        {
                            foreach (GameplayRepresentation gameplayRepresentation in _graphs[_shownGraph]
                                .GameplayInGraph)
                            {
                                if (gameplayRepresentation.IsInRect(e.mousePosition))
                                {
                                    if (_connectingNode == gameplayRepresentation)
                                        continue;

                                    if (_connectingNode.NextGameplay.Contains(gameplayRepresentation))
                                        break;
                                    _connectingNode.NextGameplay.Add(gameplayRepresentation);

                                    break;
                                }
                            }

                            _connectingNode = null;

                            break;
                        }
                    }

                    if (e.button == 1)
                    {
                        bool shouldBreak = false;
                        foreach (GameplayRepresentation gameplayRepresentation in _graphs[_shownGraph].GameplayInGraph)
                        {
                            if (gameplayRepresentation.IsInRect(e.mousePosition))
                            {
                                OpenNodeEditMenu(gameplayRepresentation);
                                shouldBreak = true;
                                break;
                            }
                        }

                        if (shouldBreak)
                            break;


                        OpenGraphEditMenu();
                    }

                    break;
            }

            if (_graphs.Count <= 0)
                return;
        }

        private void OpenNodeEditMenu(GameplayRepresentation node)
        {
            GenericMenu nodeMenu = new GenericMenu();

            bool isStartingGameplay = _graphs[_shownGraph].StartingGameplay.Contains(node);
            bool isEndGameplay = _graphs[_shownGraph].EndGameplay.Contains(node);

            nodeMenu.AddItem(new GUIContent("Make this a starting Node"), isStartingGameplay,
                ChangeStartingGameplay, node);

            nodeMenu.AddItem(new GUIContent("Make this an end Node"), isEndGameplay,
                ChangeEndGameplay, node);

            nodeMenu.AddItem(new GUIContent("Add Connection"), false,
                (object gameplay) => { _connectingNode = gameplay as GameplayRepresentation; }, node);

            nodeMenu.AddItem(new GUIContent("Remove Node"), false, RemoveGameplay, node);

            nodeMenu.ShowAsContext();
        }

        void ChangeStartingGameplay(object gameplay)
        {
            GameplayRepresentation gp = gameplay as GameplayRepresentation;

            _graphs[_shownGraph].ChangeStartingGameplay(gp);
            EditorUtility.SetDirty(_graphs[_shownGraph]);
            AssetDatabase.SaveAssets();
        }

        void ChangeEndGameplay(object gameplay)
        {
            GameplayRepresentation gp = gameplay as GameplayRepresentation;

            _graphs[_shownGraph].ChangeEndGameplay(gp);
            EditorUtility.SetDirty(_graphs[_shownGraph]);
            AssetDatabase.SaveAssets();
        }

        void RemoveGameplay(object gameplay)
        {
            GameplayRepresentation gp = gameplay as GameplayRepresentation;

            foreach (GameplayRepresentation otherGameplay in _graphs[_shownGraph].GameplayInGraph)
            {
                if (otherGameplay.NextGameplay.Contains(gp))
                    otherGameplay.NextGameplay.Remove(gp);
            }

            _graphs[_shownGraph].RemoveGameplay(gp);
            EditorUtility.SetDirty(_graphs[_shownGraph]);
            AssetDatabase.SaveAssets();
        }

        private void OpenGraphEditMenu()
        {
            GenericMenu nodeMenu = new GenericMenu();

            string[] availableGameplay = DataAccess.GetGameplayContainer().GetGameplayNames();

            for (int i = 0; i < availableGameplay.Length; i++)
            {
                if (_graphs.Count > 0)
                    nodeMenu.AddItem(new GUIContent("AddGameplayNode/" + availableGameplay[i]), false,
                        (object gameplay) =>
                        {
                            _graphs[_shownGraph].AddGameplay(gameplay as Gameplay);
                            EditorUtility.SetDirty(_graphs[_shownGraph]);
                            AssetDatabase.SaveAssets();

                        }, DataAccess.GetGameplayContainer().GetGameplay(i));
                else
                    nodeMenu.AddDisabledItem(new GUIContent("AddGameplayNode/" + availableGameplay[i]), false);
            }

            nodeMenu.ShowAsContext();
        }
    }
}
