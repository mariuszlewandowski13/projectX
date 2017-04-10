using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {
    public BallObject ballObj;
   

    public void SetBallObject(Color color, GameObject leftNeighbour)
    {
        ballObj = new BallObject(color, leftNeighbour);
        SetBallColor();
    }

    private void SetBallColor()
    {
        GetComponent<Renderer>().material.SetColor("_Color", ballObj.color);
    }
}


