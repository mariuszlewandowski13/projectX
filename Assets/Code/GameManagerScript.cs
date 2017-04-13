using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    #region Public Properties
    public GameObject ballPrefab;
    #endregion

    #region Private Properties

    private const float spawningSafeDistance = 0.4f;

    private BList balls;

    private Vector3 levelSpawningPosition;

    public static bool playing;

    #endregion


    private void Start()
    {
        Init();
        playing = false;
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
        newBall.tag = "Ball";
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
        balls.Insert(newBall, collidingBall, isRight);
        ChangeBallsDirectionOnInsert(newBall, positionForFirstBall, positionForNewBall);
        MovementManager.specialMove = true;
        newBall.tag = "Ball";
    }

    public bool CheckIfIsRightAndFindNewPositions(GameObject newBall, GameObject collidingBall,  out Vector3 posForNewBall, out Vector3 posForFirstBall)
    {
        posForFirstBall = FindNewPositionForFirestBallOnBallAdding();
        BListObject collidingBLisObj = balls.Find(collidingBall);
        return GetPosForNewBall(collidingBLisObj, newBall, out posForNewBall, posForFirstBall);
    }

    public Vector3 FindNewPositionForFirestBallOnBallAdding()
    {
        GameObject first = balls.First;
        BallObject ballObj = first.GetComponent<BallScript>().ballObj;
        Vector3 newPos = first.transform.position;
        do
        {
            newPos += ballObj.lerpVector;
        } while (Vector3.Distance(first.transform.position, newPos) < spawningSafeDistance);

        return newPos;
    }

    public bool GetPosForNewBall(BListObject collidingObject, GameObject newBall, out Vector3 posForNewBall, Vector3 posForFirstBall)
    {
        bool isRight = false;
        float distToLeft = (collidingObject.leftNeighbour != null ? Vector3.Distance(collidingObject.leftNeighbour.value.transform.position, newBall.transform.position) : Vector3.Distance(levelSpawningPosition, newBall.transform.position));
        float distToRight = (collidingObject.rightNeighbour!= null ? Vector3.Distance(collidingObject.rightNeighbour.value.transform.position, newBall.transform.position) : Vector3.Distance(posForFirstBall, newBall.transform.position)) ;

        if (distToLeft < distToRight)
        {
            isRight = true;
            posForNewBall = collidingObject.value.transform.position;
        }
        else {
            posForNewBall = (collidingObject.rightNeighbour != null ? collidingObject.rightNeighbour.value.transform.position : posForFirstBall);
        }

        newBall.GetComponent<BallScript>().ballObj.destination = collidingObject.value.GetComponent<BallScript>().ballObj.destination;

        return isRight;
    }

    private void ChangeBallsDirectionOnInsert(GameObject newBall, Vector3 positionForFirstBall, Vector3 posForNewBall)
    {
        BListObject actual = balls.InitEnumerationFromRightBListObject();

        BallObject ballObj = actual.value.GetComponent<BallScript>().ballObj;

        ballObj.destination --;
        ballObj.destinationPosition = positionForFirstBall;
        ballObj.IncreaseSpeedLevel();
        if (actual.value != newBall)
        {
            while ((actual = balls.PreviousBListObject()) != null && actual.value != newBall)
            {
                ballObj = actual.value.GetComponent<BallScript>().ballObj;
                ballObj.destination--;
                ballObj.destinationPosition = actual.rightNeighbour.value.transform.position;
                ballObj.IncreaseSpeedLevel();
            }

            ballObj = actual.value.GetComponent<BallScript>().ballObj;
            ballObj.destination--;
            ballObj.destinationPosition = posForNewBall;
            ballObj.IncreaseSpeedLevel();
        }
    }
}
