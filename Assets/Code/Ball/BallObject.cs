using UnityEngine;
using System;

[Serializable]
public class BallObject {

    private static int[] allowedSpeedLevels;

    public int actualSpeedLevel;

    public int forCounter
    {
        get {
            return allowedSpeedLevels[actualSpeedLevel];
        }
    }
    public Color color;

    public GameObject myObject;

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

    public bool bonusRollBackInfluence;

    public BallObject(Color newColor,  GameObject myObject, int speedLevel)
    {
        this.myObject = myObject;
        color = newColor;
        destination = 1;
        forwardBackward = 1.0f;
        actualSpeedLevel = speedLevel;
    }

    public BallObject(BallObject ballObj)
    {
        destination = ballObj.destination;
        forwardBackward = ballObj.forwardBackward;
        destinationPosition = ballObj.destinationPosition;
    }

    public void IncreaseSpeedLevel(bool specialMo = true)
    {
        if (actualSpeedLevel == 0)
        {
            actualSpeedLevel++;
        }
        specialMove = specialMo;
    }

    public bool DecreaseSpeedLevel()
    {
        if (actualSpeedLevel != 3 && actualSpeedLevel - 1 >= 0)
        {
            actualSpeedLevel--;
            if (actualSpeedLevel == 0) specialMove = false;
        }
        return specialMove;
    }

    public void RestartSpeedLevel()
    {
        actualSpeedLevel = 0;
    }


    public static void IncreaseNormalSpeedLevel(int increaser)
    {
            allowedSpeedLevels[0] *= increaser;
    }

    public static void DecreaseNormalSpeedLevels(int decreaser)
    {
            allowedSpeedLevels[0] /= decreaser;
    }

    public void RevertDirection()
    {
        bonusRollBackInfluence = !bonusRollBackInfluence;
        if (bonusRollBackInfluence && actualSpeedLevel == 0)
        {
            forwardBackward = -forwardBackward;
            destination--;
        }

        if (!bonusRollBackInfluence && actualSpeedLevel == 0)
        {
            forwardBackward = -forwardBackward;
            destination++;
        }
    }

    public static void ClearStatic()
    {
        allowedSpeedLevels = new int[] { 9, 100, 2, 80 };
    }


}
