using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class PlaceSpawnCount
    {
        public Place PlacePrefab;
        public int Count = 1;
    }

    public int PersonCount = 10;
    public Person PersonPrefab;
    public List<PlaceSpawnCount> PlacesToSpawn = new List<PlaceSpawnCount>();
    public List<Person> Persons = new List<Person>();
    public List<Place> Places = new List<Place>();

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        float xBounds = 7f;
        float yBound = 5f;

        //Spawn places
        foreach(PlaceSpawnCount placeSpawnCount in PlacesToSpawn)
        {
            for (int i = 0; i < placeSpawnCount.Count; i++)
            {
                GameObject go = Instantiate<GameObject>(placeSpawnCount.PlacePrefab.gameObject, new Vector3(Random.Range(-xBounds, xBounds),
                    Random.Range(-yBound, yBound), 0f), Quaternion.identity, transform);
                Places.Add(go.GetComponent<Place>());
            }
        }

        //spawn persons
        for (int i = 0; i < PersonCount; i++)
        {
            GameObject go = Instantiate<GameObject>(PersonPrefab.gameObject, new Vector3(Random.Range(-xBounds, xBounds),
                Random.Range(-yBound, yBound), 0f), Quaternion.identity, transform);
            Persons.Add(go.GetComponent<Person>());
        }
        Persons[0].isInfected = true;
    }

    void Update()
    {
        for(int i=0; i<Persons.Count; i++)
        {
            Persons[i].OnUpdate(true);
        }
    }
}
