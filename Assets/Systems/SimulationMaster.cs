using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SimulationMasterTickEventArg : EventArgs
{
    public int TimeStep;
}

public class SimulationMaster : MonoBehaviour
{
    public bool isRunning;
    public int timeStep;

    public event EventHandler<SimulationMasterTickEventArg> OnUnityUpdate;


    public Button PlayButton;
    public Button PauseButton;

    private Spawner _spawnerRef;
    private WebBridge _webBridgeRef;

    private List<float[]> _dataPoints;

    private StringBuilder[] _dataChains;

    public void Run()
    {
        isRunning = true;
        PlayButton.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(true);
    }

    public void Pause()
    {
        isRunning = false;
        PlayButton.gameObject.SetActive(true);
        PauseButton.gameObject.SetActive(false);
    }



    public void GatherSimulationData()
    {
        var uninfected = 0;
        var sick_0 = 0;
        var sick_1 = 0;
        var sick_2 = 0;
        var dead = 0;
        var recovered = 0;

        for (int i = 0; i < _spawnerRef.Persons.Count; i++)
        {
            var p = _spawnerRef.Persons[i];
            if (p.isDead)
            {
                dead += 1;
                continue;
            }

            if (p.isImmune)
            {
                recovered += 1;
                continue;
            }

            if (p.isInfected)
            {
                switch (p.infectionSeverity)
                {
                    case 0:
                        sick_0 += 1;
                        break;
                    case 1:
                        sick_1 += 1;
                        break;
                    case 2:
                        sick_2 += 1;
                        break;
                    default:
                        break;
                }
                continue;
            }

            uninfected += 1;
        }
        var data = new float[] { uninfected/(float)_spawnerRef.Persons.Count, sick_0 / (float)_spawnerRef.Persons.Count, sick_1 / (float)_spawnerRef.Persons.Count, sick_2 / (float)_spawnerRef.Persons.Count, dead / (float)_spawnerRef.Persons.Count, recovered / (float)_spawnerRef.Persons.Count };
        Debug.Log($"[{data[0]},{data[1]},{data[2]},{data[3]},{data[4]},{data[5]}]");
        _dataPoints.Add(data);

        for (int i = 0; i < 6; i++)
        {
            if (_dataChains[i].Length == 0)
                _dataChains[i].Append($"[{data[i]}]");
            else
            {
                _dataChains[i].Remove(_dataChains[i].Length - 1, 1); // cut closing bracket
                _dataChains[i].Append("," + data[i] + "]");
            }
        }

        _webBridgeRef.EmitData("pop_data", DataChainsToJS());

    }

    private string DataChainsToJS()
    {

        return $"[{_dataChains[0].ToString()},{_dataChains[1].ToString()},{_dataChains[2].ToString()},{_dataChains[3].ToString()},{_dataChains[4].ToString()},{_dataChains[5].ToString()}]";
    }

    private string DataArrayToJS()
    {
        string result = "[";
        foreach (var series in _dataPoints)
        {
            result += $"[{series[0]},{series[1]},{series[2]},{series[3]},{series[4]},{series[5]}],";
        }
        result = result.Substring(0, result.Length - 1);
        result += "]";
        return result;
    }

    private void Start()
    {
        _spawnerRef = ServiceLocator.Instance.Spawner;
        _webBridgeRef = ServiceLocator.Instance.WebBridge;
        _dataPoints = new List<float[]>();
        _dataChains = new StringBuilder[6];
        for (int i = 0; i < 6; i++)
        {
            _dataChains[i] = new StringBuilder();
            _dataChains[i].Append("");
        }
        this.OnUnityUpdate += SimulationMaster_OnUnityUpdate;
    }

    private void SimulationMaster_OnUnityUpdate(object sender, SimulationMasterTickEventArg e)
    {
        GatherSimulationData();
    }

    private void Update()
    {
        if (!isRunning)
            return;
        timeStep += 1;
        OnUnityUpdate?.Invoke(this, new SimulationMasterTickEventArg()
        {
            TimeStep = timeStep
        }); ;
    }
}

