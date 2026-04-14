using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    /// <summary>
    /// Rotate this object to point it's transform.forward at an object
    /// </summary>
    public class LookAtForShopKeeper : MonoBehaviour
    {

        /// <summary>
        /// The object to look at
        /// </summary>
        public Transform LookAt;
        public bool isLookingAtPlayer;


        void LateUpdate()
        {
            lookAt();
        }

        void lookAt()
        {

            if (LookAt != null)
            {

                transform.LookAt(LookAt, transform.forward);

                if (isLookingAtPlayer)
                {
                    transform.Rotate(-90f, 70f, 0f);
                    return;
                }
                transform.Rotate(-90f, 90f, 0f);
            }
        }
    }
}