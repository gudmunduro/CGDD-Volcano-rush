using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float dampTime = 0.4f;
    private Vector3 _cameraPos;
    private Vector3 _velocity = Vector3.zero;

    private void Update()
    {
        var position = player.position;
        _cameraPos = new Vector3(position.x, position.y, -10f);
        transform.position = Vector3.SmoothDamp(gameObject.transform.position, _cameraPos, ref _velocity, dampTime);
    }
}
