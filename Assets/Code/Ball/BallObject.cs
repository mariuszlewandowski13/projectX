using UnityEngine;

public class BallObject {

    private static float[] allowedSpeedLevels = new float[] {0.01f, 0.02f, 0.02f };

    private int actualSpeedLevel = 0;

    public Color color;

    private GameObject myObject;


    public Vector3 sourcePosition;

    private int _destination;
    public int destination
    {
         set {
            _destination = value;
            sourcePosition = myObject.transform.position;
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

    private Vector3 _lerpVector;
    public Vector3 lerpVector {
        get {
            return _lerpVector;
        }
        set {
                _lerpVector = value;

            
        }
    }

    public float forwardBackward;

    public float speed;

    public bool specialMove;

    public BallObject(Color newColor,  GameObject myObject)
    {
        this.myObject = myObject;
        color = newColor;
        destination = 1;
        forwardBackward = 1.0f;
        speed = allowedSpeedLevels[0];
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
