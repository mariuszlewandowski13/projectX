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
        LevelsPoints.actualLevel = 0;
    }

    private void CreateNewBall()
    {
        if (lastBall == null || CheckSpawningSafePosition())
        {
            GameObject newBall = Instantiate(ballPrefab, levelSpawningPosition.transform.position, new Quaternion());
            newBall.GetComponent<BallScript>().SetBallObject(ApplicationData.RandomNewColor(), levelSpawningPosition, lastBall);

            if (lastBall != null)
            {
                lastBall.GetComponent<BallScript>().ballObj.leftNeighbour = newBall;
            }

            lastBall = newBall;
            balls.Enqueue(lastBall);
        }
    }

    private void MoveBallsForward()
    {
            if (!MovementManager.MoveBalls(balls))
            {
                playing = false;
            }
    }

    private void SetLevelSpawningPosition()
    {
        levelSpawningPosition = LevelsPoints.GetLevelStartPoint();
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
        Debug.Log("ballAdded");
        bool isRight;
        int destination;
        Vector3 newPosition = FindNewPosition(newBall, collidingBall, out isRight, out destination);
        IncreaseSpeed(collidingBall, isRight);
        InsertIntoBallQueue(newBall, collidingBall, isRight);
        SetPositionForNewBall(newBall, newPosition, destination, collidingBall.GetComponent<BallScript>().ballObj.counterIncreaser);

    }

    private void IncreaseSpeed(GameObject collidingBall, bool isRight)
    {
        float increase = 0.1f;
        bool found = false;
        foreach (GameObject ball in balls)
        {

            if (ball == collidingBall)
            {
                found = true;
                if (isRight)
                {
                    ball.GetComponent<BallScript>().ballObj.counterIncreaser += increase;
                }
            }else if (!found)
            {
                ball.GetComponent<BallScript>().ballObj.counterIncreaser += increase;
            }
        }
    }


    private Vector3 FindNewPosition(GameObject newBall, GameObject collidingBall, out bool isRight, out int destination)
    {
        BallObject collidingBallObj = collidingBall.GetComponent<BallScript>().ballObj;
        Vector3 newPos;

        if (collidingBallObj.rightNeighbour != null)
        {
            if (Vector3.Distance(collidingBallObj.leftNeighbour.transform.position, newBall.transform.position) < Vector3.Distance(collidingBallObj.rightNeighbour.transform.position, newBall.transform.position))
            {
                newPos = collidingBall.transform.position;
                isRight = true;
                destination = collidingBallObj.source;
            }
            else {
                newPos = collidingBallObj.rightNeighbour.transform.position;
                isRight = false;
                destination = collidingBallObj.leftNeighbour.GetComponent<BallScript>().ballObj.source;
            }
        }
        else
        {
            newPos = new Vector3();
            isRight = true;
            destination = 0;
        }
        return newPos;
    }

    private void InsertIntoBallQueue(GameObject newBall, GameObject collidingBall, bool isRight)
    {
        Queue<GameObject> newQueue = new Queue<GameObject>();
        foreach (GameObject ball in balls)
        {
            if (ball == collidingBall)
            {
                if (isRight)
                {
                    newQueue.Enqueue(newBall);
                    newQueue.Enqueue(collidingBall);
                }
                else
                {
                    newQueue.Enqueue(collidingBall);
                    newQueue.Enqueue(newBall);
                }
            }
            else {
                newQueue.Enqueue(ball);
            }
        }
        balls.Clear();
        balls = newQueue;
    }

    private void SetPositionForNewBall(GameObject newBall, Vector3 newPos, int destination, float speedIncreaser)
    {
        newBall.GetComponent<BallScript>().ballObj.destination = destination;
        newBall.GetComponent<BallScript>().ballObj.destinationPosition = newPos;
        newBall.GetComponent<BallScript>().ballObj.sourcePosition = newBall.transform.position;
        newBall.GetComponent<BallScript>().ballObj.isChangingSpeed = true;
    }
}
