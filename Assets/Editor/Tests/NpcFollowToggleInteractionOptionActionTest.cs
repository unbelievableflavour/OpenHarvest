using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class NpcFollowToggleInteractionOptionActionTest
    {
        NpcFollowToggleInteractionOptionAction action;
        GameObject playerGo;

        [SetUp]
        public void SetUp()
        {
            GameState.Reset();
            action = ScriptableObject.CreateInstance<NpcFollowToggleInteractionOptionAction>();
            playerGo = new GameObject("Player");
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(action);
            Object.DestroyImmediate(playerGo);
            GameState.Reset();
        }

        // --- ResolveDisplayName ---

        [Test]
        public void ResolveDisplayName_WhenInteractableIsNull_ReturnsFollowLabel()
        {
            string label = action.ResolveDisplayName(new NpcInteractionOption(), null);

            Assert.AreEqual("Follow me", label);
        }

        [Test]
        public void ResolveDisplayName_WhenNotFollowing_ReturnsFollowLabel()
        {
            var (nav, interactable) = CreateNpcSetup("NpcA");
            nav.followTarget = null;

            string label = action.ResolveDisplayName(new NpcInteractionOption(), interactable);

            Assert.AreEqual("Follow me", label);

            Object.DestroyImmediate(nav.gameObject);
        }

        [Test]
        public void ResolveDisplayName_WhenFollowing_ReturnsStopFollowingLabel()
        {
            var (nav, interactable) = CreateNpcSetup("NpcA");
            nav.followTarget = playerGo.transform;

            string label = action.ResolveDisplayName(new NpcInteractionOption(), interactable);

            Assert.AreEqual("Stop following", label);

            Object.DestroyImmediate(nav.gameObject);
        }

        // --- Follow / StopFollowing GameState ---

        [Test]
        public void Follow_SetsFollowingNpcIdInGameState()
        {
            var (nav, _) = CreateNpcSetup("Jeff");

            nav.Follow(playerGo.transform);

            Assert.AreEqual("Jeff", GameState.Instance.followingNpcId);

            Object.DestroyImmediate(nav.gameObject);
        }

        [Test]
        public void StopFollowing_ClearsFollowingNpcIdInGameState()
        {
            var (nav, _) = CreateNpcSetup("Jeff");
            nav.Follow(playerGo.transform);

            nav.StopFollowing();

            Assert.IsNull(GameState.Instance.followingNpcId);
            Assert.IsNull(GameState.Instance.followingNpcInstanceId);

            Object.DestroyImmediate(nav.gameObject);
        }

        [Test]
        public void Follow_WithoutPlateau_LeavesFollowingNpcInstanceIdNull()
        {
            // Named NPCs and spawned animals have no plateau — instance ID should stay null.
            var (nav, _) = CreateNpcSetup("Jeff");

            nav.Follow(playerGo.transform);

            Assert.IsNull(GameState.Instance.followingNpcInstanceId);

            Object.DestroyImmediate(nav.gameObject);
        }

        // --- Auto-unfollow ---

        [Test]
        public void Follow_WhenAnotherNpcIsFollowing_StopsOtherNpc()
        {
            var (navA, _) = CreateNpcSetup("NpcA");
            var (navB, _) = CreateNpcSetup("NpcB");

            navA.Follow(playerGo.transform);
            Assert.AreEqual(playerGo.transform, navA.followTarget);

            navB.Follow(playerGo.transform);

            Assert.IsNull(navA.followTarget, "NPC A should stop following when NPC B starts");
            Assert.AreEqual(playerGo.transform, navB.followTarget);
            Assert.AreEqual("NpcB", GameState.Instance.followingNpcId);

            Object.DestroyImmediate(navA.gameObject);
            Object.DestroyImmediate(navB.gameObject);
        }

        [Test]
        public void Follow_WhenSameNpcFollowsAgain_OnlyOneFollowerExists()
        {
            var (nav, _) = CreateNpcSetup("Jeff");
            var target2 = new GameObject("Player2");

            nav.Follow(playerGo.transform);
            nav.Follow(target2.transform);

            Assert.AreEqual(target2.transform, nav.followTarget);
            Assert.AreEqual("Jeff", GameState.Instance.followingNpcId);

            Object.DestroyImmediate(nav.gameObject);
            Object.DestroyImmediate(target2);
        }

        // Helper: creates a root GameObject with NPCNavAgent + NpcProximityInteractable on the same object.
        // GetComponentInParent<NPCNavAgent>() on the interactable finds the agent on the same object.
        static (NPCNavAgent nav, NpcProximityInteractable interactable) CreateNpcSetup(string name)
        {
            var go = new GameObject(name);
            var nav = go.AddComponent<NPCNavAgent>();
            var interactable = go.AddComponent<NpcProximityInteractable>();
            return (nav, interactable);
        }
    }
}
