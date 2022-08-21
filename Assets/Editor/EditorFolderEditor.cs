using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

 [CustomEditor(typeof(EditorFolder)), CanEditMultipleObjects]
 public class FolderEditor : Editor 
 {
    private EditorFolder folder;

    public override void OnInspectorGUI()
    {
        folder = (EditorFolder)target;

        if (
            folder.tag != "EditorOnly" || 
            folder.transform.hideFlags != (HideFlags.NotEditable | HideFlags.HideInInspector)
            ) 
        {
            folder.tag = "EditorOnly";
            folder.transform.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
            folder.transform.position = Vector3.zero;
            EditorUtility.SetDirty(folder);
        }

        EditorGUILayout.HelpBox("This is an editor folder, it can be used to structure your scene hierarchy. This gameObject is removed during build.", MessageType.Info);
        base.OnInspectorGUI();
    }

    void OnEnable()
    {

    }
 }