using System;
using System.Runtime.Serialization;

[Serializable]
public class SaveablePlacedObject
{
    public string objectId;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;

    [OptionalField]
    public int? parentObjectIndex;
}
