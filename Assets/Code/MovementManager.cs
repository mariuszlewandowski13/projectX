using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementManager {

    private static bool playing;

    public static float safeDistance = 0.041f;

    public static bool specialMove;

    private static bool changeSpecialMoveToFalse;

    public static bool MoveBalls(BList balls)
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
           
        }
        return playing;
    }

    public static void CheckBallsCorrectDistances(BList balls)
    {
        BListObject actualBall = balls.InitEnumerationFromLeftBListObject();
        if (actualBall != null)
        {
            do {
                if (actualBall.leftNeighbour != null)
                {
                    if (Vector3.Distance(actualBall.value.transform.position, actualBall.leftNeighbour.value.transform.position) < safeDistance)
                    {
                        actualBall.value.transform.position -= actualBall.value.GetComponent<BallScript>().ballObj.lerpVector;
                    }
                }

            } while ((actualBall = balls.NextBListObject())!= null);
        }
        
    }

    private static void MoveBall(BListObject ballListObj)
    {
        GameObject ball = ballListObj.value;
        BallObject ballObj = ball.GetComponent<BallScript>().ballObj;
        if (ballObj.lerpVector == new Vector3())
        {
            CalculateLerpVector(ballObj);
        }

        int movementSpeed = ballObj.forCounter;

        for (int i = 0; i < movementSpeed; ++i)
        {
            ball.transform.position += ballObj.lerpVector;
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

    public static void CalculateLerpVector(BallObject ballObj)
    {
        ballObj.lerpVector = (Vector3.Lerp(ballObj.sourcePosition, ballObj.destinationPosition, ballObj.speed) - ballObj.sourcePosition) / Vector3.Distance(ballObj.sourcePosition, ballObj.destinationPosition); 
    }

    public static void ChangeToNextDestination(BallObject ballObj, bool decrease = true)
    {
        if ((ballObj.destination = LevelsPoints.GetNextLevelPoint(ballObj.destination, ballObj.forwardBackward)) < 0)
        {
            playing = false;
        }
        if (decrease && ballObj.forwardBackward > 0)
        {
            ballObj.DecreaseSpeedLevel();
        }
        
    }
}
