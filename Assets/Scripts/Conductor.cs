using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance { get; private set; }

    [SerializeField]
    public float BPM = 80;

    private float intervalS;
    private float elapsedTime;
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

        elapsedTime = 0;
    }


    void FixedUpdate()
    {
        if (GameManager.isRoundStarted && GameManager.isPaused)
        {
            if (elapsedTime > intervalS)
            {
                elapsedTime += Time.fixedDeltaTime;
            }
            timeUntilNextBeat = intervalS - elapsedTime;
        }
    }

    public float getTimeUntilNextBeat() => timeUntilNextBeat;
    public float getBeatTimeInterval() => 60f / BPM;
}
