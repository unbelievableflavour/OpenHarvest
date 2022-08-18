#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
namespace HarvestDataTypes
{
    [CustomEditor(typeof(Contract))]
    public class ContractEditor : DatabaseConnectedObjectEditor
    {
        private string dbLocation = "Assets/ScriptableObjects/Databases/ContractDatabase.asset";

        public override void InitializeDB() {
            currentItem = (Contract)target;
            db = (ContractDatabase)AssetDatabase.LoadAssetAtPath(dbLocation, typeof(ContractDatabase));
            dbList = db.contracts;
        }

        public void OnEnable()
        {
            base.OnEnable();

            var scriptableObject = (ScriptableObject)target;
            currentItem.contractId = scriptableObject.name;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Calculate gold reward", GUILayout.Height(20)))
            {
                currentItem.rewardGold = 0;
                foreach (Requirement requirement in currentItem.requirements)
                {
                    currentItem.rewardGold += (int)(requirement.amount * requirement.item.sellPrice * 1.3);
                    EditorUtility.SetDirty(currentItem);
                }
            }
        }
    }
}
#endif