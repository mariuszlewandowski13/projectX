using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementManager {

    private static bool playing;

    public static float safeDistance = 0.01f;

    public static bool specialMove;

    private static bool changeSpecialMoveToFalse;

    public static bool MoveBalls(BList balls, float safeDistance)
    {
        changeSpecialMoveToFalse = specialMove;
        playing = true;
        BListObject actualBall = balls.InitEnumerationFromLeftBListObject();
        if (actualBall != null)
        {
            do
            {
                if (!specialMove || actualBall.value.GetComponent<BallScript>().ballObj.specialMove)
                    MoveBall(actualBall);
            } while ((actualBall = balls.NextBListObject()) != null);
        }
        

        if (changeSpecialMoveToFalse)
        {
            changeSpecialMoveToFalse = false;
            specialMove = false;
            CheckBallsCorrectDistances(balls, 0.4f);
        }
        return playing;
    }

    public static void CheckBallsCorrectDistances(BList balls, float safeDistance )
    {
        BListObject actualBall = balls.InitEnumerationFromLeftBListObject();
        if (actualBall != null)
        {
            do {
                if (actualBall.leftNeighbour != null)
                {
                    CalculateLerpVector(actualBall.value.GetComponent<BallScript>().ballObj);
                    int prevSpeedLevel = actualBall.value.GetComponent<BallScript>().ballObj.actualSpeedLevel;
                    actualBall.value.GetComponent<BallScript>().ballObj.actualSpeedLevel = 2;

                    while (Vector3.Distance(actualBall.value.transform.position, actualBall.leftNeighbour.value.transform.position) < safeDistance)
                    {
                        MoveBall(actualBall);
                    }
                    actualBall.value.GetComponent<BallScript>().ballObj.actualSpeedLevel = prevSpeedLevel;
                }
            } while ((actualBall = balls.NextBListObject())!= null);
        }
        
    }

    public static void MoveBall(BListObject ballListObj)
    {
        GameObject ball = ballListObj.value;
        BallObject ballObj = ball.GetComponent<BallScript>().ballObj;
        CalculateLerpVector(ballObj);

        int movementSpeed = ballObj.forCounter;

        for (int i = 0; i < movementSpeed; ++i)
        {
            CalculateLerpVector(ballObj);
            //ball.transform.position += ballObj.lerpVector;
            AddLerpVector(ball, ballObj);
            if (Vector3.Distance(ball.transform.position, ballObj.destinationPosition) <= safeDistance)
            {
                ChangeToNextDestination(ballObj);
            }
        }
       

        if (ballObj.specialMove == true)
        {
            changeSpecialMoveToFalse = false;
        }
    }

    private static void AddLerpVector(GameObject ball, BallObject ballObj)
    {
        Vector3 ballPos = ball.transform.position;
        bool change = false;
        if (Mathf.Abs(ball.transform.position.x - ballObj.destinationPosition.x) > Mathf.Abs(ballObj.lerpVector.x))
        {
            ballPos.x += ballObj.lerpVector.x;
            change = true;
        }

        if (Mathf.Abs(ball.transform.position.y - ballObj.destinationPosition.y) > Mathf.Abs(ballObj.lerpVector.y))
        {
            ballPos.y += ballObj.lerpVector.y;
            change = true;
        }

        if (Mathf.Abs(ball.transform.position.z - ballObj.destinationPosition.z) > Mathf.Abs(ballObj.lerpVector.z))
        {
            ballPos.z += ballObj.lerpVector.z;
            change = true;
        }

        if (change == true)
        {
            ball.transform.position = ballPos;
        }
        else {
            ball.transform.position = ballObj.destinationPosition;
        }
        


    }

    public static void CalculateLerpVector(BallObject ballObj)
    {
        if (ballObj.lerpVector == new Vector3())
        {
            ballObj.lerpVector = (Vector3.Lerp(ballObj.sourcePosition, ballObj.destinationPosition, ballObj.speed) - ballObj.sourcePosition) / Vector3.Distance(ballObj.sourcePosition, ballObj.destinationPosition);
        }
        
    }

    public static void ChangeToNextDestination(BallObject ballObj, bool decrease = true)
    {
        if ((ballObj.destination = LevelManager.GetNextLevelPoint(ballObj.destination, ballObj.forwardBackward)) < 0)
        {
            playing = false;
        }
        if (decrease && ballObj.forwardBackward > 0)
        {
            ballObj.DecreaseSpeedLevel();
        }
        
    }
}
