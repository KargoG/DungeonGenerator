using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


public class WarningWindow : EditorWindow
{
    private UnityAction _confirmationAction;
    private string _text;

    public static WarningWindow ShowWindow(string textToShow, UnityAction confirmationAction)
    {
        WarningWindow createdWindow = CreateWindow<WarningWindow>();

        createdWindow._confirmationAction = confirmationAction;
        createdWindow._text = textToShow;

        return createdWindow;
    }

    void OnGUI()
    {
        EditorGUILayout.TextArea(_text);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Okay"))
        {
            _confirmationAction?.Invoke();
            this.Close();
        }
        EditorGUILayout.EndHorizontal();
    }
}

public class ErrorWindow : EditorWindow
{
    private UnityAction _confirmationAction;
    private string _text;

    public static ErrorWindow ShowWindow(string textToShow, UnityAction confirmationAction)
    {
        ErrorWindow createdWindow = CreateWindow<ErrorWindow>();

        createdWindow._confirmationAction = confirmationAction;
        createdWindow._text = textToShow;

        return createdWindow;
    }

    void OnGUI()
    {
        EditorGUILayout.TextArea(_text);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Okay"))
        {
            _confirmationAction?.Invoke();
            this.Close();
        }
        EditorGUILayout.EndHorizontal();
    }
}

public class ConfirmationWindow : EditorWindow
{
    private UnityAction _confirmationAction;
    private UnityAction _declineAction;
    private string _text;

    public static ConfirmationWindow ShowWindow(string textToShow, UnityAction confirmationAction, UnityAction declineAction)
    {
        ConfirmationWindow createdWindow = CreateWindow<ConfirmationWindow>();

        createdWindow._confirmationAction = confirmationAction;
        createdWindow._declineAction = declineAction;
        createdWindow._text = textToShow;

        return createdWindow;
    }

    void OnGUI()
    {
        EditorGUILayout.TextArea(_text);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Okay"))
        {
            _confirmationAction?.Invoke();
            this.Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            _declineAction?.Invoke();
            this.Close();
        }
        EditorGUILayout.EndHorizontal();
    }
}
