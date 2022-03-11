using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDots : MonoBehaviour
{
    public int trailAmount = 1000;
    Queue<Vector3> positions = new Queue<Vector3>();

    void OnDrawGizmos(){
        Gizmos.color = Color.green;
        positions.Enqueue(transform.position);
        foreach (Vector3 pos in positions)
            Gizmos.DrawWireSphere(pos, 0.5f * transform.localScale.x);
        if(positions.Count > trailAmount)
            positions.Dequeue();
    }
}
