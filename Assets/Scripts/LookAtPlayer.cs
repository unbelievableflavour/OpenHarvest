using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public bool UseLerp = true;
    public bool Stationary = true;
    public float Speed = 20f;
    public bool UseUpdate = false;
    public bool UseLateUpdate = true;

    Transform mainCam;

    void Start() {
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        if(!UseUpdate || !mainCam) {
            return;
        }

        lookAt();
    }

    void LateUpdate()
    {
        if (!UseLateUpdate || !mainCam) {
            return;
        }

        lookAt(); 
    }

    void lookAt()
    {
        if (UseLerp) {
            Quaternion rot = Quaternion.LookRotation(mainCam.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * Speed);
            return;
        }
        
        transform.LookAt(mainCam, transform.forward);
    }
}
