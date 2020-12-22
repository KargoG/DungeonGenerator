using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayElementContainer : ScriptableObject
{
    private List<GameplayElement>[] gameplayElementContainer = new List<GameplayElement>[]
    {
        new List<GameplayElement>(), // Action
        new List<GameplayElement>(), // Entity
        new List<GameplayElement>(), // Ability
        new List<GameplayElement>() // Consumable
    };

    //private List<Action> _actions = new List<Action>();
    //private List<Entity> _entities = new List<Entity>();
    //private List<Ability> _abilities = new List<Ability>();
    //private List<Consumable> _consumables = new List<Consumable>();


#region usingTemplate
    //public void AddElement<T>(T newElement) where T : GameplayElement
    //{
    //    List<T> list = gameplayElementContainer[GetContainer<T>()] as List<T>;

    //    if (!list.Contains(newElement))
    //    {
    //        list.Add(newElement);
    //    }
    //}
    //public T GetElement<T>(int elementNumber) where T : GameplayElement
    //{
    //    List<T> list = gameplayElementContainer[GetContainer<T>()] as List<T>;

    //    if (list.Count > elementNumber)
    //    {
    //        return list[elementNumber];
    //    }

    //    return null;
    //}
    //public void DeleteElement<T>(T toDelete) where T : GameplayElement
    //{
    //    List<T> list = gameplayElementContainer[GetContainer<T>()] as List<T>;
    //    if (list.Contains(toDelete))
    //    {
    //        list.Remove(toDelete);
    //    }
    //}
    //public List<T> GetAllElements<T>() where T : GameplayElement
    //{
    //    return gameplayElementContainer[GetContainer<T>()] as List<T>;
    //}g
    private int GetContainer<T>()
    {
        GameplayElementTypes gt;
        Enum.TryParse(typeof(T).ToString(), out gt);

        return (int) gt;
    }
#endregion

#region usingEnum
    public void AddElement(GameplayElement newElement, GameplayElementTypes elementType)
    {
        List<GameplayElement> list = gameplayElementContainer[(int)elementType];

        if (!list.Contains(newElement))
        {
            list.Add(newElement);
        }
    }
    public GameplayElement GetElement(int elementNumber, GameplayElementTypes elementType)
    {
        List<GameplayElement> list = gameplayElementContainer[(int)elementType];

        if (list.Count > elementNumber)
        {
            return list[elementNumber];
        }

        return null;
    }
    public void DeleteElement(GameplayElement toDelete, GameplayElementTypes elementType)
    {
        List<GameplayElement> list = gameplayElementContainer[(int)elementType];
        if (list.Contains(toDelete))
        {
            list.Remove(toDelete);
        }
    }
    public List<GameplayElement> GetAllElements(GameplayElementTypes elementType)
    {
        return gameplayElementContainer[(int)elementType];
    }
#endregion

}
