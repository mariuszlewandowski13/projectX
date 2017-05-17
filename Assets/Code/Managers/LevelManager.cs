using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LevelManager {
    private static Vector3[][] levelsPoints = new Vector3[3][] { new Vector3[] { new Vector3(-7.564834f, 7.664611f, 5.904689f), new Vector3(-4.516032f, 7.349945f, 8.571892f), new Vector3(-2.2593f, 7.482811f, 9.397152f), new Vector3(2.616142f, 7.473066f, 9.291935f), new Vector3(4.881168f, 7.91928f, 8.18164f), new Vector3(7.207488f, 7.853437f, 6.250022f) }, new Vector3[] { new Vector3(8.031924f, 6.553185f, 5.685169f), new Vector3(7.514421f, 5.767814f, 6.43679f), new Vector3(5.925914f, 5.729343f, 7.938583f), new Vector3(2.550532f, 5.50566f, 9.5905f), new Vector3(-1.032839f, 5.063087f, 9.8887f), new Vector3(-4.186964f, 4.944766f, 9.02226f) }, new Vector3[]{ new Vector3(-6.895947f, 4.88258f, 7.179384f), new Vector3(-7.56743f, 3.969829f, 6.28246f), new Vector3(-6.95577f, 3.21101f, 6.910719f), new Vector3(-5.835962f, 2.823397f, 7.686564f), new Vector3(-3.733037f, 3.247093f, 9.071382f), new Vector3(-1.932795f, 3.800519f, 9.674171f), new Vector3(-0.1699996f, 3.114975f, 9.762609f), new Vector3(-0.1700001f, 2.430832f, 9.5551f), new Vector3(0.544208f, 1.884024f, 9.352242f), new Vector3(2.081858f, 2.317025f, 9.286809f), new Vector3(2.697625f, 3.481215f, 9.409303f), new Vector3(3.815552f, 4.057912f, 9.122285f), new Vector3(5.558399f, 3.434025f, 8.093383f), new Vector3(5.577693f, 2.776419f, 7.880381f), new Vector3(6.098675f, 2.354755f, 7.288097f), new Vector3(7.259456f, 2.940452f, 6.412518f), new Vector3(7.562784f, 3.968895f, 6.287784f), new Vector3(8.372791f, 5.045178f, 5.423683f), new Vector3(8.668456f, 6.182771f, 4.681244f), new Vector3(8.528429f, 7.401023f, 4.563955f), new Vector3(7.716811f, 8.140314f, 5.492933f), new Vector3(7.738071f, 8.879885f, 5.096682f) } };
    private static int[] levelMaxPointCount = new int[] { 100, 100, 100 };
    private static int[] levelStartBallsCount = new int[] { 15, 15, 15 };

    private static int _actualLevel = 0;
    public static int actualLevel
        {
        get {
            return _actualLevel;
        }
        private set {
            _actualLevel = value;
        }

        }
    private static int lastLevel = 2;

    public static Vector3 GetPointByIndex(int i)
    {
        try {
            return levelsPoints[actualLevel][i];
        } catch (Exception)
        {
            return new Vector3();
        }
    }

    public static Vector3 GetLevelStartPoint()
    {
        return levelsPoints[actualLevel][0];   
    }

    public static int GetNextLevelPoint(int actualPointNumber, float forwardBackward)
    {
        if (forwardBackward > 0.0f)
        {
            if (actualPointNumber < 0)
            {
                actualPointNumber = 0;
            }
            else {
                actualPointNumber++;
            }
                
        }
        else {
            actualPointNumber--;
        }
        
        if (actualPointNumber >= levelsPoints[actualLevel].Length)
        {
            return -1;
        }

        if (actualPointNumber < 0)
        {
            return -2;
        }

        return actualPointNumber;
    }

    public static Vector3[] GetLevelPoints()
    {
        return levelsPoints[actualLevel];
    }

    public static int GetLevelMaxBallsCount()
    {
        return levelMaxPointCount[actualLevel];
    }

    public static int GetLevelStartBallsCount()
    {
        return levelStartBallsCount[actualLevel];
    }

    public static bool NextLevel()
    {
        if (actualLevel < lastLevel)
        {
            actualLevel++;
            return true;
        }
        return false;
    }

    public static void Clear()
    {
        actualLevel = 0;
    }
}
