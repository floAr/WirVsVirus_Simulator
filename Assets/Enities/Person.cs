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
    public List<Mission> AvailableMissions = new List<Mission>();
    public Mission CurMission = null;
    public Vector2 CurMissionPosition;

    void Start()
    {
        ageGroup = Random.Range(0, 3);
        Position = transform.position;
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(House),
            Counter = Random.Range(50, 100),
            MaxCounter = 100,
            Duration = 20,
            MaxDuration = 20
        });

        Counter = Random.Range(20, 80);
        Direction = Random.onUnitSphere.normalized;
    }

    public void OnUpdate(bool bUpdateUnity)
    {
        //rethink task priorities
        foreach(Mission mission in AvailableMissions)
        {
            if(--mission.Counter <= 0)
            {
                CurMission = mission;
                CurMissionPosition = GetNearbyPlace(mission.Destination).transform.position;
            }
        }

        //normal update
        if (CurMission == null) // if nothing to do - just idle
        {
            if(--Counter <= 0)
            {
                Direction = Random.onUnitSphere.normalized;
                Counter = 30;
            }

            Position += Direction * 0.1f;
        }
        else // follow mission
        {
            Vector2 dir = CurMissionPosition - Position;

            if (dir.magnitude < 0.2f) //arrived at place
            {
                CurMission.Duration--;
            }
            else // goto place
            {
                Position += dir.normalized * 0.1f;
            }

            //mission finished?
            if (CurMission.Duration <= 0)
            {
                CurMission.Duration = CurMission.MaxDuration;
                CurMission.Counter = CurMission.MaxCounter;
                CurMission = null;
            }
        }

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

    Place GetNearbyPlace(System.Type placeType)
    {
        Place bestPlace = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < ServiceLocator.Instance.Spawner.Places.Count; i++)
        {
            if(ServiceLocator.Instance.Spawner.Places[i].GetType() == placeType && 
                Vector2.Distance(Position, ServiceLocator.Instance.Spawner.Places[i].Position) < bestDistance)
            {
                bestDistance = Vector2.Distance(Position, ServiceLocator.Instance.Spawner.Places[i].Position);
                bestPlace = ServiceLocator.Instance.Spawner.Places[i];
            }
        }

        return bestPlace;
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
