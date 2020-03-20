using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBuilder : MonoBehaviour
{
    public Sprite ChildSprite;
    public Sprite AdultSprite;
    public Sprite ElderlySprite;

    public void UpdateRepresentation(Person person)
    {
        SpriteRenderer renderer = person.GetComponent<SpriteRenderer>();


        switch (person.ageGroup)
        {
            case (0): // kind
                renderer.sprite = ChildSprite;
                break;

            case (1): // erwachsener
                renderer.sprite = AdultSprite;
                break;

            case (2): // alter mensch
                renderer.sprite = ElderlySprite;
                break;

            default:
                break;
        }
    }
}
