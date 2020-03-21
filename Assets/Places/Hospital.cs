using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : Place
{
    public override void OnFinishMission(Person p)
    {
        p.isInfected = false;
        p.isImmune = true;
    }
}
