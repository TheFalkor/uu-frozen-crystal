using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController character;

    private void Start()
    {
        character = GetComponent<CharacterController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Trap")
        {
            Debug.Log("Player is in trap");
        }
    }
}
