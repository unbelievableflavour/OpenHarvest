using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public bool rotateLeft = true;
    public int speed = 1;
    public enum Axis {
        x,
        y,
        z 
    };
    public Axis rotationAxis = Axis.y;

    void Update()
    {
        if (rotateLeft)
        {
            if (rotationAxis == Axis.x)
            {
                this.transform.Rotate(speed * -Time.deltaTime, 0, 0, Space.Self);
                return;
            };

            if (rotationAxis == Axis.z)
            {
                this.transform.Rotate(0, 0, speed * -Time.deltaTime, Space.Self);
                return;
            };

            this.transform.Rotate(0, speed * -Time.deltaTime, 0, Space.Self);
            return;
        }

        if(rotationAxis == Axis.x) {
            this.transform.Rotate(speed * Time.deltaTime, 0, 0, Space.Self);
            return;
        };

        if (rotationAxis == Axis.z)
        {
            this.transform.Rotate(0, 0, speed * Time.deltaTime, Space.Self);
            return;
        };

        this.transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
    }
}
