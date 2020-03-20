using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    public Vector2 Position;
    public int Capacity;

    public void Start()
    {
        Position = transform.position;
    }
}
