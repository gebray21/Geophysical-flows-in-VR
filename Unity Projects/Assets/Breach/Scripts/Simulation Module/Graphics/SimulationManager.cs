﻿using System;
using Breach;
using Breach.WildWaters.Scenarios;
using UnityEngine;
using UnityEngine.Assertions;


public class SimulationManager : MonoBehaviour
{
    /// <summary>
    /// Defines the simulation and its various scenarios
    /// </summary>
    [SerializeField] private ScenarioConfig config;

    // Higher speed = faster animiation
    [SerializeField] private float playbackSpeed = 1f;
    [SerializeField] private float delayTime = 10f;

    /// <summary>
    /// Sets if the simulation should be playing or paused
    /// </summary>
    public void SetPlaying(bool playing) => this.playing = playing;
    private const float frameTime = 1f; // 1 second simulation time

    // Animation frame changed event
    public static event EventHandler<int> OnFrameChanged;

    private int animationFrame = 0;
    private float animationTime = 0f;
    public float AnimationTime => animationTime;

    private float realTime = 0f;
    public bool CurrentFrameValid => currentFrameData != null;
    public bool NextFrameValid => currentFrameData != null;
    //river data


    /// <summary>
    /// Contains the data for the frame
    /// </summary>
    private MapInfo currentFrameData, nextFrameData;

    /// <summary>
    /// Contains the mesh of the frame-data
    /// </summary>
    private GameObject currentFrameMesh, nextFrameMesh;
    private bool playing = false;
    private bool creatingMesh = false;
    private ScenarioVisibility[] scenarioVisibility;
    private Scenario activeScenario;
    private int lastFrameIndex;
    private float time = 0;

    #region MonoBehaviour

    private void Awake()
    {
        scenarioVisibility = FindObjectsOfType<ScenarioVisibility>(includeInactive: true);
    }

    private void Start()
    {
        Assert.IsTrue(config.SimulationVariants.Length > 0, "Unable to run flow simulation as there are no simulations configured");
        Assert.IsTrue(config.SimulationVariants.Length < 5, "Application does not currently support more than 5 simulation-variations");

        SetActiveSimulation(0);
        lastFrameIndex = BinaryImporter.GetNumFrames() - 1;

    }

    private void Destroy()
    {
        BinaryImporter.StopThread();
    }

    private void Update()
    {
        time += Time.deltaTime;
        // if (playing)
        if (time > delayTime)  //if(time > delayTime)
            AnimateFlow();


        /*
        if (animationFrame <= lastFrameIndex - 1)
            playing = true;       

        if (playing)
        {
            animationTime += Time.deltaTime * (playbackSpeed / frameTime);
            realTime = animationTime;
            animationTime = Mathf.Clamp(animationTime, 0f, 1f);

            if (!creatingMesh)
            {
                if (!currentFrameMesh)
                {
                    if (realTime >= delayTime)
                    {
                        currentFrameMesh = LoadFrame();
                    }
                    currentFrameData = BinaryImporter.GetFrame(animationFrame);
                    nextFrameData = BinaryImporter.GetFrame(animationFrame % BinaryImporter.GetNumFrames() - 1);
                }
                else if (!nextFrameMesh)
                {
                    animationFrame++;

                    nextFrameMesh = LoadFrame();
                    if (nextFrameMesh == null) // not loaded yet!
                        animationFrame--;
                    else
                    {
                        nextFrameMesh.SetActive(false);
                        animationFrame %= BinaryImporter.GetNumFrames() - 1;
                    }
                }
                else if (animationTime >= 1f && nextFrameMesh != null)
                {
                    Destroy(currentFrameMesh);
                    currentFrameMesh = nextFrameMesh;
                    currentFrameMesh.SetActive(true);
                    nextFrameMesh = null;
                    currentFrameData = BinaryImporter.GetFrame(animationFrame);
                    nextFrameData = BinaryImporter.GetFrame(animationFrame % BinaryImporter.GetNumFrames() - 1);
                    animationTime = 0f;
                    OnFrameChanged?.Invoke(this, AnimationFrame);
                }
            }
            Shader.SetGlobalFloat("_AnimTime", animationTime);
            //playing = false;
        }
        */

    }

    #endregion

    public void SetActiveSimulation(int index)
    {
        activeScenario = (Scenario)index;
        Log.I($"Current active scenario: {activeScenario}");

        BinaryImporter.SetDataPath(config.SimulationVariants[index].Path);
        AnimationFrame = AnimationFrame;

        // Set the visibility accordingly
        var flag = ScenarioConversions.GetFlag(activeScenario);
        foreach (var vis in scenarioVisibility)
        {
            vis.SetVisibility(flag);
        }
    }

    // Set to 0 to go back to start
    public int AnimationFrame
    {
        get => animationFrame;
        set
        {
            if (value >= BinaryImporter.GetNumFrames() - 1)
                return;
            //if (currentFrameMesh != null)
            //    Destroy(currentFrameMesh);
            if (nextFrameMesh != null)
                Destroy(nextFrameMesh);

            //CancelGenerator?.Invoke();
            animationFrame = value % (BinaryImporter.GetNumFrames() - 1);
            animationTime = 1f;
            BinaryImporter.EnqueueFrame(animationFrame);
            BinaryImporter.EnqueueFrame((animationFrame + 1) % (BinaryImporter.GetNumFrames() - 1));
            BinaryImporter.EnqueueFrame((animationFrame + 2) % (BinaryImporter.GetNumFrames() - 1));
        }
    }


    public float? GetWaterHeightAt(Vector3 pos)
    {
        if (currentFrameData == null || nextFrameData == null) return null;

        var pos2d = new Vector2(pos.x, Mathf.Abs(pos.z));
        pos2d.Scale(new Vector2(1f / currentFrameData.cellSize, 1f / currentFrameData.cellSize));

        return GenFunctions.SampleInterpolatedMatrix(
            currentFrameData.myMapData.Elevation,
            nextFrameData.myMapData.Elevation,
            animationTime,
            pos2d
        );
    }



    public Vector2? GetWaterVelocity(Vector3 pos)
    {
        if (currentFrameData is null || nextFrameData is null) return null;

        var pos2d = new Vector2(pos.x, Mathf.Abs(pos.z));
        pos2d.Scale(new Vector2(1f / currentFrameData.cellSize, 1f / currentFrameData.cellSize));

        var velX = GenFunctions.SampleInterpolatedMatrix(
            currentFrameData.myMapData.Velx,
            nextFrameData.myMapData.Velx,
            animationTime,
            pos2d
        );
        var velY = GenFunctions.SampleInterpolatedMatrix(
            currentFrameData.myMapData.Vely,
            nextFrameData.myMapData.Vely,
            animationTime,
            pos2d
        );

        return new Vector2(velX, velY);
    }

    public Vector3? GetWaterNormal(Vector3 pos)
    {
        if (currentFrameData is null || nextFrameData is null) return null;

        var pos2d = new Vector2(pos.x, Mathf.Abs(pos.z));
        pos2d.Scale(new Vector2(1f / currentFrameData.cellSize, 1f / currentFrameData.cellSize));

        var normalOS = GenFunctions.SampleInterpolatedVectorArray(
            currentFrameData.myMapData.Normal,
            nextFrameData.myMapData.Normal,
            animationTime,
            pos2d
        );

        return new Vector3(normalOS.x, normalOS.z, normalOS.y);
    }


    private GameObject LoadFrame()
    {
        int i = animationFrame;

        var frame0 = BinaryImporter.GetFrame(i);
        var frame1 = BinaryImporter.GetFrame(i + 1);
        if (frame0 == null || frame1 == null)
            return null;

        GameObject aReading = new GameObject("Reading_" + i.ToString());
        aReading.transform.parent = transform;

        var generatorGO = new GameObject("PointCloudGenerator");
        generatorGO.transform.SetParent(aReading.transform);
        var generator = generatorGO.AddComponent<PointCloudGenerator>();
        if (generator == null)
        {
            Debug.LogError("PC Generator prefab is missing PC component");
            return null;
        }

        StartCoroutine(generator.prepareAndCreate(frame0.myMapData, frame1.myMapData, null, "Water", config.SimulationMaterial, new Vector3(frame0.cellSize, frame0.cellSize, 1), (_) => { creatingMesh = false; }));
        creatingMesh = true;

        BinaryImporter.EnqueueFrame(i + 2);
        return aReading;
    }

    private void AnimateFlow()
    {
        if (playing)
        {

            animationTime += Time.deltaTime * (playbackSpeed / frameTime);
            animationTime = Mathf.Clamp(animationTime, 0f, 1f);

            if (!creatingMesh)
            {
                if (!currentFrameMesh)
                {
                    currentFrameMesh = LoadFrame();
                    currentFrameData = BinaryImporter.GetFrame(animationFrame);
                    nextFrameData = BinaryImporter.GetFrame(animationFrame % BinaryImporter.GetNumFrames() - 1);
                }
                else if (!nextFrameMesh)
                {
                    if (animationFrame >= lastFrameIndex - 1)
                    {
                        playing = false;
                    }
                    else
                    {
                        animationFrame++;

                        nextFrameMesh = LoadFrame();
                        if (nextFrameMesh == null) // not loaded yet!
                            animationFrame--;
                        else
                        {
                            nextFrameMesh.SetActive(false);
                            animationFrame %= BinaryImporter.GetNumFrames() - 1;
                        }
                    }

                }
                else if (animationTime >= 1f && nextFrameMesh != null)
                {
                    Destroy(currentFrameMesh);
                    currentFrameMesh = nextFrameMesh;
                    currentFrameMesh.SetActive(true);
                    nextFrameMesh = null;
                    currentFrameData = BinaryImporter.GetFrame(animationFrame);
                    nextFrameData = BinaryImporter.GetFrame(animationFrame % BinaryImporter.GetNumFrames() - 1);
                    animationTime = 0f;
                    OnFrameChanged?.Invoke(this, AnimationFrame);
                }
            }
            Shader.SetGlobalFloat("_AnimTime", animationTime);

        }
    }

}
