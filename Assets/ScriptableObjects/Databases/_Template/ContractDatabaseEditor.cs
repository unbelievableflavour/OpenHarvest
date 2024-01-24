#if (UNITY_EDITOR) 
using UnityEngine;
using UnityEditor;
using HarvestDataTypes;
using System.Collections.Generic;

[CustomEditor(typeof(ContractDatabase))]
public class ContractDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var database = (ContractDatabase)target;

        if (GUILayout.Button("Add all contracts", GUILayout.Height(20)))
        {
            database.contracts = new List<Contract>();

            foreach (string s in AssetDatabase.FindAssets("", new string[] { "Assets/HarvestDataTypes/Contracts/" }))
            {
                Contract contract = (Contract)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(Contract));

                if (contract == null)
                {
                    continue;
                }

                database.contracts.Add(contract);
                EditorUtility.SetDirty(database);
            }
        }
    }
}
#endif