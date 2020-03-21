using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int PersonsPerHouse = 4;
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
            for (int i = 0; i < PersonsPerHouse; i++)
            {
                GameObject go = Instantiate<GameObject>(PersonPrefab.gameObject, house.transform.position, 
                    Quaternion.identity, transform);
                Persons.Add(go.GetComponent<Person>());
            }
            Persons[Persons.Count - 1].Id = Persons.Count - 1;
        }

        for (int i = 0; i < 8; i++)
        {
            int r = Random.Range(0, Persons.Count);
            Persons[r].infectionSeverity = 0;
            Persons[r].isInfected = true;
            ServiceLocator.Instance.PersonBuilder.UpdateRepresentation(Persons[r]);
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
            for (int i = 0; i < Persons.Count; i++)
            {
                if (!Persons[i].isDead)
                    Persons[i].OnUpdate(true);
            }
        }
    }

}
