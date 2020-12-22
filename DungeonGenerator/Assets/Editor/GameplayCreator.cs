using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameplayCreator : EditorWindow
{
    private static GameplayElementContainer _gameplayElements;
    private static GameplayContainer _gameplay;
    private static GameplayCreator _window;

    private static List<GameplayElement> _actions;
    private static List<GameplayElement> _entities;
    private static List<GameplayElement> _abilities;
    private static List<GameplayElement> _consumables;

    private static string[] _actionNames;
    private static string[] _entityNames;
    private static string[] _abilityNames;
    private static string[] _consumableNames;

    public static void ShowGameplayCreator(GameplayElementContainer gameplayElements)
    {
        _gameplayElements = gameplayElements;

        _actions = _gameplayElements.GetAllElements(GameplayElementTypes.Action);
        _entities = _gameplayElements.GetAllElements(GameplayElementTypes.Entity);
        _abilities = _gameplayElements.GetAllElements(GameplayElementTypes.Ability);
        _consumables = _gameplayElements.GetAllElements(GameplayElementTypes.Consumable);

        _actionNames = new string[_actions.Count + 1];
        _entityNames = new string[_entities.Count + 1];
        _abilityNames = new string[_abilities.Count + 1];
        _consumableNames = new string[_consumables.Count + 1];

        _actionNames[0] = "";
        _entityNames[0] = "";
        _abilityNames[0] = "";
        _consumableNames[0] = "";

        for (int i = 1; i < _actionNames.Length; i++)
        {
            _actionNames[i] = _actions[i-1].Name;
        }
        for (int i = 1; i < _entityNames.Length; i++)
        {
            _entityNames[i] = _entities[i-1].Name;
        }
        for (int i = 1; i < _abilityNames.Length; i++)
        {
            _abilityNames[i] = _abilities[i-1].Name;
        }
        for (int i = 1; i < _consumableNames.Length; i++)
        {
            _consumableNames[i] = _consumables[i-1].Name;
        }

        _window = GetWindow<GameplayCreator>();
    }

    private Action _selectedAction;
    private Entity _selectedEntity;
    private Ability _selectedAbility;
    private Consumable _selectedConsumable;

    private int _selectedActionIndex = 0;
    private int _selectedEntityIndex = 0;

    private int _connectionIndex = 0;
    private int _abilityOrConsumableIndex = 0;
    private int _selectedAbilityIndex = 0;
    private int _selectedConsumableIndex = 0;



    private void OnLostFocus()
    {
        _window.Close();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Action");
        _selectedActionIndex = EditorGUILayout.Popup(_selectedActionIndex, _actionNames);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Entity");
        _selectedEntityIndex = EditorGUILayout.Popup(_selectedEntityIndex, _entityNames);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(1, true);
        _connectionIndex = EditorGUILayout.Popup(_connectionIndex, new[] { "with a", "using a", "by" });
        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical();
        _abilityOrConsumableIndex = EditorGUILayout.Popup(_abilityOrConsumableIndex, new []{"Nothing", "Ability", "Consumable" });

        if (_abilityOrConsumableIndex == 0)
            EditorGUILayout.LabelField("Nothing");
        else if(_abilityOrConsumableIndex == 1)
            _selectedAbilityIndex = EditorGUILayout.Popup(_selectedAbilityIndex, _abilityNames);
        else if(_abilityOrConsumableIndex == 2)
            _selectedConsumableIndex = EditorGUILayout.Popup(_selectedConsumableIndex, _consumableNames);

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();



        if (GUILayout.Button("Create"))
        {
            _window.Close();
        }

    }
}
