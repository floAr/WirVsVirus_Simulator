using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int PersonCount = 10;
    public Person PersonPrefab;
    public List<Person> Persons = new List<Person>();

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        float xBounds = 7f;
        float yBound = 5f;

        for(int i=0; i<PersonCount; i++)
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
