using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallObject {

    public Color color;

    private int _source;
    public int source
    {
        get {
            return _source;
        }
        private set {
            _source = value;
        }
    }

    private int _destination;
    public int destination
    {
         set {
            _destination = value;
            source = value - 1;
        }
        get {
            return _destination;
        }
    }

    public float lastPointTime;

    public float speed;
    public float forwardBackward;

    public GameObject leftNeighbour;
    public GameObject rightNeighbour;

    public BallObject(Color newColor, GameObject left)
    {
        color = newColor;
        destination = 1;
        lastPointTime = Time.time;
        speed = 1.0f;
        forwardBackward = 1.0f;
        leftNeighbour = left;
    }

}
