using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DeathTrap : MonoBehaviour
{

    public Transform spawnPoint;
    private BoxCollider col;

    void Start()
    {
        col = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            collider.GetComponent<CharacterController>().enabled = false;
            collider.transform.position = spawnPoint.position;
            collider.GetComponent<CharacterController>().enabled = true;
        }
    }
}
