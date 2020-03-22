using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int InfectAtTimestep = 15;
    public int NumberOfPatienZeros = 5;
    private bool _infected = false;
    public Person PersonPrefab;
    public List<Person> Persons = new List<Person>();
    public List<Place> Places = new List<Place>();
    public int timeSteps = 1;
    public int maxTimesteps = 0;

    public int TotalPersonCount { get; private set; }

    private int _timestepCount = 0;


    void Start()
    {
        Spawn();
        Application.targetFrameRate = 30;
        ServiceLocator.Instance.SimMaster.OnUnityUpdate += SimMaster_UnityUpdate;
    }


    public void Spawn()
    {
        Places = new List<Place>(FindObjectsOfType<Place>());

        foreach (House house in FindObjectsOfType<House>())
        {
            //spawn persons
            for (int i = 0; i < house.Capacity; i++)
            {
                GameObject go = Instantiate<GameObject>(PersonPrefab.gameObject, house.transform.position, 
                    Quaternion.identity, transform);
                Persons.Add(go.GetComponent<Person>());
            }
            Persons[Persons.Count - 1].Id = Persons.Count - 1;
        }
        TotalPersonCount = Persons.Count;
        
    }



    private void SimMaster_UnityUpdate(object sender, System.EventArgs e)
    {
        if (maxTimesteps != 0 && _timestepCount > maxTimesteps)
        {
            ServiceLocator.Instance.SimMaster.enabled = false;
            return;
        }       

        for (int n = 0; n < timeSteps; n++)
        {
           
            _timestepCount++;

            if (_timestepCount == InfectAtTimestep)
            {
                for (int i = 0; i < NumberOfPatienZeros; i++)
                {
                    int r = Random.Range(0, Persons.Count);
                    Persons[r].infectionSeverity = Random.Range(0, 2);
                    Persons[r].Infect();
                }
            }

            for (int i = 0; i < Persons.Count; i++)
            {
                if (!Persons[i].isDead)
                    Persons[i].OnUpdate(true);
            }
        }
    }

}
