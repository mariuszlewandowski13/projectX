using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LevelsPoints{
    private static Vector3[][] levelsPoints = new Vector3[1][] { new Vector3[] { new Vector3(-14.94557f, 5.414517f, 4.941434E-08f), new Vector3(-7.751179f, 8.508108f, 4.181991E-07f), new Vector3(0.6302816f, 6.133959f, 1.351784E-07f), new Vector3(2.968457f, 3.004404f, -2.378935E-07f), new Vector3(12.1413f, 2.680651f, -2.764879E-07f) } };

    public static int actualLevel = 0;

    public static Vector3 GetPointByIndex(int i)
    {
        try {
            return levelsPoints[actualLevel][i];
        } catch (Exception)
        {
            return new Vector3();
        }

       
    }

    public static GameObject GetLevelStartPoint()
    {
        GameObject levelSpawningPosition = new GameObject();
        levelSpawningPosition.transform.position = levelsPoints[actualLevel][0];

        return levelSpawningPosition;   
    }

    public static int GetNextLevelPoint(int actualPointNumber, float forwardBackward)
    {
        if (forwardBackward > 0.0f)
        {
            actualPointNumber++;
        }
        else {
            actualPointNumber--;
        }
        
        if (actualPointNumber >= levelsPoints[actualLevel].Length)
        {
            return -1;
        }

        return actualPointNumber;
    }
}
