using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private bool isOccupied;
    public bool IsOccupied => isOccupied;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TankController>() != null)
        {
            isOccupied = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<TankController>() != null)
        {
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<TankController>() != null)
        {
            isOccupied = false;
        }
    }
}
