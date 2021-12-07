using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 1;
    
    private Transform _startPoint;
    private Transform _endPoint;
    private Rigidbody2D _body;
    private bool _movingToStart = false;
    private float _distToDest = .1f;

    private Vector3 _distance;
    private Vector3 _vectorTo;
    private PlatformSensor _sensor;
    
    // Start is called before the first frame update
    void Start()
    {
        _startPoint = transform.parent.Find("StartPoint");
        _endPoint = transform.parent.Find("EndPoint");
        _sensor = transform.Find("UnderPlayerSensor").GetComponent<PlatformSensor>();
        _body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var currentPoint = _movingToStart ? _startPoint: _endPoint;

        _distance = currentPoint.position - transform.position;

        if (_distance.magnitude < _distToDest)
        {
            _movingToStart = !_movingToStart;
        }
        
        _vectorTo = _distance.normalized * (speed * 0.01f);

        if (_vectorTo.y < 0 && _sensor.Sense())
        {
            _vectorTo = new Vector3(_vectorTo.x, 0, _vectorTo.z);
        }
        
        transform.position += _vectorTo;

    }
}
