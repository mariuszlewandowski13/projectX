using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    #region Public Properties
    public GameObject ballPrefab;
    public GameObject board;
    #endregion

    #region Private Properties

    private float spawningSafeDistance;

    private BList balls;

    private Vector3 levelSpawningPosition;

    public static bool playing;
    public bool levelEnded;
    public bool lost;
    public bool levelToStart;

    private List<BListObject> ballsThatCouldBeInSequence;
    private List<BListObject> ballsThatHasToReturnWithSequence;

    private static int ballsCount;
    private static int maxBallsCount;

    public static bool canInstantiate
    {
        get {
            return ballsCount < maxBallsCount;
        }
    }

    #endregion


    private void Start()
    {
        Clear();
        playing = false;
        LoadLevel();
        AnimationsManager.PlayLevelStartAnimation(LevelManager.actualLevel);
        levelToStart = true;
    }


    private void Clear()
    {
        spawningSafeDistance = ballPrefab.transform.lossyScale.y;
        balls = new BList();
        ballsThatCouldBeInSequence = new List<BListObject>();
        ballsThatHasToReturnWithSequence = new List<BListObject>();
        ballsCount = 0;
        lost = false;
        levelEnded = false;
    }

    private void LateUpdate()
    {
        CheckWinningConditions();
        CheckAndCreateNewBall();
        CheckAndRemoveBallsInSequence();
        CheckAndLoadNewLevel();
        CheckAndPlay();
    }

    private void CheckAndPlay()
    {
        if (!playing && !levelEnded && levelToStart && !AnimationsManager.levelStartAnimation)
        {
            levelToStart = false;
            playing = true;
        }
    }



    private void CheckAndLoadNewLevel()
    {
        if (levelEnded && !AnimationsManager.levelEndedAnimation)
        {
            Clear();
            if (LevelManager.NextLevel())
            {
                //LoadLevel();
               // AnimationsManager.PlayLevelStartAnimation(LevelManager.actualLevel);
                //levelToStart = true;
                GameObject.Find("Menu").transform.FindChild("NextLevel").gameObject.SetActive(true);
            }
            else {
                AnimationsManager.PlayGameWonAnimation();
            }
        }
    }

    private void CheckWinningConditions()
    {
        if (playing)
        {
            if (!canInstantiate && balls.count == 0)
            {
                Debug.Log("Won!!!");
                playing = false;
                levelEnded = true;
                AnimationsManager.PlayLevelEndedAnimation(LevelManager.actualLevel);
            }
        }
        if (lost)
        {
            Debug.Log("Lost!!!");
            AnimationsManager.PlayGameLostAnimation();
            lost = false;
        }
        
    }

    private void CheckAndRemoveBallsInSequence()
    {
        if (!MovementManager.specialMove)
        {
            List<BListObject> removedBalls = new List<BListObject>();
            foreach (BListObject ball in ballsThatCouldBeInSequence)
            {
                if (!ball.value.GetComponent<BallScript>().ballObj.specialMove)
                {
                    CheckAndRemoveBallsByColor(ball);
                    removedBalls.Add(ball);
                }
            }

            foreach (BListObject ball in removedBalls)
            {
                ballsThatCouldBeInSequence.Remove(ball);
            }

            ReturnBallsAfterRemovingBallsInSequence();
        }
    }

    private void ReturnBallsAfterRemovingBallsInSequence()
    {
        foreach (BListObject ball in ballsThatHasToReturnWithSequence)
        {
            if ((canInstantiate) || (balls.Last != ball.value))
            {
                SetBallsToReturn(ball);
            }
            
           // MovementManager.specialMove = true;
        }
        ballsThatHasToReturnWithSequence.Clear();
    }

    private void SetBallsToReturn(BListObject ball)
    {
        while (ball != null)
        {
            ball.value.GetComponent<BallScript>().ballObj.forwardBackward = -ball.value.GetComponent<BallScript>().ballObj.forwardBackward;
            ball.value.GetComponent<BallScript>().ballObj.destination--;
            ball.value.GetComponent<BallScript>().ballObj.IncreaseSpeedLevel(false);
            //Debug.Log(ball.value.name);
            ball = ball.rightNeighbour; 
        }
    }

    private void CheckAndCreateNewBall()
    {
        if (playing)
        {    
            playing = MovementManager.MoveBalls(balls, spawningSafeDistance);
            lost = !playing;
            if (CheckSpawningSafe())
            {
                CreateNewBall();
            }
        }

    }

    private void CreateNewBall()
    {
        GameObject newBall = Instantiate(ballPrefab, levelSpawningPosition, new Quaternion());
        newBall.GetComponent<BallScript>().SetBallObject(ApplicationData.RandomNewColorForBoard());
        newBall.name = "Ball" + ApplicationData.GetBallIndex().ToString();
        newBall.tag = "Ball";
        balls.AppendLast(newBall);
        ballsCount++;
    }

    private bool CheckSpawningSafe()
    {
        if ((canInstantiate) && (balls.Last == null || Vector3.Distance(balls.Last.transform.position, levelSpawningPosition) >= spawningSafeDistance ) && !MovementManager.specialMove)
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
            MovementManager.CalculateLerpVector(ballObj);
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

    private void CheckAndRemoveBallsByColor(BListObject ballInSequence)
    {
        if (CheckBallsToRemove(ballInSequence))
        {
            RemoveBallsInSequence(ballInSequence.value);
        }
    }

    private bool CheckBallsToRemove(BListObject ballInSequence)
    {
        Color color = ballInSequence.value.GetComponent<BallScript>().ballObj.color;
        BListObject listObject = ballInSequence;

        if (listObject.rightNeighbour != null)
        {
            if (listObject.leftNeighbour != null)
            {
                if (listObject.rightNeighbour.value.GetComponent<BallScript>().ballObj.color == color && listObject.leftNeighbour.value.GetComponent<BallScript>().ballObj.color == color)
                {
                    return true;
                }
            }

            if (listObject.rightNeighbour.rightNeighbour != null && !ballInSequence.mustBeCenterPoint)
            {
                if (listObject.rightNeighbour.value.GetComponent<BallScript>().ballObj.color == color && listObject.rightNeighbour.rightNeighbour.value.GetComponent<BallScript>().ballObj.color == color)
                {
                    return true;
                }
            }
        }

        if (listObject.leftNeighbour != null)
        {
            if (listObject.leftNeighbour.leftNeighbour != null)
            {
                if (listObject.leftNeighbour.value.GetComponent<BallScript>().ballObj.color == color && listObject.leftNeighbour.leftNeighbour.value.GetComponent<BallScript>().ballObj.color == color)
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
        listObj.mustBeCenterPoint = true;
        while (listObj != null)
        {
            listObj.value.GetComponent<BallScript>().ballObj.forwardBackward = -listObj.value.GetComponent<BallScript>().ballObj.forwardBackward;
            listObj.value.GetComponent<BallScript>().ballObj.destination++;

            MovementManager.CalculateLerpVector(listObj.value.GetComponent<BallScript>().ballObj);
            if (listObj.leftNeighbour != null)
            {
                listObj.value.GetComponent<BallScript>().ballObj.actualSpeedLevel++;
                while (Vector3.Distance(listObj.value.transform.position, listObj.leftNeighbour.value.transform.position) < spawningSafeDistance)
                {
                    MovementManager.MoveBall(listObj);
                }
                listObj.value.GetComponent<BallScript>().ballObj.actualSpeedLevel--;
            }
            listObj.value.GetComponent<BallScript>().ballObj.DecreaseSpeedLevel();
            listObj = listObj.rightNeighbour;
        }
        ballsThatCouldBeInSequence.Add(balls.Find(ball));
    }

    public List<Color> GetAllColorsOnBoard()
    {
        List<Color> colors = new List<Color>();

        if (balls != null)
        {
            BListObject actual = balls.InitEnumerationFromLeftBListObject();
            if (actual != null)
            {
                do
                {
                    Color color = actual.value.GetComponent<BallScript>().ballObj.color;
                    if (!colors.Contains(color))
                    {
                        colors.Add(color);
                    }

                } while ((actual = balls.NextBListObject()) != null);
            }
        }
        return colors;
    }

    private void LoadLevel()
    {
        levelSpawningPosition = LevelManager.GetLevelStartPoint();
        maxBallsCount = LevelManager.GetLevelMaxBallsCount() ;
        LoadllevelGraphics();
        
    }

    private void LoadllevelGraphics()
    {
        Vector3[] levelPoints = LevelManager.GetLevelPoints();
        board.GetComponent<LineRenderer>().positionCount = levelPoints.Length;
        board.GetComponent<LineRenderer>().SetPositions(levelPoints);
    }


}
