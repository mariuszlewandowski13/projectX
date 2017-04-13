using UnityEngine;

public class BallObject {

    private static float[] allowedSpeedLevels = new float[] {0.02f, 0.1f, 0.1f };

    private int actualSpeedLevel = 0;

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

    public float speed;

    public bool specialMove;

    public BallObject(Color newColor,  GameObject myObject)
    {
        this.myObject = myObject;
        color = newColor;
        destination = 1;
        forwardBackward = 1.0f;
        speed = 0.02f;
    }

    public void IncreaseSpeedLevel()
    {
        if (actualSpeedLevel + 1 < allowedSpeedLevels.Length)
        {
            actualSpeedLevel++;
            speed = allowedSpeedLevels[actualSpeedLevel];
            specialMove = true;
        }
    }

    public bool DecreaseSpeedLevel()
    {
        if (actualSpeedLevel - 1 >= 0)
        {
            actualSpeedLevel--;
            speed = allowedSpeedLevels[actualSpeedLevel];
            if (actualSpeedLevel == 0) specialMove = false;
        }
        return specialMove;
    }

}
