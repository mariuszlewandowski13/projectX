using UnityEngine;
using System;

[Serializable]
public class BallObject {

    private static int[] allowedSpeedLevels = new int[] {9, 100, 1};

    public int actualSpeedLevel = 0;

    public int forCounter
    {
        get {
            return allowedSpeedLevels[actualSpeedLevel];
        }
    }
    public Color color;

    private GameObject myObject;

    public Vector3 sourcePosition;

    public int _destination;
    public int destination
    {
         set {
            _destination = value;
            sourcePosition = myObject.transform.position;
            destinationPosition = LevelManager.GetPointByIndex(value);
        }
        get {
            return _destination;
        }
    }

    public Vector3 _destinationPosition;
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

    public Vector3 _lerpVector;
    public Vector3 lerpVector {
        get {
            return _lerpVector;
        }
        set {
                _lerpVector = value;  
        }
    }

    public float forwardBackward;

    public float speed = 0.0005f;

    public bool specialMove;

    public BallObject(Color newColor,  GameObject myObject)
    {
        this.myObject = myObject;
        color = newColor;
        destination = 1;
        forwardBackward = 1.0f;
    }

    public void IncreaseSpeedLevel(bool specialMo = true)
    {
        if (actualSpeedLevel + 1 < allowedSpeedLevels.Length - 1)
        {
            actualSpeedLevel++;
        }
        specialMove = specialMo;
    }

    public bool DecreaseSpeedLevel()
    {
        if (actualSpeedLevel - 1 >= 0)
        {
            actualSpeedLevel--;
            if (actualSpeedLevel == 0) specialMove = false;
        }
        return specialMove;
    }

    

}
