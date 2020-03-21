using System;
using System.Collections;
using System.Collections.Generic;
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

        _webBridgeRef.EmitData("population",$"{{ {uninfected}, {sick_0}, {sick_1}, {sick_2}, {recovered}, {dead} }}");

    }

    private void Start()
    {
        _spawnerRef = ServiceLocator.Instance.Spawner;
        _webBridgeRef = ServiceLocator.Instance.WebBridge;
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

