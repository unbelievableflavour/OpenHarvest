#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
namespace HarvestDataTypes
{
    [CustomEditor(typeof(Contract))]
    public class ContractEditor : Editor
    {
        public void OnEnable()
        {
            var contract = (Contract)target;
            var scriptableObject = (ScriptableObject)target;

            contract.contractId = scriptableObject.name;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var contract = (Contract)target;

            if (GUILayout.Button("Calculate gold reward", GUILayout.Height(20)))
            {
                contract.rewardGold = 0;
                foreach (Requirement requirement in contract.requirements)
                {
                    contract.rewardGold += (int)(requirement.amount * requirement.item.sellPrice * 1.3);
                    EditorUtility.SetDirty(contract);
                }
            }
        }
    }
}
#endif