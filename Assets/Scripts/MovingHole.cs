using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingHole : MonoBehaviour
{
    [SerializeField] private Collider _moveArea;
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _turnSpeed = 0.5f;
    [SerializeField] private float _tolerance = 0.05f;
    
    private Vector2 _direction = Vector2.zero;
    private Vector2 _desiredDirection = Vector2.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CorrectDirection();
        Move();
    }

    private void Move()
    {
        Vector3 moveVec = new Vector3(_direction.x, 0, _direction.y) * _speed * Time.deltaTime;
        if (_moveArea.bounds.Contains(transform.position + moveVec))
        {
            transform.position += moveVec;
        }
        else
        {
            ChangeDirection();
        }
    }

    private void CorrectDirection()
    {
        if ((_desiredDirection - _direction).magnitude<=_tolerance)
        {
            _desiredDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }
        _direction = Vector2.Lerp(_direction,_desiredDirection,_turnSpeed*Time.deltaTime);
    }
    private void ChangeDirection()
    {
        _direction = Vector2.zero;
        _desiredDirection = Vector2.zero;
        CorrectDirection();
    }
}
