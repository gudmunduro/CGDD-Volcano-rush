using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Tilemap tilemap;
    public float dampTime = 0.4f;
    private Vector3 _cameraPos;
    private Camera _camera;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        var position = player.position;
        
        var camHeight = 2f * _camera.orthographicSize;
        var camWidth = camHeight * _camera.aspect;

        _fixCameraPos(ref position, camWidth, camHeight);

        _cameraPos = new Vector3(position.x, position.y, -10f);
        transform.position = Vector3.SmoothDamp(gameObject.transform.position, _cameraPos, ref _velocity, dampTime);
    }
    
    private void _fixCameraPos(ref Vector3 position, float width, float height)
    {
        var tilemapX = tilemap.transform.position.x;
        var tilemapY = tilemap.transform.position.y;
        var halfTilemapX = tilemap.size.x / 2;
        var halfTilemapY = tilemap.size.y / 2;

        if (!(position.x - width / 2 > tilemapX - 0.5f))
        {
            position.x = tilemapX + width / 2 - 0.5f;
        }
        else if (!(position.x + width / 2 < tilemapX + halfTilemapX - 1.5f))
        {
            position.x = tilemapX + halfTilemapX - width / 2 - 1.5f;
        }
        if (position.y + height > halfTilemapY - tilemapY - height/2 - 2.0f)
        {
            position.y = halfTilemapY - tilemapY - height / 2 - 2.0f - height;
        }
        else if (position.y - height < -halfTilemapY+tilemapY + 7.0f)
        {
            position.y = -halfTilemapY+tilemapY + 7.0f + height;
        }
    }
}