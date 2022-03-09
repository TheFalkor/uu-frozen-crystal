using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour
{
    public Mesh playerModel;
    private Transform spawnpoint;
    private BoxCollider hitbox;

    private void OnValidate()
    {
        spawnpoint = transform.GetChild(0);
        hitbox = GetComponent<BoxCollider>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.Scale(hitbox.size, hitbox.transform.localScale));
        Gizmos.DrawWireMesh(playerModel,
            spawnpoint.position,
            spawnpoint.rotation);
    }

}
