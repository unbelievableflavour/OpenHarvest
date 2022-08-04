using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaterial : MonoBehaviour
{
    public enum axis
    {
        y,
        x
    }

    public enum option
    {
        increase,
        decrease
    }
    public axis floatAxis = axis.y;
    public option direction = option.increase;

    float scrollSpeed = 0.5f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;

        if (floatAxis == axis.x)
        {
            rend.material.SetTextureOffset("_MainTex", new Vector2((direction == option.increase ? offset : -offset), 0));
            return;
        }

        rend.material.SetTextureOffset("_MainTex", new Vector2(0, (direction == option.increase ? offset : -offset)));
    }
}
