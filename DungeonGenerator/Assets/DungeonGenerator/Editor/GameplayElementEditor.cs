using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DungeonGenerator.Editor
{
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
            _currentlyEditedType =
                (GameplayElementTypes) EditorGUILayout.EnumPopup("Gamplay Element", _currentlyEditedType);

            if (lastEditedType != _currentlyEditedType)
            {
                _selectedElement = 0;
                _editableGameplayStringRepresentation = null;
            }


            if (_editableGameplayStringRepresentation == null)
                _editableGameplayStringRepresentation =
                    new string[DataAccess.GetGameplayElementContainer().GetAllElements(_currentlyEditedType).Count + 1];

            for (int i = 0;
                i < DataAccess.GetGameplayElementContainer().GetAllElements(_currentlyEditedType).Count;
                i++)
            {
                _editableGameplayStringRepresentation[i] = DataAccess.GetGameplayElementContainer()
                    .GetElement(i, _currentlyEditedType).name;
            }


            _editableGameplayStringRepresentation[_editableGameplayStringRepresentation.Length - 1] = "Create New";

            _selectedElement =
                EditorGUILayout.Popup("Object to Edit", _selectedElement, _editableGameplayStringRepresentation);

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
            if (GUILayout.Button("Create") && _newEntityName.Length > 0 &&
                !_editableGameplayStringRepresentation.Contains(_newEntityName))
            {
                switch (_currentlyEditedType)
                {
                    case GameplayElementTypes.Action:
                        DataAccess.CreateAction(Action.Create(_newEntityName));
                        break;
                    case GameplayElementTypes.Entity:
                        DataAccess.CreateEntity(Entity.Create(_newEntityName));
                        break;
                    case GameplayElementTypes.Ability:
                        DataAccess.CreateAbility(Ability.Create(_newEntityName));
                        break;
                    case GameplayElementTypes.Consumable:
                        DataAccess.CreateConsumable(Consumable.Create(_newEntityName));
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
                Action toEdit = DataAccess.GetGameplayElementContainer()
                    .GetElement(_selectedElement, GameplayElementTypes.Action) as Action;

                List<GameplayElement> entities = DataAccess.GetGameplayElementContainer()
                    .GetAllElements(GameplayElementTypes.Entity);

                foreach (Entity entity in entities)
                {
                    bool containsAction = toEdit.ContainsEntity(entity);
                    bool shouldContain = EditorGUILayout.Toggle(entity.name, containsAction);
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

        void ShowEntityEditing()
        {
            Entity shownEntity =
                DataAccess.GetGameplayElementContainer().GetElement(_selectedElement, _currentlyEditedType) as Entity;

            if (shownEntity == null)
                return;
            
            GameObject representation = // TODO this seems to break the serialization IDK why
                EditorGUILayout.ObjectField(shownEntity.Representation, typeof(GameObject), false) as GameObject;

            if(shownEntity.Representation != representation) // have to do this since unity doesn`t save changes otherwise
            {
                shownEntity.Representation = representation;
                EditorUtility.SetDirty(shownEntity);
                AssetDatabase.SaveAssets();
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
                GameplayContainer allGameplay = DataAccess.GetGameplayContainer();

                if (allGameplay.UsesGameplayElement(
                    DataAccess.GetGameplayElementContainer().GetElement(_selectedElement, _currentlyEditedType),
                    _currentlyEditedType))
                {
                    ErrorWindow.ShowWindow(
                        "You cant delete a gameplay element if it is still used in defined gameplay!", null);
                }
                else
                {
                    GameplayElement toDelete = DataAccess.GetGameplayElementContainer()
                        .GetElement(_selectedElement, _currentlyEditedType);
                    switch (_currentlyEditedType)
                    {
                        case GameplayElementTypes.Action:
                            DataAccess.DeleteAction(toDelete as Action);
                            break;
                        case GameplayElementTypes.Entity:
                            DataAccess.DeleteEntity(toDelete as Entity);
                            break;
                        case GameplayElementTypes.Ability:
                            DataAccess.DeleteAbility(toDelete as Ability);
                            break;
                        case GameplayElementTypes.Consumable:
                            DataAccess.DeleteConsumable(toDelete as Consumable);
                            break;
                    }
                }

                _editableGameplayStringRepresentation = null;
            }
        }

        #endregion

    }
}
