using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public int Id;
    float Speed = 0.1f;
    public int Counter = 0;
    public Vector2 Position, Direction;
    public bool isInfected;
    public bool isImmune = false;
    public bool isDead = false;
    public int sicknessCounter = 360;
    public int deathCounter = 150;
    public int infectionSeverity = 0; // 0 = keine Sypthome 1 = Husten & Fieber 3 = Lungenentzündung
    public int ageGroup = 1; //0 = Kind 1 = Erwachsen 2 = Rentner
    public SpriteRenderer render;
    public List<Mission> AvailableMissions = new List<Mission>();
    public Mission CurMission = null;
    public Place CurTargetPlace = null;
    public Place CurPlace = null;

    public int InfectedBy = -1;


    void Awake()
    {
        //age
        int randInt = Random.Range(0, 10);
        if (randInt < 3) ageGroup = 0;
        else if (randInt < 7) ageGroup = 1;
        else ageGroup = 2;

        //severity
        randInt = Random.Range(0, 10);
        if (randInt < 5) infectionSeverity = 0;
        else if (randInt < 8) infectionSeverity = 1;
        else infectionSeverity = 2;

        Position = transform.position;

        CreateTasks();

        CurMission = null;
        Counter = Random.Range(20, 80);
        Direction = Random.onUnitSphere.normalized;
        ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);
    }

    private void CreateTasks()
    {
        //Go to Hospital if severly sick
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(Hospital),
            Counter = 30,
            MaxCounter = 30,
            Duration = 120,
            MaxDuration = 120,
            IsApplicable = (p) => p.isInfected && p.infectionSeverity == 2
        });

        //Go to Hospital a little later if midly sick
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(Hospital),
            Counter = 180,
            MaxCounter = 180,
            Duration = 60,
            MaxDuration = 60,
            IsApplicable = (p) => p.isInfected && p.infectionSeverity == 1
        });

        //all go to hospital when they are tested positive for corona
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(Hospital),
            Counter = 180,
            MaxCounter = 180,
            Duration = 60,
            MaxDuration = 60,
            IsApplicable = (p) => p.isInfected && ServiceLocator.Instance.CoronaTests
        });

        //Do selfquarantaine
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(House),
            Counter = 30,
            MaxCounter = 30,
            Duration = 120,
            MaxDuration = 120,
            IsApplicable = (p) => p.isInfected && p.infectionSeverity == 0
        });
     
        //Go to work
        if (ageGroup == 1)
            AvailableMissions.Add(new Mission()
            {
                Destination = typeof(Workplace),
                Counter = Random.Range(200, 300),
                MaxCounter = 300,
                Duration = 120,
                MaxDuration = 120,
                IsApplicable = (p) => !ServiceLocator.Instance.HomeOffice
            });

        //Do HomeOffice
        if (ageGroup == 1)
            AvailableMissions.Add(new Mission()
            {
                Destination = typeof(House),
                SpecificPlace = GetNearbyPlace(typeof(House)),
                Counter = Random.Range(200, 300),
                MaxCounter = 300,
                Duration = 120,
                MaxDuration = 120,
                IsApplicable = (p) => ServiceLocator.Instance.HomeOffice
            });

        //Go to School
        if (ageGroup == 0)
            AvailableMissions.Add(new Mission()
            {
                Destination = typeof(School),
                Counter = Random.Range(200, 300),
                MaxCounter = 300,
                Duration = 120,
                MaxDuration = 120,
                IsApplicable = (p) => !ServiceLocator.Instance.CloseSchools
            });

        //Do Home schooling
        if (ageGroup == 0)
            AvailableMissions.Add(new Mission()
            {
                Destination = typeof(House),
                SpecificPlace = GetNearbyPlace(typeof(House)),
                Counter = Random.Range(200, 300),
                MaxCounter = 300,
                Duration = 120,
                MaxDuration = 120,
                IsApplicable = (p) => ServiceLocator.Instance.CloseSchools
            });

        //parents also need to stay at home
        if (ageGroup == 1)
            AvailableMissions.Add(new Mission()
            {
                Destination = typeof(House),
                SpecificPlace = GetNearbyPlace(typeof(House)),
                Counter = Random.Range(200, 300),
                MaxCounter = 300,
                Duration = 60,
                MaxDuration = 60,
                IsApplicable = (p) => ServiceLocator.Instance.CloseSchools
            });

        //Go to a restaurant
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(Freetime),
            SpecificPlace = GetNearbyPlace(typeof(Freetime)),
            Counter = Random.Range(300, 1000),
            MaxCounter = 900,
            Duration = 50,
            MaxDuration = 50,
            IsApplicable = (p) => !ServiceLocator.Instance.CloseRestaurants
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
    }

    private void DoMission()
    {
        Vector2 dir = CurTargetPlace.Position - Position;

        if (dir.magnitude < 0.2f) //arrived at place
        {
            if (CurPlace == null) //wait for capacity
            {
                if(CurTargetPlace.Capacity > 0)
                {
                    CurPlace = CurTargetPlace;
                    CurPlace.OnStartMission(this);
                    CurPlace.Capacity--;
                }
            }
            else //wait for treatment
            {
                CurMission.Duration--;
            }
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
        if (CurPlace != null)
        {
            CurTargetPlace.Capacity++;
            CurTargetPlace.OnFinishMission(this);
        }
        ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);

        CurMission.Duration = CurMission.MaxDuration;
        CurMission.Counter = CurMission.MaxCounter;
        CurTargetPlace = null;
        CurPlace = null;
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
        if(isInfected)
        {
            sicknessCounter--;
            if(infectionSeverity == 2) deathCounter--;

            if (deathCounter < 0)
            {
                Die();
            }

            if (sicknessCounter < 0)
            {
                isInfected = false;
                isImmune = true;
                ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);
            }
        }

        if (isInfected || isImmune)
            return;

        float infectionChance = ServiceLocator.Instance.InfectionChance * (ServiceLocator.Instance.WashYourHands ? 0.7f : 1f);
        Vector2 evasionVector = Vector2.zero;

        for (int i = 0; i < ServiceLocator.Instance.Spawner.Persons.Count; i++)
        {
            float distance = Vector2.Distance(Position, ServiceLocator.Instance.Spawner.Persons[i].Position);

            //social distancing
            if(distance < ServiceLocator.Instance.InfectionRadius * 2)
            {
                evasionVector += (Position - ServiceLocator.Instance.Spawner.Persons[i].Position);
            }

            //infection
            if (distance < ServiceLocator.Instance.InfectionRadius)
            {
                if (ServiceLocator.Instance.Spawner.Persons[i].isInfected && Random.value < infectionChance)
                {
                    isInfected = true;
                    InfectedBy = i;
                    ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);
                    ServiceLocator.Instance.InfectionGraph.AddInfectedPerson(this);
                }
            }
        }

        //apply social distancing
        Position += evasionVector.normalized * ServiceLocator.Instance.PersonSpeed / 2f;
    }

    private void Die()
    {
        if (CurMission != null) FinishMission();

        isDead = true;
        ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);
        enabled = false;
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
