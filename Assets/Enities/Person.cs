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
    public Place CurTargetPlace = null;

    public int InfectedBy = -1;

    void Start()
    {
      
        ageGroup = Random.Range(0, 3);

        int randInt = Random.Range(0, 10);
        if (randInt < 5) infectionSeverity = 0;
        else if (randInt < 8) infectionSeverity = 1;
        else infectionSeverity = 2;

        Position = transform.position;

        //Go to Hospital if sick
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(Hospital),
            Counter = 120,
            MaxCounter = 120,
            Duration = 50,
            MaxDuration = 50,
            IsApplicable = (p) => p.isInfected
        });

        //Go Home
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(House),
            SpecificPlace = GetNearbyPlace(typeof(House)),
            Counter = Random.Range(200, 300),
            MaxCounter = 300,
            Duration = 50,
            MaxDuration = 50
        });

        CurMission = null;
        Counter = Random.Range(20, 80);
        Direction = Random.onUnitSphere.normalized;
        ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);
    }

    public void OnUpdate(bool bUpdateUnity)
    {
        //rethink task priorities
        PlanTasks();

        //normal update
        if (CurMission == null) // if nothing to do - just idle
        {
            DoIdle();
        }
        else // follow mission
        {
            DoMission();
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

    private void PlanTasks()
    {
        foreach (Mission mission in AvailableMissions)
        {
            if (mission.IsApplicable(this))
            {
                if (--mission.Counter <= 0 && CurMission == null)
                {
                    StartMission(mission);
                }
            }
        }
    }

    private void StartMission(Mission mission)
    {
        CurMission = mission;

        if (mission.SpecificPlace != null)
        {
            CurTargetPlace = mission.SpecificPlace;
        }
        else
        {
            CurTargetPlace = GetNearbyPlace(mission.Destination);
        }

        CurTargetPlace.Capacity--;
    }

    private void DoMission()
    {
        Vector2 dir = CurTargetPlace.Position - Position;

        if (dir.magnitude < 0.2f) //arrived at place
        {
            CurMission.Duration--;
        }
        else // goto place
        {
            Position += dir.normalized * ServiceLocator.Instance.PersonSpeed;
        }

        //mission finished?
        if (CurMission.Duration <= 0)
        {
            FinishMission();
        }
    }

    private void FinishMission()
    {
        CurTargetPlace.Capacity++;
        CurTargetPlace.OnFinishMission(this);
        ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);

        CurMission.Duration = CurMission.MaxDuration;
        CurMission.Counter = CurMission.MaxCounter;
        CurTargetPlace = null;
        CurMission = null;
    }

    private void DoIdle()
    {
        if (--Counter <= 0)
        {
            Direction = Random.onUnitSphere.normalized;
            Counter = 30;
        }

        Position += Direction * ServiceLocator.Instance.PersonSpeed;
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
        if (isInfected || isImmune)
            return;

        for (int i = 0; i < ServiceLocator.Instance.Spawner.Persons.Count; i++)
        {
            if (Vector2.Distance(Position, ServiceLocator.Instance.Spawner.Persons[i].Position) < ServiceLocator.Instance.InfectionRadius)
            {
                if (ServiceLocator.Instance.Spawner.Persons[i].isInfected && Random.value < ServiceLocator.Instance.InfectionChance)
                {
                    isInfected = true;
                    InfectedBy = i;
                    ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);
                }
            }
        }
    }

    private void UpdateUnity()
    {      
        transform.position = Position;
    }

    private void HandleWallCollision()
    {
        if (Position.x < -ServiceLocator.Instance.xBounds || Position.x > ServiceLocator.Instance.xBounds)
            Direction = new Vector2(-Direction.x, Direction.y);
        if (Position.y < -ServiceLocator.Instance.yBounds || Position.y > ServiceLocator.Instance.yBounds)
            Direction = new Vector2(Direction.x, -Direction.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (InfectedBy != -1)
            Gizmos.DrawLine(this.transform.position, ServiceLocator.Instance.Spawner.Persons[InfectedBy].transform.position);
    }
}
