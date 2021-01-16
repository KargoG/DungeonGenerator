using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;
using UnityEngine;

namespace DungeonGenerator
{
    [Serializable]
    public class GameplayElementContainer : ScriptableObject
    {
        [SerializeField] private List<GameplayElement> _actionContainer = new List<GameplayElement>();
        [SerializeField] private List<GameplayElement> _entityContainer = new List<GameplayElement>();
        [SerializeField] private List<GameplayElement> _abilityContainer = new List<GameplayElement>();
        [SerializeField] private List<GameplayElement> _consumableContainer = new List<GameplayElement>();

        void OnEnable()
        {
            if(_actionContainer == null)
                _actionContainer = new List<GameplayElement>();
            if (_entityContainer == null)
                _entityContainer = new List<GameplayElement>();
            if (_abilityContainer == null)
                _abilityContainer = new List<GameplayElement>();
            if (_consumableContainer == null)
                _consumableContainer = new List<GameplayElement>();
        }

        #region usingTemplate

        private int GetContainer<T>()
        {
            GameplayElementTypes gt;
            Enum.TryParse(typeof(T).ToString(), out gt);

            return (int) gt;
        }

        #endregion

        private List<GameplayElement> GetList(GameplayElementTypes elementType)
        {
            switch (elementType)
            {
                case GameplayElementTypes.Action:
                    return _actionContainer;
                case GameplayElementTypes.Entity:
                    return _entityContainer;
                case GameplayElementTypes.Ability:
                    return _abilityContainer;
                case GameplayElementTypes.Consumable:
                    return _consumableContainer;
            }

            return new List<GameplayElement>();
        }

        #region usingEnum

        public void AddElement(GameplayElement newElement, GameplayElementTypes elementType)
        {
            List<GameplayElement> list = GetList(elementType);

            if (!list.Contains(newElement))
            {
                list.Add(newElement);
            }
        }

        public GameplayElement GetElement(int elementNumber, GameplayElementTypes elementType)
        {
            List<GameplayElement> list = GetList(elementType);

            if (list.Count > elementNumber)
            {
                return list[elementNumber];
            }

            return null;
        }

        public void DeleteElement(GameplayElement toDelete, GameplayElementTypes elementType)
        {
            List<GameplayElement> list = GetList(elementType);
            if (list.Contains(toDelete))
            {
                list.Remove(toDelete);
            }
        }

        public List<GameplayElement> GetAllElements(GameplayElementTypes elementType)
        {
            return GetList(elementType);
        }

        #endregion

    }
}
