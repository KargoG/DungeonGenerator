using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameplayElementEditor : EditorWindow
{
    private static GameplayElementContainer _gameplayElements;

    //private static List<Action> _editableActions;
    //private static List<Entity> _editableEntities;
    //private static List<Ability> _editableAbilities;
    //private static List<Consumable> _editableConsumables;

    [MenuItem("Window/DungeonCreator/GameplayElementEditor")]
    public static void ShowWindow()
    {
        string[] assetsGUID = AssetDatabase.FindAssets("t:GameplayElementContainer");

        if (_gameplayElements == null)
        {
            if (assetsGUID.Length > 0)
                LoadGameplayContainer(assetsGUID[0]);
            else
                CreateGameplayContainer();
        }

        //if (_editableActions == null)
        //    _editableActions = _gameplayElements.GetAllElements<Action>();

        //if (_editableEntities == null)
        //    _editableEntities = _gameplayElements.GetAllElements<Entity>();

        //if (_editableAbilities == null)
        //    _editableAbilities = _gameplayElements.GetAllElements<Ability>();

        //if (_editableConsumables == null)
        //    _editableConsumables = _gameplayElements.GetAllElements<Consumable>();

        GetWindow<GameplayElementEditor>();
    }

    private static void CreateGameplayContainer()
    {
        _gameplayElements = ScriptableObject.CreateInstance<GameplayElementContainer>();
        AssetDatabase.CreateAsset(_gameplayElements, "Assets/ScriptableObjects/GameplayElements.asset");
    }

    private static void LoadGameplayContainer(string guid)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        _gameplayElements = AssetDatabase.LoadAssetAtPath<GameplayElementContainer>(assetPath);
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
            _editableGameplayStringRepresentation = new string[_gameplayElements.GetAllElements(_currentlyEditedType).Count + 1];

        for (int i = 0; i < _gameplayElements.GetAllElements(_currentlyEditedType).Count; i++)
        {
            _editableGameplayStringRepresentation[i] = _gameplayElements.GetElement(i, _currentlyEditedType).Name;
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
                    _gameplayElements.AddElement(new Action(_newEntityName), GameplayElementTypes.Action);
                    break;
                case GameplayElementTypes.Entity:
                    _gameplayElements.AddElement(new Entity(_newEntityName), GameplayElementTypes.Entity);
                    break;
                case GameplayElementTypes.Ability:
                    _gameplayElements.AddElement(new Ability(_newEntityName), GameplayElementTypes.Ability);
                    break;
                case GameplayElementTypes.Consumable:
                    _gameplayElements.AddElement(new Consumable(_newEntityName), GameplayElementTypes.Consumable);
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
            Action toEdit = _gameplayElements.GetElement(_selectedElement, GameplayElementTypes.Action) as Action;

            List<GameplayElement> entities = _gameplayElements.GetAllElements(GameplayElementTypes.Entity);

            foreach (Entity entity in entities)
            {
                bool containsAction = toEdit.ContainsEntity(entity);
                bool shouldContain = EditorGUILayout.Toggle(entity.Name, containsAction);
                if (containsAction && !shouldContain)
                {
                    toEdit.RemoveEntityToPerformOn(entity);
                    entity.RemoveActionThatCanPerformOnThis(toEdit);
                }
                else if (!containsAction && shouldContain)
                {
                    toEdit.AddEntityToPerformOn(entity);
                    entity.AddActionThatCanPerformOnThis(toEdit);
                }
            }
        }
    }

    private bool _shouldShowActions = false;
    void ShowEntityEditing()
    {
        EditorGUILayout.Space();

        _shouldShowActions = EditorGUILayout.Foldout(_shouldShowActions, "Actions");

        if (_shouldShowActions)
        {
            Entity toEdit = _gameplayElements.GetElement(_selectedElement, GameplayElementTypes.Entity) as Entity;

            List<GameplayElement> actions = _gameplayElements.GetAllElements(GameplayElementTypes.Action);

            foreach (Action action in actions)
            {
                bool containsAction = toEdit.ContainsAction(action);
                bool shouldContain = EditorGUILayout.Toggle(action.Name, containsAction);
                if (containsAction && !shouldContain)
                {
                    toEdit.RemoveActionThatCanPerformOnThis(action);
                    action.RemoveEntityToPerformOn(toEdit);
                }
                else if (!containsAction && shouldContain)
                {
                    toEdit.AddActionThatCanPerformOnThis(action);
                    action.AddEntityToPerformOn(toEdit);
                }
            }
        }
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
            _gameplayElements.DeleteElement(_gameplayElements.GetElement(_selectedElement, _currentlyEditedType), _currentlyEditedType);

            _editableGameplayStringRepresentation = null;
        }
    }

    #endregion

}
