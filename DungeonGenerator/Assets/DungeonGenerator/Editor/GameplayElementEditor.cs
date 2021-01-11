using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameplayElementEditor : EditorWindow
{
    [MenuItem("Window/DungeonCreator/GameplayElementEditor")]
    public static void ShowWindow()
    {
        GetWindow<GameplayElementEditor>();
    }

    private GameplayElementTypes _currentlyEditedType = GameplayElementTypes.Entity;

    private string[] _editableGameplayStringRepresentation = null;
    private int _selectedElement = 0;
    private string _newEntityName = "";

    void OnGUI()
    {

        GameplayElementTypes lastEditedType = _currentlyEditedType;
        _currentlyEditedType = (GameplayElementTypes) EditorGUILayout.EnumPopup("Gamplay Element", _currentlyEditedType);

        if (lastEditedType != _currentlyEditedType)
        {
            _selectedElement = 0;
            _editableGameplayStringRepresentation = null;
        }


        if (_editableGameplayStringRepresentation == null)
            _editableGameplayStringRepresentation = new string[DataAccess.GetGameplayElementContainer().GetAllElements(_currentlyEditedType).Count + 1];

        for (int i = 0; i < DataAccess.GetGameplayElementContainer().GetAllElements(_currentlyEditedType).Count; i++)
        {
            _editableGameplayStringRepresentation[i] = DataAccess.GetGameplayElementContainer().GetElement(i, _currentlyEditedType).Name;
        }


        _editableGameplayStringRepresentation[_editableGameplayStringRepresentation.Length - 1] = "Create New";

        _selectedElement = EditorGUILayout.Popup("Object to Edit", _selectedElement, _editableGameplayStringRepresentation);

        EditorGUILayout.Space();

        if (_selectedElement != _editableGameplayStringRepresentation.Length - 1)
            ShowEditingElements();
        else
        {
            ShowCreationElements();
        }
    }

    #region Creation

    void ShowCreationElements()
    {
        ShowGeneralCreationTop();

        switch (_currentlyEditedType)
        {
            case GameplayElementTypes.Action:
                ShowActionCreation();
                break;
            case GameplayElementTypes.Entity:
                ShowEntityCreation();
                break;
            case GameplayElementTypes.Ability:
                ShowAbilityCreation();
                break;
            case GameplayElementTypes.Consumable:
                ShowConsumableCreation();
                break;
        }

        ShowGeneralCreationEnd();
    }
    void ShowGeneralCreationTop()
    {
        _newEntityName = EditorGUILayout.TextField("Name: ", _newEntityName);
    }
    void ShowActionCreation()
    {

    }
    void ShowEntityCreation()
    {

    }
    void ShowAbilityCreation()
    {

    }
    void ShowConsumableCreation()
    {

    }
    void ShowGeneralCreationEnd()
    {
        if (GUILayout.Button("Create") && _newEntityName.Length > 0 && !_editableGameplayStringRepresentation.Contains(_newEntityName))
        {
            switch (_currentlyEditedType)
            {
                case GameplayElementTypes.Action:
                    DataAccess.CreateAction(new Action(_newEntityName));
                    break;
                case GameplayElementTypes.Entity:
                    DataAccess.CreateEntity(new Entity(_newEntityName));
                    break;
                case GameplayElementTypes.Ability:
                    DataAccess.CreateAbility(new Ability(_newEntityName));
                    break;
                case GameplayElementTypes.Consumable:
                    DataAccess.CreateConsumable(new Consumable(_newEntityName));
                    break;
            }

            _editableGameplayStringRepresentation = null;
        }
    }

    #endregion

    #region Editing

    void ShowEditingElements()
    {
        ShowGeneralEditingTop();

        switch (_currentlyEditedType)
        {
            case GameplayElementTypes.Action:
                ShowActionEditing();
                break;
            case GameplayElementTypes.Entity:
                ShowEntityEditing();
                break;
            case GameplayElementTypes.Ability:
                ShowAbilityEditing();
                break;
            case GameplayElementTypes.Consumable:
                ShowConsumableEditing();
                break;
        }

        ShowGeneralEditingEnd();
    }
    void ShowGeneralEditingTop()
    {
        EditorGUILayout.LabelField("Name: ", _editableGameplayStringRepresentation[_selectedElement]);
    }

    private bool _shouldShowEntities = false;
    void ShowActionEditing()
    {
        EditorGUILayout.Space();

        _shouldShowEntities = EditorGUILayout.Foldout(_shouldShowEntities, "Entities");

        if (_shouldShowEntities)
        {
            Action toEdit = DataAccess.GetGameplayElementContainer().GetElement(_selectedElement, GameplayElementTypes.Action) as Action;

            List<GameplayElement> entities = DataAccess.GetGameplayElementContainer().GetAllElements(GameplayElementTypes.Entity);

            foreach (Entity entity in entities)
            {
                bool containsAction = toEdit.ContainsEntity(entity);
                bool shouldContain = EditorGUILayout.Toggle(entity.Name, containsAction);
                if (containsAction && !shouldContain)
                {
                    toEdit.RemoveEntityToPerformOn(entity);
                    //entity.RemoveActionThatCanPerformOnThis(toEdit);
                }
                else if (!containsAction && shouldContain)
                {
                    toEdit.AddEntityToPerformOn(entity);
                    //entity.AddActionThatCanPerformOnThis(toEdit);
                }
            }
        }
    }

    private bool _shouldShowActions = false;
    void ShowEntityEditing()
    {
        //EditorGUILayout.Space();

        //_shouldShowActions = EditorGUILayout.Foldout(_shouldShowActions, "Actions");

        //if (_shouldShowActions)
        //{
        //    Entity toEdit = DataAccess.GetGameplayElementContainer().GetElement(_selectedElement, GameplayElementTypes.Entity) as Entity;

        //    List<GameplayElement> actions = DataAccess.GetGameplayElementContainer().GetAllElements(GameplayElementTypes.Action);

        //    foreach (Action action in actions)
        //    {
        //        bool containsAction = toEdit.ContainsAction(action);
        //        bool shouldContain = EditorGUILayout.Toggle(action.Name, containsAction);
        //        if (containsAction && !shouldContain)
        //        {
        //            toEdit.RemoveActionThatCanPerformOnThis(action);
        //            action.RemoveEntityToPerformOn(toEdit);
        //        }
        //        else if (!containsAction && shouldContain)
        //        {
        //            toEdit.AddActionThatCanPerformOnThis(action);
        //            action.AddEntityToPerformOn(toEdit);
        //        }
        //    }
        //}
    }
    void ShowAbilityEditing()
    {

    }
    void ShowConsumableEditing()
    {

    }
    void ShowGeneralEditingEnd()
    {
        if (GUILayout.Button("Delete"))
        {
            GameplayContainer allGameplay = DataAccess.GetGameplayContainer();

            if (allGameplay.UsesGameplayElement(
                DataAccess.GetGameplayElementContainer().GetElement(_selectedElement, _currentlyEditedType),
                _currentlyEditedType))
            {
                ErrorWindow.ShowWindow("You cant delete a gameplay element if it is still used in defined gameplay!", null);
            }
            else
                DataAccess.GetGameplayElementContainer().DeleteElement(DataAccess.GetGameplayElementContainer().GetElement(_selectedElement, _currentlyEditedType), _currentlyEditedType);

            _editableGameplayStringRepresentation = null;
        }
    }

    #endregion

}
