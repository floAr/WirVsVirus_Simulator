using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graveyard : MonoBehaviour
{
    public int HeadCount;
    public int X, Y;
    public float StepSize = 0.1f;

    public void MoveToGraveyard(Person p)
    {
        var x = HeadCount % X;
        var y = HeadCount / X;

        HeadCount += 1;
        StartCoroutine(LerpToGrave(p.gameObject, this.transform.position + new Vector3(x * StepSize, y * StepSize, 0)));



    }

    IEnumerator LerpToGrave(GameObject p, Vector3 graveSpot)
    {
        while (Vector3.Distance(p.transform.position, graveSpot) > 0.01f)
        {
            yield return new WaitForEndOfFrame();
            p.transform.position = Vector3.Lerp(p.transform.position, graveSpot, 0.1f);
        }
    }

}
