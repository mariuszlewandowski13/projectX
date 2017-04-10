using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    #region Public Properties
    public GameObject ballPrefab;
    #endregion

    //constants
    private const float spawningSafeDistance = 0.8f;


    //Objects and Positions
    private Queue<GameObject> balls;
    private GameObject lastBall;
    private GameObject levelSpawningPosition;

    //flags
    private bool playing = false;
    private bool prepareLevel = true;

    //game info
    private int actualLevel = 0;


	void Start () {
        Init();
	}


	void Update () {
        if (prepareLevel)
        {
            SetLevelSpawningPosition();
            prepareLevel = false;
            playing = true;
        }

        if (playing)
        {
            MoveBallsForward();
            CreateNewBall();
        }
        
	}

    private void Init()
    {
        balls = new Queue<GameObject>();
    }

    private void CreateNewBall()
    {
        if (lastBall == null || CheckSpawningSafePosition())
        {
            GameObject left;
            if (lastBall == null)
            {
                left = levelSpawningPosition;
            }
            else {
                left = lastBall;
            }

            GameObject newBall = Instantiate(ballPrefab, levelSpawningPosition.transform.position, new Quaternion());
            newBall.GetComponent<BallScript>().SetBallObject(ApplicationData.RandomNewColor(), left);

            if (lastBall != null)
            {
                lastBall.GetComponent<BallScript>().ballObj.rightNeighbour = newBall;
            }

            lastBall = newBall;
            balls.Enqueue(lastBall);
        }
    }

    private void MoveBallsForward()
    {
            if (!MovementManager.MoveBalls(balls, actualLevel))
            {
                playing = false;
            }
    }

    private void SetLevelSpawningPosition()
    {
        levelSpawningPosition = LevelsPoints.GetLevelStartPoint(actualLevel);
    }

    private bool CheckSpawningSafePosition()
    {
        if (Vector3.Distance(levelSpawningPosition.transform.position, lastBall.transform.position) > spawningSafeDistance)
        {
            return true;
        }
        return false;
    }

    public void AddNewBallFromPlayer(GameObject newBall, GameObject collidingBall)
    {
        bool isRight;
        Vector3 newPosition = FindNewPosition(newBall, collidingBall, out isRight);
        IncreaseSpeed(collidingBall, isRight);
    }

    private void IncreaseSpeed(GameObject collidingBall, bool isRight)
    {
        float increase = 2.0f;
        bool found = false;
        foreach (GameObject ball in balls)
        {
            if (found)
            {
                ball.GetComponent<BallScript>().ballObj.speed *= increase;
            }
            else if (ball == collidingBall)
            {

                found = true;
                if (isRight)
                {
                    ball.GetComponent<BallScript>().ballObj.speed *= increase;
                }
            }
        }
    }


    private Vector3 FindNewPosition(GameObject newBall, GameObject collidingBall, out bool isRight)
    {//TO DO
        isRight = true;
        return new Vector3();
    }
    
}
