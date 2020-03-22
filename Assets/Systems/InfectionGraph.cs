using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InfectionGraph : MonoBehaviour
{
    // Start is called before the first frame update
    //public LineRenderer GraphRenderer;
    //private Dictionary<int, List<int>> _infectionGraph;
    private Spawner _spawnerRef;
    //private List<Transform> _linearGraph;

    public Material glMat;
    public List<Person> _persons;
    public List<Vector3> _infectionSpots;

    private void Start()
    {
        //_infectionGraph = new Dictionary<int, List<int>>();
        //_linearGraph = new List<Transform>();
        _spawnerRef = ServiceLocator.Instance.Spawner;
        _persons = new List<Person>();
        _infectionSpots = new List<Vector3>();
    }

    //private void Update()
    //{
    //    GraphRenderer.positionCount = _linearGraph.Count;
    //    GraphRenderer.SetPositions((from t in _linearGraph select t.position).ToArray());
    //}

    public void AddInfectedPerson(Person p)
    {
        if (p.InfectedBy != -1)
        {
            _persons.Add(p);
            //if (!_infectionGraph.ContainsKey(p.InfectedBy))
            //    _infectionGraph.Add(p.InfectedBy, new List<int>());
            //_infectionGraph[p.InfectedBy].Add(p.Id);
        }

        //  _linearGraph.Clear();
        //  foreach (var item in _infectionGraph.Keys)
        //  {
        //      _linearGraph = TraverseGraph(_linearGraph, item);
        //  }
    }

    //private List<Transform> TraverseGraph(List<Transform> points, int current)
    //{
    //    if (_infectionGraph.ContainsKey(current)) // we have more links from this
    //    {
    //        foreach (var item in _infectionGraph[current])
    //        {
    //            points.Add(_spawnerRef.Persons[current].transform);
    //            points = TraverseGraph(points, item);
    //        }
    //        return points;
    //    }
    //    else // we reached an end node
    //    {
    //        points.Add(_spawnerRef.Persons[current].transform);
    //        return points;
    //    }
    //}

    void OnPostRender()
    {
        if (!glMat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }

        GL.PushMatrix();
        glMat.SetPass(0);

        GL.LoadIdentity();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        foreach (var item in _persons)
        {
            GL.Vertex(item.transform.position);
            GL.Vertex(_spawnerRef.Persons[item.InfectedBy].transform.position);
        }
        GL.End();

        GL.PopMatrix();
    }

    public void RegisterInfection(Vector3 pos)
    {
        _infectionSpots.Add(pos);
    }
}
