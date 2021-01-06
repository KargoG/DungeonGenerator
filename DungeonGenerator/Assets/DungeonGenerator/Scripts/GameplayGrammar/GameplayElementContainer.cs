using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameplayElementContainer : ScriptableObject
{
    [SerializeField]
    private List<GameplayElement> _actionContainer = new List<GameplayElement>();
    [SerializeField]
    private List<GameplayElement> _entityContainer = new List<GameplayElement>();
    [SerializeField]
    private List<GameplayElement> _abilityContainer = new List<GameplayElement>();
    [SerializeField]
    private List<GameplayElement> _consumableContainer = new List<GameplayElement>();

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

private List<GameplayElement> GetList(GameplayElementTypes elementType)
{
    switch (elementType)
    {
        case GameplayElementTypes.Action:
            return _actionContainer;
            break;
        case GameplayElementTypes.Entity:
            return _entityContainer;
            break;
        case GameplayElementTypes.Ability:
            return _abilityContainer;
            break;
        case GameplayElementTypes.Consumable:
            return _consumableContainer;
            break;
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
