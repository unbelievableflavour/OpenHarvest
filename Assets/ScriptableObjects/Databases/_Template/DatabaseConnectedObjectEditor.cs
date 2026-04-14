#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DatabaseConnectedObjectEditor : Editor {
    protected dynamic currentItem;
    protected dynamic db;
    protected dynamic dbList;
    
    protected bool currentToggleValue;
    protected bool lastToggleValue;

    public void OnEnable()
    {
        InitializeDB();
        InitializeDBToggle();
    }

    public virtual void InitializeDB(){
        throw new System.Exception("DB is not yet configured!");
    }

    public override void OnInspectorGUI()
    {
        AddDBToggle(db);
        EditorGUILayout.Space();
        base.OnInspectorGUI();
    }

    public void AddDBToggle(dynamic db) {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        currentToggleValue = GUILayout.Toggle(currentToggleValue, "Added to Database", GUILayout.Height(20));
        if (lastToggleValue != currentToggleValue)
        {
            if (currentToggleValue)
            {
                dbList.Add(currentItem);
            }
            else
            {
                dbList.Remove(currentItem);
            }

            EditorUtility.SetDirty(db);
            lastToggleValue = currentToggleValue;
        }

        EditorGUILayout.EndHorizontal();
    }

    public void InitializeDBToggle() {
        currentToggleValue = dbList.Contains(currentItem);
        lastToggleValue = currentToggleValue;
    }
}
#endif