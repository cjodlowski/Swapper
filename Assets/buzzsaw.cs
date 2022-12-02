    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class buzzsaw : MonoBehaviour
{
    [SerializeField]
    public List<Vector3> listPoints;
    public float speed;
    public float cooldown;
    public float switchDistance = 0.01f;
    public float rotateSpeed = 5;
    public LineRenderer buzzTrail;
    public float percentTrail = 0.3f;
    public bool debug = false;
    public GameObject BehindPointMarker;
    public GameObject AheadPointMarker;
    public LineRenderer SegmentDeltaMarker;
    public LineRenderer PointDeltaMarker;

    private int lastPointIndex = 0;
    private float lastDistance;
    private Rigidbody2D rb;
    private float lastTime;
    private float startTime;

   

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        transform.position = listPoints[lastPointIndex];
        startTime = Time.time;
        //var next_point = (currentPoint + 1) % listPoints.Count;
        //var next_dir = (listPoints[next_point] - listPoints[currentPoint]).normalized;
        //rb.velocity = next_dir * speed;
        //currentPoint = next_point;
        //lastTime = Time.time;
        GetComponent<Rigidbody2D>().angularVelocity = rotateSpeed;
        setBuzzTrail();
        //buzzTrail.positionCount = 3;
        //buzzTrail.SetPositions(new Vector3[3] { });
        //lastDistance = Vector3.Distance(transform.position, listPoints[currentPoint]);
    }

    public struct SegmentIndices
    {
        public int fst;
        public int snd;

        public SegmentIndices(int fst, int snd)
        {
            this.fst = fst;
            this.snd = snd;
        }
    }

    public SegmentIndices getNearestSegmentIndices(Vector3 currPos, Vector3[] points)
    {
        // find closest two indices to current position
        float leastProjDist = float.PositiveInfinity;
        int fstBoundIdx = 0;
        int sndBoundIdx = 1;

        for (int fstIdx = 0; fstIdx < points.Length; fstIdx++)
        {
            var sndIdx = (fstIdx + 1) % points.Length;
            var fstPoint = points[fstIdx];
            var sndPoint = points[sndIdx];

            var segmentDelta = sndPoint - fstPoint;
            var firstPointDelta = currPos - fstPoint;
            var secondPointDelta = currPos - sndPoint;

            var pointProj = Vector3.Project(firstPointDelta, segmentDelta) + fstPoint;

            float reconstrSegmentMagn = Vector3.Magnitude(firstPointDelta) + Vector3.Magnitude(secondPointDelta);

            float candidateLeadProjDist = Vector3.Distance(currPos, pointProj);
            if (candidateLeadProjDist < leastProjDist && Mathf.Approximately(reconstrSegmentMagn, Vector3.Magnitude(segmentDelta)))
            {
                if (debug)
                {
                    SegmentDeltaMarker.gameObject.SetActive(true);
                    PointDeltaMarker.gameObject.SetActive(true);
                    SegmentDeltaMarker.SetPositions(new Vector3[] { fstPoint, sndPoint});
                    PointDeltaMarker.SetPositions(new Vector3[] { fstPoint, currPos});
                }
                leastProjDist = candidateLeadProjDist;
                fstBoundIdx = fstIdx;
                sndBoundIdx = sndIdx;
            }

        }

        return new SegmentIndices(fstBoundIdx, sndBoundIdx);
    }

    public float getPathDistance(Vector3[] points)
    {
        float distance = 0;
        for (int fstIdx = 0; fstIdx < points.Length; fstIdx++)
        {
            int sndIdx = (fstIdx + 1) % points.Length;

            distance += Vector3.Distance(points[fstIdx], points[sndIdx]);
        }
        return distance;
    }

    public Vector3[] createEffectivePath(Vector3 startPos, Vector3[] pathPoints)
    {
        var nearestSegIdx = getNearestSegmentIndices(startPos, pathPoints);
        if (debug)
        {
            BehindPointMarker.SetActive(true);
            AheadPointMarker.SetActive(true);
            BehindPointMarker.transform.position = pathPoints[nearestSegIdx.fst];
            AheadPointMarker.transform.position = pathPoints[nearestSegIdx.snd];
        }

        //Debug.Log(string.Format("segment: ({0}, {1})", nearestSegIdx.fst, nearestSegIdx.snd));
        var effectivePath = new Vector3[pathPoints.Length + 1];
        effectivePath[0] = startPos;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            int effectiveIdx = (nearestSegIdx.snd + i) % pathPoints.Length;
            effectivePath[i + 1] = pathPoints[effectiveIdx];
        }
        return effectivePath;

    }

    public List<Vector3> GetPathFractionPositions(Vector3 startPos, float fractionAhead, float fractionBehind, Vector3[] points)
    {
        float totalDist = getPathDistance(points);
        var effectivePath = createEffectivePath(startPos, points);

        //string pointRepr = "Points: ";
        //for (int i = 0; i < points.Length; i++)
        //{
        //    pointRepr += string.Format(", ({0},{1},{2})", points[i].x, points[i].y, points[i].z);
        //}

        //string effectiveRepr = "Effective path: ";
        //for (int i = 0; i < effectivePath.Length; i++)
        //{
        //    effectiveRepr += string.Format(", ({0},{1},{2})", effectivePath[i].x, effectivePath[i].y, effectivePath[i].z);
        //}

        float fractionAheadLeft = fractionAhead;
        List<Vector3> buzzTrailAhead = new List<Vector3>();
        for (int fstIdx = 0; fstIdx < effectivePath.Length; fstIdx++)
        {
            int sndIdx = (fstIdx + 1) % effectivePath.Length;
            float segmentFraction = Vector3.Distance(effectivePath[fstIdx], effectivePath[sndIdx]) / totalDist;

            if (fractionAheadLeft >= segmentFraction)
            {
                fractionAheadLeft -= segmentFraction;
                buzzTrailAhead.Add(effectivePath[fstIdx]);
            }
            else
            {
                float relativeFraction = fractionAheadLeft / segmentFraction;
                buzzTrailAhead.Add(effectivePath[fstIdx]);
                buzzTrailAhead.Add(Vector3.Lerp(effectivePath[fstIdx], effectivePath[sndIdx], relativeFraction));
                break;
            }
        }

        float fractionBehindLeft = fractionBehind;
        List<Vector3> buzzTrailBehind = new List<Vector3>();
        for (int i = 0; i < effectivePath.Length; i++)
        {
            int fstIdx = (effectivePath.Length - i) % effectivePath.Length;
            int sndIdx = (fstIdx - 1 + effectivePath.Length) % effectivePath.Length;
            float segmentFraction = Vector3.Distance(effectivePath[fstIdx], effectivePath[sndIdx]) / totalDist;
            if (fractionBehindLeft >= segmentFraction)
            {
                fractionBehindLeft -= segmentFraction;
                buzzTrailBehind.Add(effectivePath[fstIdx]);
            }
            else
            {
                float relativeFraction = fractionBehindLeft / segmentFraction;
                buzzTrailBehind.Add(effectivePath[fstIdx]);
                buzzTrailBehind.Add(Vector3.Lerp(effectivePath[fstIdx], effectivePath[sndIdx], relativeFraction));
                break;
            }
        }

        buzzTrailBehind.Reverse();
        buzzTrailBehind.AddRange(buzzTrailAhead);
        return buzzTrailBehind;
    }
         
    void setBuzzTrail()
    {
        var trailPositions = GetPathFractionPositions(transform.position, percentTrail, percentTrail, listPoints.ToArray());
        buzzTrail.positionCount = trailPositions.Count;
        buzzTrail.SetPositions(trailPositions.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        var nextPoint = (lastPointIndex + 1) % listPoints.Count;
        float distCovered = (Time.time - startTime) * speed;
        float fractionCovered = distCovered / (listPoints[nextPoint] - listPoints[lastPointIndex]).magnitude;
        transform.position = Vector3.Lerp(listPoints[lastPointIndex], listPoints[nextPoint], fractionCovered);
        var currDistance = Vector3.Distance(transform.position, listPoints[nextPoint]);
        setBuzzTrail();

        if (currDistance < switchDistance)
        {

            startTime = Time.time;
            lastPointIndex = nextPoint;
        }
        //var next_point = (currentPoint + 1) % listPoints.Count;
        //var currDistance = Vector3.Distance(transform.position, listPoints[next_point]);
        //if (currDistance < switchDistance)
        //{
        //    var next_dir = (listPoints[next_point] - listPoints[currentPoint]).normalized;
        //    rb.velocity = next_dir * speed;
        //    currentPoint = next_point;
        //    lastDistance = Vector3.Distance(transform.position, listPoints[next_point]);
        //}
        //else
        //{
        //    if (currDistance > lastDistance)
        //    {
        //        var next_dir = (listPoints[next_point] - listPoints[currentPoint]).normalized;
        //        rb.velocity = next_dir * speed;
        //    }
        //}
        //lastDistance = currDistance;
    }
}
