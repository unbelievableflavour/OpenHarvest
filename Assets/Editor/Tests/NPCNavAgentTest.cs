using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class NPCNavAgentTest
    {
        GameObject npcGo;
        GameObject lookTargetGo;
        NPCNavAgent nav;

        [SetUp]
        public void SetUp()
        {
            npcGo = new GameObject("Npc");
            nav = npcGo.AddComponent<NPCNavAgent>();
            EditModeLifecycle.InvokeAwake(nav);
            lookTargetGo = new GameObject("LookTarget");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(npcGo);
            Object.DestroyImmediate(lookTargetGo);
        }

        [Test]
        public void BeginInteractionAim_LocksHorizontalAimToOpeningDirection()
        {
            npcGo.transform.position = Vector3.zero;
            lookTargetGo.transform.position = new Vector3(0f, 0f, 5f);

            nav.BeginInteractionAim(lookTargetGo.transform);
            lookTargetGo.transform.position = new Vector3(-5f, 0f, 0f);

            SimulateInteractionAimUntilSettled();

            Vector3 expectedForward = Vector3.forward;
            Assert.Less(AngleBetween(npcGo.transform.forward, expectedForward), 5f);
        }

        [Test]
        public void InteractionAim_StopsRotatingAfterSettled_WhenLookTargetMoves()
        {
            npcGo.transform.position = Vector3.zero;
            npcGo.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            lookTargetGo.transform.position = new Vector3(0f, 0f, 5f);

            nav.BeginInteractionAim(lookTargetGo.transform);
            SimulateInteractionAimUntilSettled();

            Quaternion settledRotation = npcGo.transform.rotation;
            lookTargetGo.transform.position = new Vector3(-5f, 0f, 0f);

            for (int i = 0; i < 30; i++)
            {
                EditModeLifecycle.InvokeMethod(nav, "Update");
            }

            Assert.Less(Quaternion.Angle(settledRotation, npcGo.transform.rotation), 1f);
        }

        [Test]
        public void EndInteractionAim_ClearsLockedAim()
        {
            lookTargetGo.transform.position = new Vector3(0f, 0f, 5f);
            nav.BeginInteractionAim(lookTargetGo.transform);
            nav.EndInteractionAim();

            Quaternion before = npcGo.transform.rotation;
            lookTargetGo.transform.position = new Vector3(-5f, 0f, 0f);

            for (int i = 0; i < 10; i++)
            {
                EditModeLifecycle.InvokeMethod(nav, "Update");
            }

            Assert.Less(Quaternion.Angle(before, npcGo.transform.rotation), 1f);
        }

        void SimulateInteractionAimUntilSettled()
        {
            for (int i = 0; i < 120; i++)
            {
                EditModeLifecycle.InvokeMethod(nav, "Update");
            }
        }

        static float AngleBetween(Vector3 a, Vector3 b)
        {
            return Vector3.Angle(a, b);
        }
    }
}
