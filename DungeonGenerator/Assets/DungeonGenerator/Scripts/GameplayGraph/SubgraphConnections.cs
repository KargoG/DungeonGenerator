using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace DungeonGenerator
{
    [Serializable]
    public class SubgraphConnections : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private Dictionary<GameplayGraph, List<GameplayGraph>> _connections;

        [SerializeField] private List<GameplayGraph> _keys = new List<GameplayGraph>();
        [SerializeField] private List<int> _valuesPerKey = new List<int>();
        [SerializeField] private List<GameplayGraph> _values = new List<GameplayGraph>();

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _valuesPerKey.Clear();
            _values.Clear();

            foreach (KeyValuePair<GameplayGraph, List<GameplayGraph>> pair in _connections)
            {
                _keys.Add(pair.Key);
                _valuesPerKey.Add(pair.Value.Count);
                foreach (GameplayGraph value in pair.Value)
                {
                    _values.Add(value);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (_connections == null)
                _connections = new Dictionary<GameplayGraph, List<GameplayGraph>>();
            _connections.Clear();

            int nextValue = 0;
            for (int i = 0; i < _keys.Count; i++)
            {
                List<GameplayGraph> values = new List<GameplayGraph>();

                for (int j = 0; j < _valuesPerKey[i]; j++)
                {
                    values.Add(_values[nextValue++]);
                }

                _connections.Add(_keys[i], values);
            }
        }

        void OnEnable()
        {
            if (_connections == null)
            {
                _connections = new Dictionary<GameplayGraph, List<GameplayGraph>>();
            }
        }

        public void AddConnection(GameplayGraph from, GameplayGraph to)
        {
            if (!_connections.ContainsKey(from))
            {
                _connections.Add(from, new List<GameplayGraph>());
            }

            if(!_connections[from].Contains(to))
                _connections[from].Add(to);
        }

        public void RemoveConnection(GameplayGraph from, GameplayGraph to)
        {
            if (_connections[from].Contains(to))
                _connections[from].Remove(to);
        }

        public List<GameplayGraph> GetConnections(GameplayGraph from)
        {
            if(_connections.ContainsKey(from))
                return _connections[from];

            return null;
        }

        public List<GameplayGraph> GetReplacableGraphs()
        {
            List<GameplayGraph> graphs = new List<GameplayGraph>();

            foreach (KeyValuePair<GameplayGraph, List<GameplayGraph>> connection in _connections)
            {
                graphs.Add(connection.Key);
            }

            return graphs;
        }
    }
}
