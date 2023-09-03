using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] private CharacterController PlayerBody;
    [Tooltip("Step stride distance (in meters)")]
    [SerializeField] private float StrideLength = 0.3f;
    [Tooltip("If it's been less time than this value between steps, use running sounds (in seconds)")]
    [SerializeField] private float RunningTimeBetweenSteps = 0.5f;
    [Tooltip("Footsteps volume")]
    [SerializeField] private float TerrainFootstepsVolume = 1f;

    

    [SerializeField] private List<AudioClip> WalkingTerrainSounds;
    [SerializeField] private List<AudioClip> RunningTerrainSounds;

    private Vector2 lastStepPosition = Vector3.zero;
    private float lastStepTime = 0f;
    private AudioSource FootstepsSource;

    void Start()
    {
        InitFootstepsSource();
        lastStepPosition = PlayerBody.transform.position;
        lastStepTime = Time.time;
    }

    private void FixedUpdate()
    {        
        HandleFootsteps();
    }
    private void HandleFootsteps()
    {
        float distanceTravelled = Vector2.Distance(lastStepPosition, PlayerPos());
        float timePassed = Time.time - lastStepTime;
        bool running = timePassed < RunningTimeBetweenSteps;
        float distanceNeeded = StrideLength * (running ? 2 : 1);

        if (distanceTravelled >= distanceNeeded)
        {
           
            PlayFootstep(running);
        }
    }

    private void PlayFootstep(bool running)
    {
        var clips = new List<AudioClip>();
        if (running)
            clips = RunningTerrainSounds;
        else
            clips = WalkingTerrainSounds;       
      
    var clip = clips[Random.Range(0, clips.Count)];
    
      FootstepsSource.PlayOneShot(clip);

    lastStepPosition = PlayerPos();
    lastStepTime = Time.time;

    }

private void InitFootstepsSource()
    {
        if (FootstepsSource != null) Destroy(FootstepsSource);

        FootstepsSource = gameObject.AddComponent<AudioSource>();
    }

    private Vector2 PlayerPos() =>
       new Vector2(PlayerBody.transform.position.x, PlayerBody.transform.position.z);
}
