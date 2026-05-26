using UnityEngine;

namespace BNG {
    public class PlaySoundOnRelease : GrabbableEvents {

        public AudioClip SoundToPlay;

        public override void OnRelease()
        {

            // Play Sound
            if (SoundToPlay)
            {
                VRUtils.Instance.PlaySpatialClipAt(SoundToPlay, transform.position, 1f, 1f);
            }

            base.OnRelease();
        }
    }
}
