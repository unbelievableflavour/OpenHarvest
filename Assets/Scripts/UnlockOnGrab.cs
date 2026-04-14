using UnityEngine;
using UnityEngine.UI;

namespace BNG
{
    public class UnlockOnGrab : GrabbableEvents
    {
        public GameObject unlockMessage;
        private ItemInformation itemInformation;
        public AudioClip unlockSound;

        private bool alreadyTriggered = false;

        void Start()
        {
            unlockMessage.SetActive(false);
        }

        public override void OnGrab(Grabber grabber)
        {
            if (alreadyTriggered)
            {
                return;
            }
            alreadyTriggered = true;
            
            
            var item = Definitions.GetItemFromObject(this.gameObject);
            if (item == null)
            {
                return;
            }

            if (GameState.Instance.ownsMaximumNumber(item))
            {
                return;
            }

            if (unlockSound)
            {
                VRUtils.Instance.PlaySpatialClipAt(unlockSound, transform.position, 1f, 1f);
            }

            GameState.Instance.unlock(item.itemId, 1);
            unlockMessage.GetComponentInChildren<Text>().text = "Unlocked " + item.name + "!";

            CancelInvoke("HideUnlockedMessage");
            ShowUnlockedMessage();
            Invoke("HideUnlockedMessage", 2.0f);

            base.OnGrab(grabber);
        }

        void ShowUnlockedMessage()
        {
            unlockMessage.SetActive(true);
        }

        void HideUnlockedMessage()
        {
            unlockMessage.SetActive(false);
        }
    }
}