using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public enum State { normal, sickLigth, sickMedium, sickHard, healed }

    float Speed = 0.1f;
    public int Counter = 0;
    public Vector2 Position, Direction;
    public State CurState = Person.State.normal;
    public SpriteRenderer render;

    void Start()
    {
        Position = transform.position;
    }

    public void OnUpdate(bool bUpdateUnity)
    {
        //new Task
        if (Counter-- <= 0)
        {
            Direction = Random.insideUnitCircle.normalized * Speed;
            Counter = Random.Range(30, 50);
        }

        //normal update
        Position += Direction * 0.1f;

        //update sickness state
        for(int i=0; i<ServiceLocator.Instance.Spawner.Persons.Count; i++)
        {
            if(Vector2.Distance(Position, ServiceLocator.Instance.Spawner.Persons[i].Position) < ServiceLocator.Instance.InfectionRadius)
            {
                if(ServiceLocator.Instance.Spawner.Persons[i].CurState == State.sickMedium && Random.value < ServiceLocator.Instance.InfectionChance)
                {
                    CurState = State.sickMedium;
                }
            }
        }

        //deflect from wall
        if (Position.x < -ServiceLocator.Instance.xBounds || Position.x > ServiceLocator.Instance.xBounds)
            Direction = new Vector2(-Direction.x, Direction.y);
        if (Position.y < -ServiceLocator.Instance.yBounds || Position.y > ServiceLocator.Instance.yBounds)
            Direction = new Vector2(Direction.x, -Direction.y);

        //Update Unity
        if (bUpdateUnity)
        {
            transform.position = Position;

            if (CurState == State.normal) render.color = Color.gray;
            else if (CurState == State.sickMedium) render.color = Color.red;
        }
    }
}
