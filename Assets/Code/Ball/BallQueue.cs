using System.Collections.Generic;
using UnityEngine;

public class BallQueue {
    private float _speed;
    public float speed
    {
        get {
            return _speed * forwardBackward;
        }
        set {
            _speed = value;
        }
    }
    public float forwardBackward;

    public Queue<GameObject> ballQueue;
    

    public BallQueue()
    {
        ballQueue = new Queue<GameObject>();
        speed = 1.0f;
        forwardBackward = 1.0f;
    }

    public BallQueue(float newSpeed, float newForwardBackward)
    {
        ballQueue = new Queue<GameObject>();
        speed = newSpeed;
        forwardBackward = newForwardBackward;
    }
}
