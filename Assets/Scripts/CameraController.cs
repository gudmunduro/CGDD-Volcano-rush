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

        _cameraPos = new Vector3(position.x, position.y, -10f);
        transform.position = Vector3.SmoothDamp(gameObject.transform.position, _cameraPos, ref _velocity, dampTime);
    }

    private bool _isInTilemapBounds(Vector2 position)
    {
        var tilemapX = tilemap.transform.position.x;
        var tilemapY = tilemap.transform.position.y;
        var halfTilemapX = (tilemap.size.x / 2);
        var halfTilemapY = (tilemap.size.y);

        Debug.Log($"X of tilemap: {tilemapX + halfTilemapX}, X of cam: {position.x}");

        return (position.x > tilemapX - 2 && position.x < tilemapX + halfTilemapX);
        //(position.y > tilemapY && position.y < tilemapY + halfTilemapY);
    }
}