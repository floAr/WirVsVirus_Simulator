using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    public Vector2 Position;
    public int Capacity = -1;

    public void Start()
    {
        Position = transform.position;
    }

    public virtual void OnStartMission(Person p) { }

    public virtual void OnFinishMission(Person p) { }
}
