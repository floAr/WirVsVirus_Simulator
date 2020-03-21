using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : Place
{
    int _capacity;

    void Start()
    {
        _capacity = Capacity;
        base.Start();
    }

    private void FixedUpdate()
    {
        if (ServiceLocator.Instance.MoreHospitalCapacity)
            Capacity = _capacity * 2;
        else
            Capacity = _capacity;
    }

    public override void OnFinishMission(Person p)
    {
        p.isInfected = false;
        p.isImmune = true;
    }

    public override void OnStartMission(Person p)
    {
        p.isInfected = false;
        p.isImmune = true;
    }
}
