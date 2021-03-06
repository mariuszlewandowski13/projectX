﻿using UnityEngine;

public class PlayerBallsScript : MonoBehaviour {

    public GameObject ballPrefab;

    private GameObject myBall;

    private Vector3 startPosition;

    private float forceMultiplier = 100.0f;

    private bool ballToSpawn;

    private float lastShotTime;

    private float timeFromShootToSpawn = 0.0f;

     void Start()
    {
        startPosition = Camera.main.transform.position;
        CreateNewBall(); 
    }

    void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);

            if (hit.transform != null && (hit.transform.tag == "Board" || hit.transform.tag == "Ball") )
            {
                if (ShootTheBall(hit.point))
                {
                    SetTimeForSpawningNewBall();
                }   
            }
        }

        if (ballToSpawn && (Time.time - lastShotTime) > timeFromShootToSpawn)
        {
            CreateNewBall();
            ballToSpawn = false;
        }
    }

    private void CreateNewBall()
    {
        myBall = Instantiate(ballPrefab, startPosition , new Quaternion());
        myBall.name = "Ball" + ApplicationData.GetBallIndex().ToString();
        Color color = ApplicationData.RandomNewColorForPlayer();
        myBall.GetComponent<BallScript>().SetBallObject(color/*Color.black*/);
        GameObject.Find("board").GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    private bool ShootTheBall(Vector3 destination)
    {
        if (myBall != null)
        {
            Vector3 force = destination - startPosition;
            myBall.AddComponent<BallFlyingScript>().myOldParent = Camera.main.gameObject;
            Rigidbody rigid =  myBall.AddComponent<Rigidbody>();
            rigid.useGravity = false;
            rigid.isKinematic = false;
            rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigid.AddForce(force* forceMultiplier, ForceMode.Force);
            myBall = null;
            return true;
        }

        return false;
    }

    private void SetTimeForSpawningNewBall()
    {
        lastShotTime = Time.time;
        ballToSpawn = true;
    }

}
