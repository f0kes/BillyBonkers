using System;
using System.Collections;
using System.Collections.Generic;
using GameState;
using Networking;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovingHole : NetworkEntity
{
    [SerializeField] private Collider _moveArea;
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _turnSpeed = 0.5f;
    [SerializeField] private float _tolerance = 0.05f;
    
    private Vector2 _direction = Vector2.zero;
    private Vector2 _desiredDirection = Vector2.zero;
    
    // Start is called before the first frame update
   
    protected override void OnTick()
    {
        base.OnTick();
        if (isServer)
        {
            CorrectDirection();
            Move();
        }
    }
    

    // Update is called once per frame
   

    private void Move()
    {
        Vector3 moveVec = new Vector3(_direction.x, 0, _direction.y) * _speed * TimeTicker.TickInterval;
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
        _direction = Vector2.Lerp(_direction,_desiredDirection,_turnSpeed*TimeTicker.TickInterval);
    }
    private void ChangeDirection()
    {
        _direction = Vector2.zero;
        _desiredDirection = Vector2.zero;
        CorrectDirection();
    }
    public override Message Serialize()
    {
        Message message = new Message();
        var position = transform.position;
        message.AddFloat(position.x);
        message.AddFloat(position.y);
        message.AddFloat(position.z);

        return message;
    }

    public override void Deserialize(Message message)
    {
        float x = message.GetFloat();
        float y = message.GetFloat();
        float z = message.GetFloat();
        transform.position = new Vector3(x, y, z);
       
    }

    public override bool HasChanged(Message message)
    {
        return true;
    }
}
