using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {
    public BallObject ballObj;
   

    public void SetBallObject(Color color)
    {
        ballObj = new BallObject(color, gameObject);
        SetBallColor();
    }

    private void SetBallColor()
    {
        GetComponent<Renderer>().material.SetColor("_Color", ballObj.color);
    }
}


