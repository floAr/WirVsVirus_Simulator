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
    public int sicknessCounter = 960;
    public int deathCounter = 480;
    public int infectionSeverity = 0; // 0 = keine Sypthome 1 = Husten & Fieber 3 = Lungenentzündung
    public int ageGroup = 1; //0 = Kind 1 = Erwachsen 2 = Rentner
    public SpriteRenderer render;
    public List<Mission> AvailableMissions = new List<Mission>();
    public Mission CurMission = null;
    public Place CurTargetPlace = null;
    public Place CurPlace = null;

    public ParticleSystem InfectionSystem;

    public int InfectedBy = -1;


    void Awake()
    {
        //age
        int randInt = Random.Range(0, 12);
        if (randInt < 3) ageGroup = 0;
        else if (randInt < 9) ageGroup = 1;
        else ageGroup = 2;

        //severity
        randInt = Random.Range(0, 10);
        if (randInt < 5) infectionSeverity = 0;
        else if (randInt < 8) infectionSeverity = 1;
        else infectionSeverity = 2;
    }

    void Start()
    { 
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
            Delay = 30,
            MaxDelay = 30,
            Duration = 100,
            MaxDuration = 100,
            IsApplicable = (p) => p.isInfected && p.infectionSeverity == 2
        });

        //Go to Home a little later if midly sick // todo ? balance death rate
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(House),
            Delay = 180,
            MaxDelay = 180,
            Duration = 360,
            MaxDuration = 360,
            IsApplicable = (p) => p.isInfected && p.infectionSeverity == 1 && ServiceLocator.Instance.SelfQuarantaine
        });

        //all other go to self quaratine if we have tests 
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(House),
            Delay = 30,
            MaxDelay = 30,
            Duration = 180,
            MaxDuration = 180,
            IsApplicable = (p) => p.isInfected && ServiceLocator.Instance.CoronaTests
        });
     
        //Go to work
        if (ageGroup == 1)
            AvailableMissions.Add(new Mission()
            {
                Destination = typeof(Workplace),
                Delay = Random.Range(200, 500),
                MaxDelay = 500,
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
                Delay = Random.Range(200, 500),
                MaxDelay = 500,
                Duration = 120,
                MaxDuration = 120,
                IsApplicable = (p) => ServiceLocator.Instance.HomeOffice
            });

        //Go to School
        if (ageGroup == 0)
            AvailableMissions.Add(new Mission()
            {
                Destination = typeof(School),
                Delay = Random.Range(200, 500),
                MaxDelay = 400,
                Duration = 90,
                MaxDuration = 90,
                IsApplicable = (p) => !ServiceLocator.Instance.CloseSchools
            });

        //Do Home schooling
        if (ageGroup == 0)
            AvailableMissions.Add(new Mission()
            {
                Destination = typeof(House),
                SpecificPlace = GetNearbyPlace(typeof(House)),
                Delay = Random.Range(200, 500),
                MaxDelay = 500,
                Duration = 90,
                MaxDuration = 90,
                IsApplicable = (p) => ServiceLocator.Instance.CloseSchools
            });

        //parents also need to stay at home
        if (ageGroup == 1)
            AvailableMissions.Add(new Mission()
            {
                Destination = typeof(House),
                SpecificPlace = GetNearbyPlace(typeof(House)),
                Delay = Random.Range(200, 500),
                MaxDelay = 500,
                Duration = 90,
                MaxDuration = 90,
                IsApplicable = (p) => ServiceLocator.Instance.CloseSchools
            });

        //Go to a restaurant
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(Freetime),
            //SpecificPlace = GetNearbyPlace(typeof(Freetime)),
            Delay = Random.Range(300, 1000),
            MaxDelay = 1000,
            Duration = 50,
            MaxDuration = 50,
            IsApplicable = (p) => !ServiceLocator.Instance.CloseRestaurants
        });

        //Stay at Home // todo add numerical values
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(House),
            SpecificPlace = GetNearbyPlace(typeof(House)),
            Delay = Random.Range(0, 30),
            MaxDelay = 30,
            Duration = 100,
            MaxDuration = 100,
            IsApplicable = (p) => ServiceLocator.Instance.StayAtHome
        });

        //Go Home
        AvailableMissions.Add(new Mission()
        {
            Destination = typeof(House),
            SpecificPlace = GetNearbyPlace(typeof(House)),
            Delay = Random.Range(300, 500),
            MaxDelay = 500,
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
                if (--mission.Delay <= 0 && CurMission == null)
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
        CurMission.Delay = CurMission.MaxDelay;
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

        float infectionChance = ServiceLocator.Instance.InfectionChance * (ServiceLocator.Instance.SocialDistancing ? 0.7f : 1f);
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
                    InfectedBy = i;
                    Infect();
                }
            }
        }

        //apply social distancing
        if(CurMission == null && ServiceLocator.Instance.SocialDistancing)
            Position += evasionVector.normalized * ServiceLocator.Instance.PersonSpeed * 0.8f;
    }

    private void Die()
    {
        if (CurMission != null) FinishMission();

        isDead = true;
        ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);

        ServiceLocator.Instance.Graveyard.MoveToGraveyard(this);

        enabled = false;
    }


    public void Infect()
    {
        isInfected = true;
        ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(this);
        ServiceLocator.Instance.InfectionGraph.AddInfectedPerson(this);
        InfectionSystem.Play();

        // TODO 
        ServiceLocator.Instance.InfectionGraph.RegisterInfection(this.transform.position);
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
