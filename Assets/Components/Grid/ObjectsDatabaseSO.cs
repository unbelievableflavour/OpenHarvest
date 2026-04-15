using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public List<string> unlockableIds { get; private set; } = new List<string>();
    [field: SerializeField]
    public GameObject prefab { get; private set; }
}

[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;
}
