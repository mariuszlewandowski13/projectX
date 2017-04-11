using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallObject {

    public Color color;

    private GameObject myObject;

    private int _source;
    public int source
    {
        get {
            return _source;
        }
        private set {
            _source = value;
            sourcePosition = myObject.transform.position;
        }
    }
    public Vector3 sourcePosition;

    private int _destination;
    public int destination
    {
         set {
            _destination = value;
            source = value - 1;
            destinationPosition = LevelsPoints.GetPointByIndex(value);
        }
        get {
            return _destination;
        }
    }
    public Vector3 destinationPosition;

    public float counter = 0.0f;
   // public float actualLerp = 0.0f;
    public float counterIncreaser = 0.1f;

    public float forwardBackward;

    public GameObject leftNeighbour;
    public GameObject rightNeighbour;

    public bool isChangingSpeed;

    public BallObject(Color newColor, GameObject left, GameObject right, GameObject myObject)
    {
        this.myObject = myObject;
        color = newColor;
        destination = 1;
        forwardBackward = 1.0f;
        leftNeighbour = left;
        rightNeighbour = right;
        
    }

}
