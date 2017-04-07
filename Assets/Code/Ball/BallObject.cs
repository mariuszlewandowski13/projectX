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

    public BallObject(Color newColor)
    {
        color = newColor;
        destination = 1;
        lastPointTime = Time.time;
    }

}
