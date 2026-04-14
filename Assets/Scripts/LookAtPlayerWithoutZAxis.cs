using UnityEngine;

public class LookAtPlayerWithoutZAxis : MonoBehaviour
{
    public bool UseLerp = true;
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
            transform.rotation = Quaternion.Euler(new Vector3(0f, Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * Speed).eulerAngles.y, 0f));
        } else {
            transform.LookAt(mainCam, transform.forward);
        }
    }
}
