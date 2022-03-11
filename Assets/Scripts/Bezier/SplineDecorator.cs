using System;
using UnityEngine;

public class SplineDecorator : MonoBehaviour
{
    public BezierSpline spline;
    public int frequency;
    public int precision;
    public bool lookForward;
    public bool equallyDistanced;
    public Transform[] items;
    public UnityEngine.Object[] decorations;

    public void CreateDecoration()
    {
        if(frequency <= 0  || items == null ||items.Length == 0)
        {
            return;
        }

        float stepSize = frequency;
        decorations = new UnityEngine.Object[(int)stepSize];
        if(spline.Loop || stepSize == 1)
            stepSize = 1f / stepSize;
        else
            stepSize = 1f / (stepSize - 1);

        GameObject parent = new GameObject("Decorations");
        parent.transform.parent = transform;
        for (int i = 0; i < frequency; i++)
        {
            Transform item = Instantiate(items[i % items.Length], parent.transform) as Transform;
            item.parent = parent.transform;
            Vector3 position;
            if (equallyDistanced)
            {
                position = spline.GetEquallyDistancedPoint(i, frequency, precision);
            }
            else
            {
                position = spline.GetPoint(i * stepSize);
            }
            item.position = position;
            if (lookForward)
            {
                item.transform.LookAt(position + spline.GetDirection(i * stepSize));
            }
            parent.transform.rotation = Quaternion.identity;
            UnityEngine.Object o = item.gameObject;
            decorations[i] = o;
        }
    }

    public void DeleteDecorations()
    {
        Array.Clear(decorations, 0, decorations.Length);
    }
}
