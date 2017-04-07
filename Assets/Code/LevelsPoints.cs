using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelsPoints{
    public static Vector3[][] levelsPoints = new Vector3[1][] { new Vector3[] { new Vector3(-14.94557f, 5.414517f, 4.941434E-08f), new Vector3(-7.751179f, 8.508108f, 4.181991E-07f), new Vector3(0.6302816f, 6.133959f, 1.351784E-07f), new Vector3(2.968457f, 3.004404f, -2.378935E-07f), new Vector3(12.1413f, 2.680651f, -2.764879E-07f) } };

    public static Vector3 GetLevelStartPoint(int level)
    {
        return levelsPoints[level][0];
    }

    public static int GetNextLevelPoint(int actualPointNumber, int actualLevel, float forwardBackward)
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
