using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementManager {

    //const
    private const float distanceToChangeDestination = 0.01f;

    public static bool MoveBalls(BallQueue queue, int actualLevel)
    {
        foreach (GameObject ball in queue.ballQueue)
        {
            BallScript ballScript = ball.GetComponent<BallScript>();
            Vector3 dest = LevelsPoints.levelsPoints[actualLevel][ballScript.ballObj.destination];
            Vector3 source = LevelsPoints.levelsPoints[actualLevel][ballScript.ballObj.source];

            CheckAndSetSpeed(ballScript.ballObj);

            float calculatedSpeed = (Time.time - ballScript.ballObj.lastPointTime) * queue.speed/ Vector3.Distance(dest, source);

            Vector3 newPosition =  Vector3.Lerp(source, dest, calculatedSpeed);
            ball.transform.position = newPosition;

            if (Vector3.Distance(newPosition, dest) < distanceToChangeDestination)
            {
                if ((ballScript.ballObj.destination = LevelsPoints.GetNextLevelPoint(ballScript.ballObj.destination, actualLevel, queue.forwardBackward)) == -1)
                {
                    return false;
                }
                else {
                    SetSpeed(ballScript.ballObj);
                }
            }
        }

        return true;
    }

    private static void CheckAndSetSpeed(BallObject ballObj)
    {
        if (ballObj.lastPointTime == 0.0f)
        {
            SetSpeed(ballObj);
        }
    }

    private static void SetSpeed(BallObject ballObj)
    {
        ballObj.lastPointTime = Time.time;
    }
}
