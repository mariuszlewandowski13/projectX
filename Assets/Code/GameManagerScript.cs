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
    private Queue<BallQueue> ballsQueues;
    private GameObject lastBall;
    private Vector3 levelSpawningPosition;

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
        ballsQueues = new Queue<BallQueue>();
        ballsQueues.Enqueue(new BallQueue());
    }

    private void CreateNewBall()
    {
        if (lastBall == null || CheckSpawningSafePosition())
        {
            lastBall = Instantiate(ballPrefab, levelSpawningPosition, new Quaternion());
            lastBall.GetComponent<BallScript>().SetBallObject(ApplicationData.RandomNewColor());
            ballsQueues.Peek().ballQueue.Enqueue(lastBall);
        }
    }

    private void MoveBallsForward()
    {
        foreach (BallQueue queue in ballsQueues)
        {
            if (!MovementManager.MoveBalls(queue, actualLevel))
            {
                playing = false;
            }
        }
        
    }

    private void SetLevelSpawningPosition()
    {
        levelSpawningPosition = LevelsPoints.GetLevelStartPoint(actualLevel);
    }

    private bool CheckSpawningSafePosition()
    {
        if (Vector3.Distance(levelSpawningPosition, lastBall.transform.position) > spawningSafeDistance)
        {
            return true;
        }
        return false;
    }
}
