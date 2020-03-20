using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    float Speed = 0.1f;
    public int Counter = 0;
    public Vector2 Position, Direction;
    public bool isInfected;
    public bool isImmune = false; 
    public int infectionSeverity = 0; // 0 = keine Sypthome 1 = Husten & Fieber 3 = Lungenentzündung
    public int ageGroup = 1; //0 = Kind 1 = Erwachsen 2 = Rentner
    public int deathCounter = 0;
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
        HandleSickness();

        //deflect from wall
        HandleWallCollision();

        //Update Unity
        if (bUpdateUnity)
        {
            UpdateUnity();
        }
    }

    private void HandleSickness()
    {
        for (int i = 0; i < ServiceLocator.Instance.Spawner.Persons.Count; i++)
        {
            if (Vector2.Distance(Position, ServiceLocator.Instance.Spawner.Persons[i].Position) < ServiceLocator.Instance.InfectionRadius)
            {
                if (ServiceLocator.Instance.Spawner.Persons[i].isInfected && Random.value < ServiceLocator.Instance.InfectionChance)
                {
                    isInfected = true;
                }
            }
        }
    }

    private void UpdateUnity()
    {
        transform.position = Position;

        if (!isInfected) render.color = Color.gray;
        else if (isInfected) render.color = Color.red;
    }

    private void HandleWallCollision()
    {
        if (Position.x < -ServiceLocator.Instance.xBounds || Position.x > ServiceLocator.Instance.xBounds)
            Direction = new Vector2(-Direction.x, Direction.y);
        if (Position.y < -ServiceLocator.Instance.yBounds || Position.y > ServiceLocator.Instance.yBounds)
            Direction = new Vector2(Direction.x, -Direction.y);
    }
}
