using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;
    public float xBounds = 8f;
    public float yBounds = 6f;
    public float InfectionRadius = 0.3f;
    public float BaseInfectionChance = 0.02f;
    public float InfectionChanceReductionPercent = 0.3f;
    public float PersonSpeed = 0.1f;
    public Spawner Spawner;
    public PersonBuilder PersonBuilder;
    public SimulationMaster SimMaster;
    public InfectionGraph InfectionGraph;
    public WebBridge WebBridge;

    public bool OptimizeBehaviour;

    public int Seed = 12345;

    public float InfectionChance
    {
        get
        {
            return OptimizeBehaviour ? BaseInfectionChance * InfectionChanceReductionPercent : BaseInfectionChance;
        }
    }



    private void Awake()
    {
        Instance = this;
        Random.InitState(Seed);
    }
}
