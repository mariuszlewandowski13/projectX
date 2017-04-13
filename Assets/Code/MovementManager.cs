using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementManager {

    private static bool playing;

    private static float safeDistance = 0.1f;

    public static bool MoveBalls(BList balls, float speed = 0.02f)
    {
        playing = true;
        GameObject actualBall = balls.InitEnumerationFromLeft();
        do
        {
            MoveBall(actualBall, speed);
        } while ((actualBall = balls.Next()) != null);

        return playing;
    }


    private static void MoveBall(GameObject ball, float speed)
    {
        BallObject ballObj = ball.GetComponent<BallScript>().ballObj;
        if (ballObj.lerpVector == new Vector3())
        {
            CalculateLerpVector(ballObj, ball,  speed);
        }

        ball.transform.position += ballObj.lerpVector;

        if (Vector3.Distance(ball.transform.position, ballObj.destinationPosition) < safeDistance)
        {
            ChangeToNextDestination(ballObj);
        }
    }

    private static void CalculateLerpVector(BallObject ballObj, GameObject ball, float speed)
    {
        ballObj.lerpVector = (Vector3.Lerp(ballObj.sourcePosition, ballObj.destinationPosition, speed) - ball.transform.position)/ Vector3.Distance(ballObj.sourcePosition, ballObj.destinationPosition);
    }

    private static void ChangeToNextDestination(BallObject ballObj)
    {
        if ((ballObj.destination = LevelsPoints.GetNextLevelPoint(ballObj.destination, ballObj.forwardBackward)) < 0)
        {
            playing = false;
        }
    }
}
