using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class HideIfQuestNotFinishedTest
    {
        [TearDown]
        public void TearDown()
        {
            GameState.Reset();
        }

        [Test]
        public void CheckQuestStatus_HidesUntilRuntimeStateIsCompleted()
        {
            GameState.Reset();
            GameObject go = new GameObject("HideIfQuestNotFinishedTest");
            HideIfQuestNotFinished component = go.AddComponent<HideIfQuestNotFinished>();
            SetPrivateField(component, "questId", "test_quest_nf");

            GameState.Instance.questRuntimeStates["test_quest_nf"] = new QuestRuntimeProgressState { isCompleted = false };
            component.CheckQuestStatus();
            Assert.IsFalse(go.activeSelf);

            GameState.Instance.questRuntimeStates["test_quest_nf"] = new QuestRuntimeProgressState { isCompleted = true };
            component.CheckQuestStatus();
            Assert.IsTrue(go.activeSelf);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void CheckQuestStatus_WithEmptyQuestId_Deactivates()
        {
            GameState.Reset();
            GameObject go = new GameObject("HideIfQuestNotFinishedTest2");
            HideIfQuestNotFinished component = go.AddComponent<HideIfQuestNotFinished>();
            go.SetActive(true);

            component.CheckQuestStatus();
            Assert.IsFalse(go.activeSelf);

            Object.DestroyImmediate(go);
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(field, $"Missing private field '{fieldName}' on {target.GetType().Name}");
            field.SetValue(target, value);
        }
    }
}
