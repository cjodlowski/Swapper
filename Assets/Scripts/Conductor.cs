using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance { get; private set; }

    [SerializeField]
    public float BPM = 80;

    private float intervalS;
    private float intervalTime = float.NegativeInfinity;
    private float timeUntilNextBeat = 0;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        intervalS = 60f / BPM;
    }


    void FixedUpdate()
    {
        if (Time.time > intervalTime)
        {
            intervalTime = Time.time + intervalS;
        }
        timeUntilNextBeat = intervalTime - Time.time;
    }

    public float getTimeUntilNextBeat() => timeUntilNextBeat;
    public float getBeatTimeInterval() => 60f / BPM;
}
