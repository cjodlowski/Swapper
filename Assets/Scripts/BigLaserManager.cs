using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BigLaserManager : MonoBehaviour
{
        
    int RANDOM_POS_RETRIES = 5;

    [SerializeField]
    public GameObject laserBody;
    public GameObject laserWarning;
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
    public float defaultWarningFlashInterval=0.19f;
    public Color laserColor = Color.red;
    public Color warningColor = Color.yellow;
    public float laserFlashDuration = 4;
    public float defaultLaserDeployInterval = 20;
    public float defaultWarningFlashDuration = 4;

    private float laserDeployInterval;
    private float warningFlashDuration;
    private bool deploying = false;
    private float warningFlashInterval;

    private Coroutine warningRoutine;
    // Start is called before the first frame update
    private void Awake()
    {
        laserDeployInterval = defaultLaserDeployInterval;
        warningFlashDuration = defaultWarningFlashDuration;
        warningFlashInterval = defaultWarningFlashInterval;
        toggleWarning(false);
        toggleLaser(false);
    }
    void Start()
    {
        //SetLaserPosition(new Vector2(0,-7), new Vector2(0,7));

        laserDeployInterval = defaultLaserDeployInterval;
        StartCoroutine(periodicLaserDeploy());
    }

    public IEnumerator periodicLaserDeploy()
    {
        while (true)
        {

            while (deploying)
            {
                yield return null;
            }
            yield return StartCoroutine(CoroutineUtils.WaitForSecondsExcludePause(laserDeployInterval));
            StartCoroutine(laserSequence());

        }
    }

    public void toggleLaserSpeedup(bool speedup)
    {
        //StopCoroutine(periodicLaserDeployCoroutine);
        //periodicLaserDeployCoroutine = StartCoroutine(periodicLaserDeploy(defaultLaserDeployInterval));
        //toggleWarning(false);
        //toggleLaser(false);
        if (speedup)
        {
            laserDeployInterval = defaultLaserDeployInterval / 3;
            warningFlashDuration = defaultWarningFlashDuration * 3f / 5f;
            warningFlashInterval = defaultWarningFlashInterval * 3f / 5f;
            //StartCoroutine(periodicLaserDeploy());
        }
        else
        {
            laserDeployInterval = defaultLaserDeployInterval;
            warningFlashDuration = defaultWarningFlashDuration;
            warningFlashInterval = defaultWarningFlashInterval;

            Debug.Log(laserDeployInterval);
            //StartCoroutine(periodicLaserDeploy());
        }
    }

    public void SetLaserPosition(Vector2 start, Vector2 stop)
    {
        var laserLineRenderer = laserBody.GetComponent<LineRenderer>();
        var effectiveStart = new Vector2(start.x, start.y);
        var effectiveStop = new Vector2(stop.x, stop.y);
        var effectiveDelta = effectiveStop - effectiveStart;
        var laserPositions = new Vector3[2] { effectiveStart, effectiveStop};
        laserLineRenderer.SetPositions(laserPositions);


        var warningLineRenderer = laserWarning.GetComponent<LineRenderer>();
        var warningPositions = new Vector3[5]
        {
            effectiveStart + Vector2.Perpendicular(effectiveDelta).normalized * 1.5f,
            effectiveStart + Vector2.Perpendicular(effectiveDelta).normalized * 1.5f + effectiveDelta,
            effectiveStart + Vector2.Perpendicular(effectiveDelta).normalized * 1.5f + effectiveDelta - Vector2.Perpendicular(effectiveDelta).normalized * 3,
            effectiveStart - Vector2.Perpendicular(effectiveDelta).normalized * 1.5f,
            effectiveStart + Vector2.Perpendicular(effectiveDelta).normalized * 1.5f,
        };
        warningLineRenderer.SetPositions(warningPositions);

        var coll = laserBody.GetComponent<PolygonCollider2D>();
        coll.points = new Vector2[5]
        {
            effectiveStart + Vector2.Perpendicular(effectiveDelta).normalized * 1.5f,
            effectiveStart + Vector2.Perpendicular(effectiveDelta).normalized * 1.5f + effectiveDelta,
            effectiveStart + Vector2.Perpendicular(effectiveDelta).normalized * 1.5f + effectiveDelta - Vector2.Perpendicular(effectiveDelta).normalized * 3,
            effectiveStart - Vector2.Perpendicular(effectiveDelta).normalized * 1.5f,
            effectiveStart + Vector2.Perpendicular(effectiveDelta).normalized * 1.5f,
        };



    }

    public void SetRandomLaserPosition()
    {
        var xSpan = xMax - xMin;
        var ySpan = yMax - yMin;
        //horzontal start
        if (Random.value > 0.5f)
        {
            var startPoint = new Vector2(xMin, yMin + Random.value * ySpan);
            var endPoint = new Vector2(xMax, yMin + Random.value * ySpan);
            SetLaserPosition(startPoint, endPoint);
        }
        //vertical start
        else
        {
            var startPoint = new Vector2(xMin + Random.value * xSpan, yMin);
            var endPoint = new Vector2(xMin + Random.value * xSpan, yMax);
            SetLaserPosition(startPoint, endPoint);
        }


    }

    public void toggleWarning(bool warning)
    {
        if (!warning)
        {
            if (warningRoutine != null)
            {
                StopCoroutine(warningRoutine);

            }
            var lineRend = laserWarning.GetComponent<LineRenderer>();
            lineRend.startColor = Color.clear;
            lineRend.endColor = Color.clear;
        }
        else
        {
            warningRoutine = StartCoroutine(warningFlash());
        }
    }

    private IEnumerator warningFlash()
    {
        var lineRend = laserWarning.GetComponent<LineRenderer>();
        while (true)
        {
            lineRend.startColor = warningColor;
            lineRend.endColor = warningColor;
            yield return StartCoroutine(CoroutineUtils.WaitForSecondsExcludePause(warningFlashInterval));
            lineRend.startColor = Color.clear;
            lineRend.endColor = Color.clear;
            yield return StartCoroutine(CoroutineUtils.WaitForSecondsExcludePause(warningFlashInterval));
        }

    }

    public void toggleLaser(bool laser)
    {
        var lineRend = laserBody.GetComponent<LineRenderer>();
        var coll = laserBody.GetComponent<PolygonCollider2D>();
        if (!laser)
        {
            lineRend.startColor = Color.clear;
            lineRend.endColor = Color.clear;
            coll.enabled = false;
        }
        else
        {
            lineRend.startColor = laserColor;
            lineRend.endColor = laserColor;
            coll.enabled = true;
        }
    }

    public void laserFlash()
    {
        StartCoroutine(TempLaser());
    }

    private IEnumerator TempLaser()
    {
        toggleLaser(true);
        yield return StartCoroutine(CoroutineUtils.WaitForSecondsExcludePause(laserFlashDuration));
        toggleLaser(false);
    }
    public IEnumerator laserSequence()
    {
        deploying = true;
        SetRandomLaserPosition();
        toggleWarning(true);
        yield return StartCoroutine(CoroutineUtils.WaitForSecondsExcludePause(warningFlashDuration));
        toggleWarning(false);
        laserFlash();
        deploying = false;
    }
}
