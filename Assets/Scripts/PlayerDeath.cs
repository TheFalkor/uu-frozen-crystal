using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
	private Transform spawnpoint;

	public void Respawn()
	{
		transform.position = spawnpoint.position;
	}

    private void Start()
    {
		spawnpoint = transform;
    }

    private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Checkpoint")
		{
			spawnpoint = other.transform.GetChild(0);
		}
		else if (other.tag == "Trap")
		{
			transform.position = spawnpoint.position;
		}
	}
}
