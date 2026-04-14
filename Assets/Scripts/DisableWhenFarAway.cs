using UnityEngine;
using UnityEngine.UI;

namespace BNG
{
    /// <summary>
    /// Shows a ring at the grab point of a grabbable if within a certain distance
    /// </summary>
    public class DisableWhenFarAway : MonoBehaviour
    {
        public float maxShowDistance = 5f;
        public GameObject gameObjectToDisable;

        bool shouldBeEnabled = true;
        Transform mainCam;

        // Start is called before the first frame update
        void Start()
        {
            mainCam = Camera.main.transform;
        }

        void Update()
        {
            float currentDistance = Vector3.Distance(transform.position, mainCam.position);
            UpdateActiveState(currentDistance <= maxShowDistance);
        }

        void UpdateActiveState(bool newState)
        {
            if(shouldBeEnabled == newState)
            {
                return;
            }
            shouldBeEnabled = newState;
            gameObjectToDisable.SetActive(shouldBeEnabled);
        }
    }
}

