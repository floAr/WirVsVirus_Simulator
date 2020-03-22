using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBuilder : MonoBehaviour
{
    public Sprite ChildSprite;
    public Sprite AdultSprite;
    public Sprite ElderlySprite;

    public Sprite InfectedChildSprite;
    public Sprite InfectedAdultSprite;
    public Sprite InfectedElderlySprite;

    public Sprite DeadSprite;

    public Color HealthyColor;
    public Color SevereInfectionColor;
    public Color ImmuneColor;


    public void UpdateRepresentation(Person person)
    {
        SpriteRenderer renderer = person.GetComponent<SpriteRenderer>();

        if (person.isDead)
        {
            renderer.sprite = DeadSprite;
            renderer.color = Color.white;
            renderer.size = renderer.size * 3f;
            return;
        }
        
        // age based spriteshape
        switch (person.ageGroup)
        {
            case (0): // kind
                renderer.sprite = person.isInfected ? InfectedChildSprite : ChildSprite;
                break;

            case (1): // erwachsener
                renderer.sprite = person.isInfected ? InfectedAdultSprite : AdultSprite;
                break;

            case (2): // alter mensch
                renderer.sprite = person.isInfected ? InfectedElderlySprite : ElderlySprite;
                break;

            default:
                break;
        }

        renderer.color =
            person.isImmune ? ImmuneColor :
            !person.isInfected ? HealthyColor :
            Color.Lerp(HealthyColor, SevereInfectionColor, (person.infectionSeverity+1) / 3f);

    }
}
