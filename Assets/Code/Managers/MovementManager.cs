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
            CheckBallsCorrectDistances(balls, GameManagerScript.spawningSafeDistance);
        }
        //CheckBallsCorrectDistances(balls, GameManagerScript.spawningSafeDistance);

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
                        while (GetDistance(actualBall.value, actualBall.leftNeighbour.value) < safeDistance)
                        {
                            MoveBall(actualBall);
                        }
                    actualBall.value.GetComponent<BallScript>().ballObj.actualSpeedLevel = prevSpeedLevel;
                }
            } while ((actualBall = balls.NextBListObject())!= null);
        }
        
    }

    private static float GetDistance(GameObject first, GameObject second)
    {
        if (first.GetComponent<BallScript>().ballObj.destination == second.GetComponent<BallScript>().ballObj.destination)
        {
            return Vector3.Distance(first.transform.position, second.transform.position);
        }
        else {
            return Vector3.Distance(first.transform.position, LevelManager.GetPointByIndex(first.GetComponent<BallScript>().ballObj.destination - 1)) + Vector3.Distance(second.transform.position, second.GetComponent<BallScript>().ballObj.destinationPosition);
        }

    }

    public static void MoveBall(BListObject ballListObj)
    {
        GameObject ball = ballListObj.value;
        BallObject ballObj = ball.GetComponent<BallScript>().ballObj;
        CalculateLerpVector(ballObj);

        int movementSpeed = ballObj.forCounter;

        bool special = ballObj.specialMove;
        for (int i = 0; i < movementSpeed; ++i)
        {
            CalculateLerpVector(ballObj);
            AddLerpVector(ball, ballObj);
            if (Vector3.Distance(ball.transform.position, ballObj.destinationPosition) <= safeDistance)
            {
                ChangeToNextDestination(ballObj);
                if (special) break;
            }
        }
        if (ballObj.specialMove)
        {
            changeSpecialMoveToFalse = false;
        }
    }

    private static void AddLerpVector(GameObject ball, BallObject ballObj)
    {
        Vector3 ballPos = ball.transform.position;
        bool change = false;

        float prevDistanceToDestinationPoint = Vector3.Distance(ballPos, ballObj.destinationPosition);

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

        float actualDistanceToDestinationPoint = Vector3.Distance(ballPos, ballObj.destinationPosition);

        if (change == true && !(actualDistanceToDestinationPoint >= prevDistanceToDestinationPoint))
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

    public static bool CheckIfIsRightAndFindNewPositions(GameObject newBall, BListObject collidingBall, out Vector3 posForNewBall, out Vector3 posForFirstBall, GameObject first)
    {
        posForFirstBall = FindNewPositionForFirstLastBallOnBallAdding(first, 1);
        Vector3 posForLastBall = FindNewPositionForFirstLastBallOnBallAdding(collidingBall.value, -1);

        return GetPosForNewBall(collidingBall, newBall, out posForNewBall, posForFirstBall, posForLastBall);
    }

    public static Vector3 FindNewPositionForFirstLastBallOnBallAdding(GameObject first, int firstLast)
    {
        BallObject ballObj = first.GetComponent<BallScript>().ballObj;
        int dest = 0;
        Vector3 newPos = first.transform.position;
        float distance = 0.0f;
        ballObj.forwardBackward *= firstLast;
        if (firstLast < 0)
        {
            ballObj.destination--;
            dest++;
        }
        do
        {
            CalculateLerpVector(ballObj);
            newPos += ballObj.lerpVector;
            distance += ballObj.lerpVector.magnitude;

            if (Vector3.Distance(newPos, ballObj.destinationPosition) < safeDistance)
            {
                ChangeToNextDestination(ballObj, false);
                dest++;
            }

        } while (distance < GameManagerScript.spawningSafeDistance);

        ballObj.forwardBackward *= firstLast;

        if (dest > 0)
        {
            ballObj.destination -= firstLast*dest;
        }


        return newPos;
    }
    public static bool GetPosForNewBall(BListObject collidingObject, GameObject newBall, out Vector3 posForNewBall, Vector3 posForFirstBall, Vector3 posForLastBall)
    {
        bool isRight = false;
        if (Vector3.Distance(collidingObject.value.transform.position, GameManagerScript.levelSpawningPosition) < GameManagerScript.spawningSafeDistance)
        {
            posForLastBall = GameManagerScript.levelSpawningPosition;
        }

        float distToLeft = (collidingObject.leftNeighbour != null ? Vector3.Distance(collidingObject.leftNeighbour.value.transform.position, newBall.transform.position) : Vector3.Distance(posForLastBall, newBall.transform.position));
        float distToRight = (collidingObject.rightNeighbour != null ? Vector3.Distance(collidingObject.rightNeighbour.value.transform.position, newBall.transform.position) : Vector3.Distance(posForFirstBall, newBall.transform.position));

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

}
