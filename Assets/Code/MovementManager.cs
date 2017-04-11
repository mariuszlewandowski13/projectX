using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementManager {

    //const
    private const float distanceToChangeDestination = 0.01f;

    private static int counter = 0;

    private static float firstFrameTime = 0.0f;
    private static float timeBetweenFrames = 0.0f;

    private static GameObject flagObjectToDecreaseSpeed;

    private static bool decreasingSpeed;

    public static bool MoveBalls(Queue<GameObject> queue)
    {
        if (timeBetweenFrames == 0.0f)
        {
            if (firstFrameTime == 0.0f)
            {
                firstFrameTime = Time.time;
                return true;
            }
            timeBetweenFrames = Time.time - firstFrameTime;
        }

        if (flagObjectToDecreaseSpeed != null)
        {
            decreasingSpeed = true;
        }

        foreach (GameObject ball in queue)
        {
            if (decreasingSpeed)
            {
                if (ball == flagObjectToDecreaseSpeed)
                {
                    decreasingSpeed = false;
                    flagObjectToDecreaseSpeed = null;
                }
                else {
                    ball.GetComponent<BallScript>().ballObj.counterIncreaser -= 0.1f;
                }
            }

            BallScript ballScript = ball.GetComponent<BallScript>();
            Vector3 dest = ballScript.ballObj.destinationPosition;
            Vector3 source = ballScript.ballObj.sourcePosition;

                float calculatedLerp = timeBetweenFrames / Vector3.Distance(dest, source);
                //CheckAndSetLerp(ballScript.ballObj, calculatedLerp);

                Vector3 newPosition = Vector3.Lerp(source, dest, calculatedLerp*ballScript.ballObj.counter);
                ball.transform.position = newPosition;
                ballScript.ballObj.counter += ballScript.ballObj.counterIncreaser;
                if (Vector3.Distance(newPosition, dest) < distanceToChangeDestination)
                {
                    if ((ballScript.ballObj.destination = LevelsPoints.GetNextLevelPoint(ballScript.ballObj.destination, ballScript.ballObj.forwardBackward)) == -1)
                    {
                        return false;
                    }
                    else
                    {

                        if (ballScript.ballObj.isChangingSpeed)
                        {
                            ballScript.ballObj.isChangingSpeed = false;
                            flagObjectToDecreaseSpeed = ball;
                        }

                        ballScript.ballObj.counter = 0;
                        SetLerp(ballScript.ballObj, 0.0f);
                    }
                }
            }
        counter++;
        return true;
    }

    private static void CheckAndSetLerp(BallObject ballObj, float lerp)
    {
        //if (ballObj.actualLerp == 0.0f)
        //{
        //    SetLerp(ballObj, lerp);
        //}
    }

    private static void SetLerp(BallObject ballObj, float lerp)
    {
       // ballObj.actualLerp = lerp;
    }
}
