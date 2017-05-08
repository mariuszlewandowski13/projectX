using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NewBallToAdd
{
    public GameObject newBall;
    public GameObject collidingBall;

    public NewBallToAdd(GameObject newBall, GameObject collidingBall)
    {
        this.newBall = newBall;
        this.collidingBall = collidingBall;
    }
}


public class GameManagerScript : MonoBehaviour {

    #region Public Events
    public delegate void ColorsChange(List<Color> colors);
    public static event ColorsChange deleteSequence;

    public delegate void BallsEvents(GameObject ball);
    public static event BallsEvents ballCreated;
    #endregion

    #region Public Properties
    public GameObject ballPrefab;
    public GameObject board;
    public static bool canShoot
    {
        get
        {
            return _canShoot && playing;
        }
    }
    #endregion

    #region Private Properties

    private static bool _canShoot;
    private static int spawningBallSpeedLevelOnStart = 3;
    private static int spawningBallSpeedLevelNormal = 0;
    private static int onStartBallsMaxCount;

    public static float spawningSafeDistance;

    private BList balls;
    private List<NewBallToAdd> ballsToAdd;

    public static Vector3 levelSpawningPosition;

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

    private object canAddBallLock = new object();

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
        _canShoot = false;
        ballsToAdd = new List<NewBallToAdd>();
    }

    private void Update()
    {
        CheckWinningConditions();
        CheckAndCreateNewBall();
        CheckAndRemoveBallsInSequence();
        CheckAndLoadNewLevel();
        CheckAndPlay();
        CheckAndAddNextBall();
    }

    private void CheckAndAddNextBall()
    {
        if (ballsToAdd.Count > 0)
        {
            NewBallToAdd ballToAddStruct = ballsToAdd[0];
            if (AddNewBallFromPlayer(ballToAddStruct.newBall, ballToAddStruct.collidingBall, true))
            {
                ballsToAdd.Remove(ballToAddStruct);
            }
        }
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
                LoadLevel();
                AnimationsManager.PlayLevelStartAnimation(LevelManager.actualLevel);
                levelToStart = true;
                //GameObject.Find("Menu").transform.FindChild("NextLevel").gameObject.SetActive(true);
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
                CheckStartSpeedLevel();
            }
        }
    }

    private void CheckStartSpeedLevel()
    {
        if (balls.starting && balls.count >= onStartBallsMaxCount)
        {
            balls.starting = false;
            ClearSpeedLevels(balls);
            _canShoot = true;
        }
    }

    private void ClearSpeedLevels(BList balls)
    {
        BListObject ball = balls.InitEnumerationFromLeftBListObject();
        if (ball != null)
        {
            do
            {
                ball.value.GetComponent<BallScript>().ballObj.RestartSpeedLevel();
            } while ((ball = balls.NextBListObject()) != null);
        }
    }

    private void CreateNewBall()
    {
        GameObject newBall = Instantiate(ballPrefab, levelSpawningPosition, new Quaternion());
        newBall.GetComponent<BallScript>().SetBallObject(ApplicationData.RandomNewColorForBoard(), (balls.starting? spawningBallSpeedLevelOnStart:spawningBallSpeedLevelNormal));
        newBall.name = "Ball" + ApplicationData.GetBallIndex().ToString();
        newBall.tag = "Ball";
        balls.AppendLast(newBall);
        ballsCount++;
        if (ballCreated != null)
        {
            ballCreated(newBall);
        }
    }

    private bool CheckSpawningSafe()
    {
        if ((canInstantiate) && (balls.Last == null || Vector3.Distance(balls.Last.transform.position, levelSpawningPosition) >= spawningSafeDistance ) && !MovementManager.specialMove)
        {
            return true;
        }
        else return false;
    }

    public bool AddNewBallFromPlayer(GameObject newBall, GameObject collidingBall, bool isOnList = false)
    {
        bool isAdded = false;
        lock(canAddBallLock)
        {
            if (!MovementManager.specialMove)
            {
                Vector3 positionForNewBall;
                Vector3 positionForFirstBall;
                bool isRight = MovementManager.CheckIfIsRightAndFindNewPositions(newBall, balls.Find(collidingBall), out positionForNewBall, out positionForFirstBall, balls.First);
                ballsThatCouldBeInSequence.Add(balls.Insert(newBall, collidingBall, isRight));
                ChangeBallsDirectionOnInsert(newBall, positionForFirstBall, positionForNewBall);
                MovementManager.specialMove = true;
                newBall.tag = "Ball";
                isAdded = true;
            }
            else {
                if (!isOnList)
                {
                    ballsToAdd.Add(new NewBallToAdd(newBall, collidingBall));
                }
                
            }
        }
        return isAdded;
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
            if (deleteSequence != null)
            {
                deleteSequence(GetAllColorsOnBoard());
            }
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
        maxBallsCount = LevelManager.GetLevelMaxBallsCount();
        onStartBallsMaxCount = LevelManager.GetLevelStartBallsCount();
        LoadllevelGraphics();
        
    }

    private void LoadllevelGraphics()
    {
        Vector3[] levelPoints = LevelManager.GetLevelPoints();
        board.GetComponent<LineRenderer>().positionCount = levelPoints.Length;
        board.GetComponent<LineRenderer>().SetPositions(levelPoints);
    }


}
