using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace OpenHarvest.Editor.Tests.QuestGraph
{
    /// <summary>
    /// Locks serialized field names used by <see cref="QuestNodeEditorDrawing.PropertyFieldOrSkip"/>.
    /// </summary>
    public class QuestNodeSerializedPropertyPathsTest
    {
        [Test]
        public void QuestChatNode_HasExpectedSerializedProperties()
        {
            QuestChatNode node = ScriptableObject.CreateInstance<QuestChatNode>();
            try
            {
                SerializedObject serializedObject = new SerializedObject(node);
                Assert.IsNotNull(serializedObject.FindProperty("targetNpc"), "targetNpc");
                Assert.IsNotNull(serializedObject.FindProperty("body"), "body");
                Assert.IsNotNull(serializedObject.FindProperty("tip"), "tip");
            }
            finally
            {
                Object.DestroyImmediate(node);
            }
        }

        [Test]
        public void QuestFinishNode_HasExpectedSerializedProperties()
        {
            QuestFinishNode node = ScriptableObject.CreateInstance<QuestFinishNode>();
            try
            {
                SerializedObject serializedObject = new SerializedObject(node);
                Assert.IsNotNull(serializedObject.FindProperty("targetNpc"), "targetNpc");
                Assert.IsNotNull(serializedObject.FindProperty("coinReward"), "coinReward");
                Assert.IsNotNull(serializedObject.FindProperty("handRewardItem"), "handRewardItem");
            }
            finally
            {
                Object.DestroyImmediate(node);
            }
        }

        [Test]
        public void QuestGiftNode_HasExpectedSerializedProperties()
        {
            QuestGiftNode node = ScriptableObject.CreateInstance<QuestGiftNode>();
            try
            {
                SerializedObject serializedObject = new SerializedObject(node);
                Assert.IsNotNull(serializedObject.FindProperty("targetNpc"), "targetNpc");
                Assert.IsNotNull(serializedObject.FindProperty("giftPrompt"), "giftPrompt");
                Assert.IsNotNull(serializedObject.FindProperty("tip"), "tip");
                Assert.IsNotNull(serializedObject.FindProperty("requiredItem"), "requiredItem");
                Assert.IsNotNull(serializedObject.FindProperty("requiredAmount"), "requiredAmount");
            }
            finally
            {
                Object.DestroyImmediate(node);
            }
        }

        [Test]
        public void QuestWorldObjectiveNode_HasExpectedSerializedProperties()
        {
            QuestWorldObjectiveNode node = ScriptableObject.CreateInstance<QuestWorldObjectiveNode>();
            try
            {
                SerializedObject serializedObject = new SerializedObject(node);
                Assert.IsNotNull(serializedObject.FindProperty("targetNpc"), "targetNpc");
                Assert.IsNotNull(serializedObject.FindProperty("objectiveKey"), "objectiveKey");
            }
            finally
            {
                Object.DestroyImmediate(node);
            }
        }
    }
}
