﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace BNG
{
    public class ShowUnlockMessageOnGrab : GrabbableEvents
    {
        public GameObject unlockMessage;
        public AudioClip unlockSound;

        private bool alreadyTriggered = false;
        private HarvestDataTypes.Item newItem;

        void Start()
        {
            unlockMessage.SetActive(false);
        }

        public override void OnGrab(Grabber grabber)
        {
            Debug.Log(newItem);
            if(this.newItem == null)
            {
                return;
            }

            if (alreadyTriggered)
            {
                return;
            }

            alreadyTriggered = true;

            if (unlockSound)
            {
                VRUtils.Instance.PlaySpatialClipAt(unlockSound, transform.position, 1f, 1f);
            }

            unlockMessage.GetComponentInChildren<Text>().text = "Unlocked " + this.newItem.name + "!";

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

            var grabbable = GetComponent<Grabbable>();
            grabbable.DropItem(false, true);
            Destroy(gameObject);
        }

        public void setItem(HarvestDataTypes.Item newItem)
        {
            this.newItem = newItem;
        }
    }
}