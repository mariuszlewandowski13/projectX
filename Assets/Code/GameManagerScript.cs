using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    #region Public Properties
    public GameObject ballPrefab;
    #endregion

    #region Private Properties

    private const float spawningSafeDistance = 0.8f;

    private BList balls;

    private Vector3 levelSpawningPosition;

    private bool playing;

    #endregion


    private void Start()
    {
        Init();
        playing = true;
    }


    private void Init()
    {
        balls = new BList();
        levelSpawningPosition = LevelsPoints.GetLevelStartPoint();
    }

    private void Update()
    {
        CheckAndCreateNewBall();
    }

    private void CheckAndCreateNewBall()
    {
        if (playing)
        {
            if (CheckSpawningSafe())
            {
                CreateNewBall();
            }
            playing = MovementManager.MoveBalls(balls);
        }

    }

    private void CreateNewBall()
    {
        GameObject newBall = Instantiate(ballPrefab, levelSpawningPosition, new Quaternion());
        newBall.GetComponent<BallScript>().SetBallObject(ApplicationData.RandomNewColor());
        balls.AppendLast(newBall);
    }

    private bool CheckSpawningSafe()
    {
        if (balls.Last == null || Vector3.Distance(balls.Last.transform.position, levelSpawningPosition) >= spawningSafeDistance)
        {
            return true;
        }
        else return false;
    }

    public void AddNewBallFromPlayer(GameObject newBall, GameObject collidingBall)
    {
        Vector3 positionForNewBall;
        Vector3 positionForFirstBall;
        bool isRight = CheckIfIsRightAndFindNewPositions(newBall, collidingBall, out positionForNewBall, out positionForFirstBall);
    }

    public bool CheckIfIsRightAndFindNewPositions(GameObject newBall, GameObject collidingBall,  out Vector3 posForNewBall, out Vector3 posForFirstBall)
    {
        //To DO
    }
}
