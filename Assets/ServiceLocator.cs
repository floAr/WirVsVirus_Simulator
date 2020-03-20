using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;
    public float xBounds = 8f;
    public float yBounds = 6f;
    public float InfectionRadius = 0.3f;
    public float InfectionChance = 0.02f;
    public float PersonSpeed = 0.1f;
    public Spawner Spawner;
    public PersonBuilder PersonBuilder;

    private void Awake()
    {
        Instance = this;
    }
}
