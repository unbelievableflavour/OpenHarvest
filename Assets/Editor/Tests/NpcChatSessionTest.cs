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
            NpcChatNode a = g.AddNode<NpcChatNode>();
            a.body = "Line a";
            a.choices.Add("Go");

            NpcChatNode b = g.AddNode<NpcChatNode>();
            b.body = "Line b";

            a.UpdatePorts();
            b.UpdatePorts();

            g.entryNode = a;

            Assert.IsTrue(g.TryValidate(out string err), err);
        }

        [Test]
        public void Session_FollowsChoiceToNextNode()
        {
            var g = ScriptableObject.CreateInstance<NpcChatGraph>();
            NpcChatNode a = g.AddNode<NpcChatNode>();
            a.body = "Line a";
            a.choices.Add("Go");

            NpcChatNode b = g.AddNode<NpcChatNode>();
            b.body = "Line b";
            b.choices.Add("End");

            a.UpdatePorts();
            b.UpdatePorts();

            var outPort = a.GetOutputPort("choices 0") ?? a.GetOutputPort("choices");
            var inPort = b.GetInputPort("inFlow");
            Assert.IsNotNull(outPort, "Expected an output port for choices.");
            Assert.IsNotNull(inPort, "Expected input port 'inFlow'.");
            outPort.Connect(inPort);
            g.entryNode = a;

            var session = new NpcChatSession(g);
            Assert.IsTrue(session.TryGetCurrentNode(out NpcChatNode first));
            Assert.AreEqual(a, first);

            Assert.IsTrue(session.TryChoose(0, out bool ended));
            Assert.IsFalse(ended);
            Assert.IsTrue(session.TryGetCurrentNode(out NpcChatNode second));
            Assert.AreEqual(b, second);

            Assert.IsTrue(session.TryChoose(0, out ended));
            Assert.IsTrue(ended);
            Assert.IsTrue(session.IsFinished);
        }
    }
}
