using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererUtils
{
    public LineRendererUtils()
    {
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

    public static SegmentIndices getNearestSegmentIndices(Vector3 currPos, Vector3[] points)
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
            var pointDelta = currPos - fstPoint;

            var pointProj = Vector3.Project(pointDelta, segmentDelta) + fstPoint;

            float candidateLeadProjDist = Vector3.Distance(currPos, pointProj);
            if (candidateLeadProjDist < leastProjDist)
            {
                leastProjDist = candidateLeadProjDist;
                fstBoundIdx = fstIdx;
                sndBoundIdx = sndIdx;
            }

        }

        return new SegmentIndices(fstBoundIdx, sndBoundIdx);
    }

    public static float getPathDistance(Vector3[] points)
    {
        float distance = 0;
        for (int fstIdx = 0; fstIdx < points.Length; fstIdx++)
        {
            int sndIdx = (fstIdx + 1) % points.Length;

            distance += Vector3.Distance(points[fstIdx], points[sndIdx]);
        }
        return distance;
    }

    public static Vector3[] createEffectivePath(Vector3 startPos, Vector3[] pathPoints)
    {
        var nearestSegIdx = getNearestSegmentIndices(startPos, pathPoints);
        Debug.Log(string.Format("segment: ({0}, {1})", nearestSegIdx.fst, nearestSegIdx.snd));
        var effectivePath = new Vector3[pathPoints.Length + 1];
        effectivePath[0] = startPos;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            int effectiveIdx = (nearestSegIdx.snd + i) % pathPoints.Length;
            effectivePath[i + 1] = pathPoints[effectiveIdx];
        }
        return effectivePath;

    }

    public static List<Vector3> getPathPositionsFraction(Vector3 startPos, float fractionAhead, float fractionBehind, Vector3[] points)
    {
        float totalDist = getPathDistance(points);
        var effectivePath = createEffectivePath(startPos, points);

        string pointRepr = "";
        for (int i = 0; i < effectivePath.Length; i++)
        {
            pointRepr += string.Format(", ({0},{1},{2})", effectivePath[i].x, effectivePath[i].y, effectivePath [i].z);
        }
        Debug.Log(pointRepr);

        float fractionAheadLeft = fractionAhead;
        List<Vector3> buzzTrailAhead = new List<Vector3>() {};
        int fstIdx = 0;
        for (int i = 0; i < effectivePath.Length; i++)
        {
            
            int sndIdx = (fstIdx + 1) % effectivePath.Length;
            float segmentFraction = Vector3.Distance(effectivePath[fstIdx], effectivePath[sndIdx]) / totalDist;

            Debug.Log(string.Format("fraction ahead left: {0}", fractionAheadLeft));
            Debug.Log(string.Format("segment fraction ahead: {0}", segmentFraction));
            if (fractionAheadLeft >= segmentFraction)
            {
                fractionAheadLeft -= segmentFraction;
                buzzTrailAhead.Add(effectivePath[fstIdx]);
            }
            else
            {
                float relativeFraction = fractionAheadLeft / segmentFraction;
                buzzTrailAhead.Add(Vector3.Lerp(effectivePath[fstIdx], effectivePath[sndIdx], relativeFraction));
                break;
            }
        }

        return buzzTrailAhead;
    }
}
