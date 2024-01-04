#if (UNITY_EDITOR) 
using UnityEngine;
using UnityEditor;
using HarvestDataTypes;

[CustomEditor(typeof(ContractDatabase))]
public class ContractDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var database = (ContractDatabase)target;

        if (GUILayout.Button("Add all contracts", GUILayout.Height(20)))
        {
            foreach (string s in AssetDatabase.FindAssets("", new string[] { "Assets/ScriptableObjects/Contracts/" }))
            {
                Contract contract = (Contract)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(Contract));

                if (contract == null)
                {
                    continue;
                }

                if (database.contracts.Contains(contract))
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