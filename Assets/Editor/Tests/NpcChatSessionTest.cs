using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class NpcChatSessionTest
    {
        [Test]
        public void GraphValidation_FailsWhenNoNodes()
        {
            var g = ScriptableObject.CreateInstance<NpcChatGraph>();

            Assert.IsFalse(g.TryValidate(out string err));
            Assert.IsNotNull(err);
        }

        [Test]
        public void GraphValidation_SucceedsForSimpleLinearTree()
        {
            var g = ScriptableObject.CreateInstance<NpcChatGraph>();
            var a = new ChatTreeNodeData
            {
                body = "Line a",
                choices = new System.Collections.Generic.List<ChatChoiceData>
                {
                    new ChatChoiceData { label = "Go", nextNodeId = "b" },
                },
            };
            a.SetId("a");
            g.nodes.Add(a);

            var b = new ChatTreeNodeData
            {
                body = "Line b",
                choices = new System.Collections.Generic.List<ChatChoiceData>(),
            };
            b.SetId("b");
            g.nodes.Add(b);

            Assert.IsTrue(g.TryValidate(out string err), err);
        }

        [Test]
        public void Session_FollowsChoiceToNextNode()
        {
            var g = ScriptableObject.CreateInstance<NpcChatGraph>();
            var a = new ChatTreeNodeData
            {
                body = "Line a",
                choices = new System.Collections.Generic.List<ChatChoiceData>
                {
                    new ChatChoiceData { label = "Go", nextNodeId = "b" },
                },
            };
            a.SetId("a");
            g.nodes.Add(a);

            var b = new ChatTreeNodeData
            {
                body = "Line b",
                choices = new System.Collections.Generic.List<ChatChoiceData>
                {
                    new ChatChoiceData { label = "End", nextNodeId = "" },
                },
            };
            b.SetId("b");
            g.nodes.Add(b);

            var session = new NpcChatSession(g);
            Assert.IsTrue(session.TryGetCurrentNode(out ChatTreeNodeData first));
            Assert.AreEqual("a", first.Id);

            Assert.IsTrue(session.TryChoose(0, out bool ended));
            Assert.IsFalse(ended);
            Assert.IsTrue(session.TryGetCurrentNode(out ChatTreeNodeData second));
            Assert.AreEqual("b", second.Id);

            Assert.IsTrue(session.TryChoose(0, out ended));
            Assert.IsTrue(ended);
            Assert.IsTrue(session.IsFinished);
        }
    }
}
