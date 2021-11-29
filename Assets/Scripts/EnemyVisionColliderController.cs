using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionColliderController : MonoBehaviour
{
    public GameObject PlayerInVision { get; private set; }
    public bool IsPlayerInVision => PlayerInVision != null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "character")
        {
            PlayerInVision = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "character")
        {
            PlayerInVision = null;
        }
    }
}