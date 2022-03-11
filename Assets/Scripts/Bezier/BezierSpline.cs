using System;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{
    public Vector3 nextCurveOffset;

    [SerializeField]
    private Vector3[] points;
    [SerializeField]
    private BezierControlPointMode[] modes;
    [SerializeField]
    private bool loop;

    public int CurveCount{
        get {
            return (points.Length - 1) / 3;
        }
    }

    public int ControlPointCount{
        get{
            return points.Length;
        }
    }

    public bool Loop{
        get {
            return loop;
        }
        set {
            loop = value;
            if(value == true){
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    public void Reset(){
        points = new Vector3[]{
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
        modes = new BezierControlPointMode[2]{
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    public Vector3 GetPoint (float t){
        int i;
        if(t >= 1f){
            t = 1f;
            i = points.Length - 4;
        }
        else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public Vector3 GetEquallyDistancedPoint(int index, int numSegments, int precision = 5)
    {
        Vector3[] chordPoints = new Vector3[numSegments];
        float t = (float)index / numSegments;
        for(int i = 0; i < chordPoints.Length; i++) 
        {
            float r = (1f / numSegments) * i;
            chordPoints[i] = GetPoint(r);
        }

        for(int n = 0; n < precision; n++)
        {
            float[] distances = new float[numSegments - 1];
            for(int i = 0; i < distances.Length; i++)
            {
                distances[i] = Vector3.Distance(chordPoints[i], chordPoints[i + 1]);
            }
            float[] ratios = new float[numSegments - 2];
            for(int i = 0; i < ratios.Length; i++)
            {
                ratios[i] = 0.5f * 
                    (distances[i + 1] - distances[i]) /
                    (distances[i] + distances[i + 1]);
            }
            for(int i = 1; i < chordPoints.Length - 2; i++)
            {
                if(ratios[i - 1] > 0f)
                {
                    chordPoints[i] += ratios[i - 1] * (chordPoints[i + 1] - chordPoints[i]);
                }
                else
                {
                    chordPoints[i] += ratios[i - 1] * (chordPoints[i] - chordPoints[i - 1]);
                }
            }
        }
        return chordPoints[index];
    }


    public Vector3 GetVelocity(float t){
        int i;
        if(t >= 1f){
            t = 1f;
            i = points.Length - 4;
        }
        else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t){
        return GetVelocity(t).normalized;
    }

    public void AddCurve(){
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += nextCurveOffset.x;
        point.y += nextCurveOffset.y;
        point.z += nextCurveOffset.z;
        points[points.Length - 3] = point;
        point.x += nextCurveOffset.x;
        point.y += nextCurveOffset.y;
        point.z += nextCurveOffset.z;
        points[points.Length - 2] = point;
        point.x += nextCurveOffset.x;
        point.y += nextCurveOffset.y;
        point.z += nextCurveOffset.z;
        points[points.Length - 1] = point;
        point.x += nextCurveOffset.x;
        point.y += nextCurveOffset.y;
        point.z += nextCurveOffset.z;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if(loop){
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

    public void RemoveCurve(int pointIndex)
    {
        int curveIndex = (pointIndex - 1) / 3;
        if (curveIndex < 0 || curveIndex > CurveCount)
            return;

        if(curveIndex == CurveCount)
        {
            Array.Clear(points, points.Length - 3, 3);
            Array.Resize(ref points, points.Length - 3);

            Array.Clear(modes, modes.Length - 1, 1);
            Array.Resize(ref modes, modes.Length - 1);
        }
        else
        {
            Array.Clear(points, curveIndex * 3, 3);
            for (int i = curveIndex * 3 + 3; i < points.Length; i++)
                points[i - 3] = points[i];

            Array.Resize(ref points, points.Length - 3);
        }

    }

    public Vector3 GetControlPoint(int index){
        return points[index];
    }

    public void SetControlPoint(int index, Vector3 point){
        if (index % 3 == 0){
            Vector3 delta = point - points[index];
            if(loop){
                if(index == 0){
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if(index == points.Length - 1){
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else{
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else {
                if(index > 0){
                    points[index - 1] += delta;
                }
                if(index + 1 < points.Length){
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        EnforceMode(index);
    }

    public BezierControlPointMode GetControlPointMode (int index){
        int i = (index + 1) / 3;
        return modes[i];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode){
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if(loop){
            if(modeIndex == 0){
                modes[modes.Length - 1] = mode;
            }
            else if(modeIndex == modes.Length - 1){
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode(int index){
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if(mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)){
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex){
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0){
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if(enforcedIndex >= points.Length){
                enforcedIndex = 1;
            }
        }
        else {
            fixedIndex = middleIndex + 1;
            if(fixedIndex >= points.Length){
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if(enforcedIndex < 0){
                enforcedIndex = points.Length - 2;
            }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if(mode == BezierControlPointMode.Aligned){
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }
}
