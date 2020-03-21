using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : Place
{
    int _capacity;
    bool moreHospitalsOn = false;

    void Start()
    {
        _capacity = Capacity;
        base.Start();
    }

    private void FixedUpdate()
    {
        if (ServiceLocator.Instance.MoreHospitalCapacity != moreHospitalsOn)
        {
            moreHospitalsOn = ServiceLocator.Instance.MoreHospitalCapacity;
            if (moreHospitalsOn) Capacity += _capacity;
            else Capacity -= _capacity;
        }
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
