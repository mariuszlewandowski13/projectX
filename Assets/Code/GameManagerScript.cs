using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    #region Public Properties
    public GameObject ballPrefab;
    #endregion

    #region Private Properties

    private float spawningSafeDistance;

    private BList balls;

    private Vector3 levelSpawningPosition;

    public static bool playing;

    private List<BListObject> ballsThatCouldBeInSequence;
    private List<BListObject> ballsThatHasToReturnWithSequence;

    #endregion


    private void Start()
    {
        Init();
        playing = true;
    }


    private void Init()
    {
        spawningSafeDistance = ballPrefab.transform.lossyScale.y;
        balls = new BList();
        levelSpawningPosition = LevelsPoints.GetLevelStartPoint();
        ballsThatCouldBeInSequence = new List<BListObject>();
        ballsThatHasToReturnWithSequence = new List<BListObject>();
    }

    private void Update()
    {
        CheckAndCreateNewBall();
        CheckAndRemoveBallsInSequence();
    }

    private void CheckAndRemoveBallsInSequence()
    {
        List<BListObject> removedBalls = new List<BListObject>();
        foreach (BListObject ball in ballsThatCouldBeInSequence)
        {
            if (!ball.value.GetComponent<BallScript>().ballObj.specialMove)
            {
                CheckAndRemoveBallsByColor(ball.value);
                removedBalls.Add(ball);
            }
        }

        foreach (BListObject ball in removedBalls)
        {
            ballsThatCouldBeInSequence.Remove(ball);
        }

        ReturnBallsAfterRemovingBallsInSequence();

    }

    private void ReturnBallsAfterRemovingBallsInSequence()
    {
        foreach (BListObject ball in ballsThatHasToReturnWithSequence)
        {
            SetBallsToReturn(ball);
            //MovementManager.specialMove = true;
        }
        ballsThatHasToReturnWithSequence.Clear();
        
    }

    private void SetBallsToReturn(BListObject ball)
    {
        while (ball != null)
        {
            ball.value.GetComponent<BallScript>().ballObj.forwardBackward = -ball.value.GetComponent<BallScript>().ballObj.forwardBackward;
            ball.value.GetComponent<BallScript>().ballObj.destination--;
            ball.value.GetComponent<BallScript>().ballObj.IncreaseSpeedLevel();
            ball = ball.rightNeighbour;
        }
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
        ballsThatCouldBeInSequence.Add( balls.Insert(newBall, collidingBall, isRight));
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
        int dest = 0;
        Vector3 newPos = first.transform.position;
        do
        {
            if (ballObj.lerpVector == new Vector3())
            {
                MovementManager.CalculateLerpVector(ballObj);
            }
            newPos += ballObj.lerpVector;

            if (Vector3.Distance(newPos, ballObj.destinationPosition) < MovementManager.safeDistance)
            {
                MovementManager.ChangeToNextDestination(ballObj, false);
                dest++;
            }

        } while (Vector3.Distance(first.transform.position, newPos) < spawningSafeDistance);

        if (dest > 0)
        {
            ballObj.destination -= dest;
        }

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

        bool curveChange = false;

        ballObj.destination--;
        ballObj.destinationPosition = positionForFirstBall;
        ballObj.IncreaseSpeedLevel();
        if (actual.value != newBall)
        {
            while ((actual = balls.PreviousBListObject()) != null && actual.value != newBall)
            {
                ballObj = actual.value.GetComponent<BallScript>().ballObj;
                ballObj.destination--;
                if (actual.rightNeighbour.value.GetComponent<BallScript>().ballObj.destination > ballObj.destination && !curveChange)
                {
                    ballObj.destination++;
                    curveChange = true;
                }
                else if (curveChange)
                {
                    curveChange = false;
                }

                ballObj.destinationPosition = actual.rightNeighbour.value.transform.position;
                ballObj.IncreaseSpeedLevel();
            }
            ballObj = actual.value.GetComponent<BallScript>().ballObj;
            ballObj.destination--;
            ballObj.destinationPosition = posForNewBall;
            ballObj.IncreaseSpeedLevel();
        }
    }

    private void CheckAndRemoveBallsByColor(GameObject ballInSequence)
    {
        if (CheckBallsToRemove(ballInSequence))
        {
            RemoveBallsInSequence(ballInSequence);
        }
    }

    private bool CheckBallsToRemove(GameObject ballInSequence)
    {
        Color color = ballInSequence.GetComponent<BallScript>().ballObj.color;
        BListObject listObject = balls.Find(ballInSequence);
        if (listObject.rightNeighbour != null)
        {
            if (listObject.leftNeighbour != null)
            {
                if (listObject.rightNeighbour.value.GetComponent<BallScript>().ballObj.color == color && listObject.leftNeighbour.value.GetComponent<BallScript>().ballObj.color == color)
                {
                    return true;
                }
            }

            if (listObject.rightNeighbour.rightNeighbour != null)
            {
                if (listObject.rightNeighbour.value.GetComponent<BallScript>().ballObj.color == color && listObject.rightNeighbour.rightNeighbour.value.GetComponent<BallScript>().ballObj.color == color)
                {
                    return true;
                }
            }
        } else if (listObject.leftNeighbour != null)
        {
            if (listObject.leftNeighbour.leftNeighbour != null)
            {
                if (listObject.leftNeighbour.value.GetComponent<BallScript>().ballObj.color == color && listObject.rightNeighbour.leftNeighbour.leftNeighbour.value.GetComponent<BallScript>().ballObj.color == color)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private List<BListObject> FindBallsInSequence(GameObject ballInSequence)
    {
        List<BListObject> ballsToRemove = new List<BListObject>();
        Color color = ballInSequence.GetComponent<BallScript>().ballObj.color;

        BListObject pivotBall = balls.Find(ballInSequence);
        BListObject actualListObj = pivotBall;

        while (actualListObj != null && actualListObj.value.GetComponent<BallScript>().ballObj.color == color)
        {
            ballsToRemove.Add(actualListObj);
            actualListObj = actualListObj.rightNeighbour;
        }

        if (actualListObj != null)
        {
            ballsThatHasToReturnWithSequence.Add(actualListObj);
            actualListObj.value.AddComponent<BallReturningScript>();
            Rigidbody rigid = actualListObj.value.AddComponent<Rigidbody>();
            rigid.useGravity = false;
            rigid.isKinematic = false;
            rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        actualListObj = pivotBall.leftNeighbour;

        while (actualListObj != null && actualListObj.value.GetComponent<BallScript>().ballObj.color == color)
        {
            ballsToRemove.Add(actualListObj);
            actualListObj = actualListObj.leftNeighbour;
        }

        return ballsToRemove;
    }
    private void RemoveBallsInSequence(GameObject ballInSequence)
    {
        List<BListObject> ballsToRemove = FindBallsInSequence(ballInSequence);
        foreach (BListObject ball in ballsToRemove)
        {
            balls.Remove(ball);
            Destroy(ball.value);
        }

    }

    public void ChangeBallDirection(GameObject ball)
    {
        BListObject listObj = balls.Find(ball);
            while (listObj != null)
            {
            listObj.value.GetComponent<BallScript>().ballObj.forwardBackward = -listObj.value.GetComponent<BallScript>().ballObj.forwardBackward;
            listObj.value.GetComponent<BallScript>().ballObj.destination ++;
            listObj.value.GetComponent<BallScript>().ballObj.DecreaseSpeedLevel();
            listObj = listObj.rightNeighbour;
            }

        ballsThatCouldBeInSequence.Add(balls.Find(ball));

    }
}
