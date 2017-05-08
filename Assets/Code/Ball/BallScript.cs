using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallScript : MonoBehaviour {
    [SerializeField]
    public BallObject ballObj;
   

    public void SetBallObject(Color color, int speedLevel = 0)
    {
        ballObj = new BallObject(color, gameObject, speedLevel);
        SetBallColor();
    }

    private void SetBallColor()
    {
        GetComponent<Renderer>().material.SetColor("_Color", ballObj.color);
    }
}


