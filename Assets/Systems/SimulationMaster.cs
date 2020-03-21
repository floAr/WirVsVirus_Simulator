using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XCharts;

public class SimulationMasterTickEventArg : EventArgs
{
    public int TimeStep;
}

public class SimulationMaster : MonoBehaviour
{
    public bool isRunning;
    public int timeStep;
    public LineChart chart;

    private int _lastFrame = 0;

    public event EventHandler<SimulationMasterTickEventArg> OnUnityUpdate;


    public Button PlayButton;
    public Button PauseButton;

    private Spawner _spawnerRef;
    private WebBridge _webBridgeRef;


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
        var sick = 0;
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
                sick += 1;
                continue;
            }

            uninfected += 1;
        }
        var data = new float[] { uninfected/(float)_spawnerRef.Persons.Count, sick / (float)_spawnerRef.Persons.Count, dead / (float)_spawnerRef.Persons.Count, recovered / (float)_spawnerRef.Persons.Count };
        Debug.Log($"[{data[0]},{data[1]},{data[2]},{data[3]}]");


        chart.AddData(0, sick);
        chart.AddData(1, recovered);
        chart.AddData(2, dead);

        for (int i = 0; i < _dataChains.Length; i++)
        {
            if (_dataChains[i].Length == 0)
                _dataChains[i].Append($"[{(data[i]*100f).ToString(CultureInfo.InvariantCulture)}]");
            else
            {
                _dataChains[i].Remove(_dataChains[i].Length - 1, 1); // cut closing bracket
                _dataChains[i].Append("," + (data[i]*100f).ToString(CultureInfo.InvariantCulture) + "]");
            }
        }

        _webBridgeRef.EmitData("pop_data", DataChainsToJS());

    }

    private string DataChainsToJS()
    {

        return $"[{_dataChains[0].ToString()},{_dataChains[1].ToString()},{_dataChains[2].ToString()},{_dataChains[3].ToString()}]";
    }

    private void Start()
    {
        _spawnerRef = ServiceLocator.Instance.Spawner;
        _webBridgeRef = ServiceLocator.Instance.WebBridge;
        _dataChains = new StringBuilder[4];
        for (int i = 0; i < 4; i++)
        {
            _dataChains[i] = new StringBuilder();
            
            _dataChains[i].Append("");
        }
        this.OnUnityUpdate += SimulationMaster_OnUnityUpdate;
    }

    private void SimulationMaster_OnUnityUpdate(object sender, SimulationMasterTickEventArg e)
    {
        if (_lastFrame + 10 < Time.frameCount)
        {
            _lastFrame = Time.frameCount;
            GatherSimulationData();
        }
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

