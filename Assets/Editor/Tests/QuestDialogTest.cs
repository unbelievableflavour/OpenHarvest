using UnityEngine;
using NUnit.Framework;
using UnityEditor;

namespace Crops
{
    public class QuestDialogTest
    {
        GameObject questDialog;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/QuestDialog/QuestDialog.prefab");
            questDialog = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItHasDialogueAsAParameterInsteadOfGetComponentToBeReliable()
        {
            var dialogue = questDialog.GetComponent<Dialogue>();
            var dialogueController = questDialog.GetComponent<QuestDialogueController>();

            Assert.AreNotEqual(null, dialogue);
            Assert.AreEqual(dialogue, dialogueController.dialogue);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(questDialog);
        }
    }
}
