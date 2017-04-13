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

    private Vector3 _destinationPosition;
    public Vector3 destinationPosition
    {
        get {
            return _destinationPosition;
        }
        set {
            _destinationPosition = value;
            lerpVector = new Vector3();
        }
    }

    public Vector3 lerpVector;

    public float forwardBackward;

    public BallObject(Color newColor,  GameObject myObject)
    {
        this.myObject = myObject;
        color = newColor;
        destination = 1;
        forwardBackward = 1.0f;  
    }

}
