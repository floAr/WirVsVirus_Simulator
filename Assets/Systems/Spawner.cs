using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int PersonsPerHouse = 4;
    public Person PersonPrefab;
    public List<Person> Persons = new List<Person>();
    public List<Place> Places = new List<Place>();

    void Start()
    {
        Spawn();
        Application.targetFrameRate = 30;
        ServiceLocator.Instance.SimMaster.UnityUpdate += SimMaster_UnityUpdate;
    }


    public void Spawn()
    {
        foreach (House house in FindObjectsOfType<House>())
        {
            //spawn persons
            for (int i = 0; i < PersonsPerHouse; i++)
            {
                GameObject go = Instantiate<GameObject>(PersonPrefab.gameObject, house.transform.position, 
                    Quaternion.identity, transform);
                Persons.Add(go.GetComponent<Person>());
            }
        }
        Persons[0].isInfected = true;
    }


    private void SimMaster_UnityUpdate(object sender, System.EventArgs e)
    {
        for (int i = 0; i < Persons.Count; i++)
        {
            Persons[i].OnUpdate(true);
        }
    }

}
