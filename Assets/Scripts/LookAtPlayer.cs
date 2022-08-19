using UnityEngine;

namespace BNG
{
    /// <summary>
    /// Rotate this object to point it's transform.forward at an object
    /// </summary>
    public class LookAtPlayer : MonoBehaviour
    {
        /// <summary>
        /// If true will Slerp to the object. If false will use transform.LookAt
        /// </summary>
        public bool UseLerp = true;

        /// <summary>
        /// Slerp speed if UseLerp is true
        /// </summary>
        public float Speed = 20f;

        public bool UseUpdate = false;
        public bool UseLateUpdate = true;
        public Transform cameraPosition;

        void Update()
        {
            if (cameraPosition && UseUpdate)
            {
                lookAt();
                return;
            }
            if (!cameraPosition && GameState.Instance.currentPlayerPosition != null)
            {
                cameraPosition = GameState.Instance.currentPlayerPosition.Find("CameraRig/TrackingSpace/CenterEyeAnchor").transform;
            }
        }

        void LateUpdate()
        {
            if (cameraPosition && UseLateUpdate)
            {
                lookAt();
                return;
            }
        }

        void lookAt()
        {

            if (GameState.Instance.currentPlayerPosition != null)
            {
                if (UseLerp)
                {
                    Quaternion rot = Quaternion.LookRotation(cameraPosition.position - transform.position);

                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * Speed);
                }
                else
                {
                    transform.LookAt(cameraPosition, transform.forward);
                }
            }
        }
    }
}
