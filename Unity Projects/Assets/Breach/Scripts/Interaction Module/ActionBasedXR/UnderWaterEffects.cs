using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class UnderWaterEffects : MonoBehaviour
{
    [FormerlySerializedAs("GraphicsManager")] [SerializeField] private SimulationManager SimulationManager;
    [SerializeField] private VolumeProfile UnderwaterProfile;
    [SerializeField] private Volume UnderwaterVolume;

    [Tooltip("The midpoint is at water elevation height")]
    [SerializeField] private float FadeDistance = 0.2f;
    [Tooltip("Useful to set this to camera near plane")]
    [SerializeField] private float Offset = 0.3f;

    [SerializeField] private CharacterController PlayerBody;
    [Tooltip("Step stride distance (in meters)")]
    [SerializeField] private float StrideLength = 0.3f;
    [Tooltip("If it's been less time than this value between steps, use running sounds (in seconds)")]
    [SerializeField] private float RunningTimeBetweenSteps = 0.5f;
    [Tooltip("Footsteps volume")]
    [SerializeField] private float TerrainFootstepsVolume = 1f;


    private void Start()
    {
        SimulationManager??= FindObjectOfType<SimulationManager>();

        if (SimulationManager == null)
        {
            Debug.LogError("No Water SimulationManager found in scene. Make sure you have added the WaterSimulation prefab to your scene");
            gameObject.SetActive(false);
        }

        if (UnderwaterVolume is null)
        {
            var go = new GameObject("Underwater Volume");
            UnderwaterVolume = go.AddComponent<Volume>();
            UnderwaterVolume.profile = UnderwaterProfile;
        }      
    }

    private void FixedUpdate()
    {
        var waterHeight = SimulationManager.GetWaterHeightAt(transform.position);

        HandleUnderwaterVolume(waterHeight);
       
    }
    private void HandleUnderwaterVolume(float? waterHeight)
    {
        if (waterHeight.HasValue)
        {
            var weight = Mathf.Clamp01((1 / FadeDistance) * (waterHeight.Value - (transform.position.y - 0.5f * FadeDistance - Offset)));
            UnderwaterVolume.weight = weight;
            UnderwaterVolume.gameObject.SetActive(weight > 0);
        }
        else
            UnderwaterVolume.gameObject.SetActive(false);
    }

}
