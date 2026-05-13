using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;

namespace Tests
{
    public class QuestVerifierTest
    {
        private const string DatabasePath = "Assets/ScriptableObjects/Databases/QuestDatabase.asset";

        [Test]
        public void AllQuestNodes_HaveTargetNpcAssigned()
        {
            var db = AssetDatabase.LoadAssetAtPath<QuestDatabase>(DatabasePath);
            Assert.IsNotNull(db, $"QuestDatabase not found at {DatabasePath}");

            var errors = new List<string>();

            foreach (QuestGraph quest in db.quests)
            {
                if (quest == null || quest.nodes == null) continue;

                foreach (XNode.Node node in quest.nodes)
                {
                    if (node is QuestNodeBase questNode && questNode.targetNpc == null)
                    {
                        errors.Add($"{quest.displayName} ({quest.questId}) > {node.GetType().Name}: targetNpc is not assigned");
                    }
                }
            }

            Assert.IsEmpty(errors, "Nodes missing targetNpc:\n" + string.Join("\n", errors));
        }

        [Test]
        public void AllQuestNodes_HaveTipFilled()
        {
            var db = AssetDatabase.LoadAssetAtPath<QuestDatabase>(DatabasePath);
            Assert.IsNotNull(db, $"QuestDatabase not found at {DatabasePath}");

            var errors = new List<string>();

            foreach (QuestGraph quest in db.quests)
            {
                if (quest == null || quest.nodes == null) continue;

                foreach (XNode.Node node in quest.nodes)
                {
                    string tip = null;
                    if (node is QuestChatNode chatNode)
                    {
                        tip = chatNode.tip;
                    }
                    else if (node is QuestGiftNode giftNode)
                    {
                        tip = giftNode.tip;
                    }
                    else if (node is QuestWorldObjectiveNode worldNode)
                    {
                        tip = worldNode.tip;
                    }

                    if (tip != null && string.IsNullOrWhiteSpace(tip))
                    {
                        errors.Add($"{quest.displayName} ({quest.questId}) > {node.GetType().Name}: tip is empty");
                    }
                }
            }

            Assert.IsEmpty(errors, "Nodes missing tip:\n" + string.Join("\n", errors));
        }
    }
}
