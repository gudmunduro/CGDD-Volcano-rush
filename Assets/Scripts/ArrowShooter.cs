using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootDirection
{
    Left,
    Right,
    Up,
    Down
}

public class ArrowShooter : MonoBehaviour
{
    public float arrowFiringRate = 4.0f;
    public float arrowSpeed = 3.5f;
    public Transform arrowPrefab;
    public ShootDirection shootDirection = ShootDirection.Left;


    private float _lastArrowShotTime;

    void Start()
    {
    }

    void Update()
    {
        if (_lastArrowShotTime + arrowFiringRate < Time.realtimeSinceStartup)
        {
            _shootArrow();
            _lastArrowShotTime = Time.realtimeSinceStartup;
        }
    }

    private void _shootArrow()
    {
        var arrow = Instantiate(arrowPrefab);
        arrow.position = transform.position;
        arrow.rotation = Quaternion.Euler(new Vector3(0, 0, _arrowRotation()));

        var rigidBody = arrow.GetComponent<Rigidbody2D>();
        rigidBody.velocity = shootDirection switch
        {
            ShootDirection.Right => new Vector2(arrowSpeed, 0),
            ShootDirection.Left => new Vector2(-arrowSpeed, 0),
            ShootDirection.Up => new Vector2(0, arrowSpeed),
            ShootDirection.Down => new Vector2(0, -arrowSpeed)
        };
    }

    private float _arrowRotation()
    {
        return shootDirection switch
        {
            ShootDirection.Right => 0,
            ShootDirection.Left => 180.0f,
            ShootDirection.Up => 90.0f,
            ShootDirection.Down => 270.0f
        };
    }
}