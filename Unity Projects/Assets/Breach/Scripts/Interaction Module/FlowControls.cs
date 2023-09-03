using System;
using Breach;
using Breach.WildWaters.Scenarios;
using UnityEngine;
using UnityEngine.Assertions;

public class FlowControls : MonoBehaviour
{

    private SimulationManager simulationManager;

    public bool isSimulating=true;

    void Awake()
    {
        simulationManager = FindObjectOfType<SimulationManager>();
        if (simulationManager == null) throw new InvalidOperationException("No water SimulationManager in the scene. Please add the WaterSimulation-prefab to the scene");
       
        if (isSimulating)
            Play();

    }

       
    
    public void Play()
    {
        
        simulationManager.SetPlaying(true);
    }

    public void Pause()
    {
       
        simulationManager.SetPlaying(false);
    }

    public void Stop()
    {
        
        Pause();
        Reset();
    }

    private void Reset()
    {

        simulationManager.SetPlaying(false); // Pause the simulation
        simulationManager.AnimationFrame = 0; // Reset the animation frame to 
    }
 
}
