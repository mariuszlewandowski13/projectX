using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ApplicationData {

    public static Color[] ballsColors = new Color[] { Color.red, Color.green, Color.yellow, Color.blue, Color.white };

    private static int actualCounter = 0;
    private static int counterTreshold = -1;

    private static Color actualColor;

    private static System.Random ran = new System.Random();
    private static System.Random ran2 = new System.Random();

    private static object indexLock = new object();
    private static int ballIndex = 0;


    public static Color RandomNewColorForBoard()
    {
        if (actualCounter >= counterTreshold)
        {
            UpdateColor();
            UpdateCounter();
        }
        actualCounter++;
        return actualColor;
    }

    public static Color RandomNewColorForPlayer(List<Color> colors = null)
    {
        if (colors == null)
        {
            colors = GameObject.Find("GameManager").GetComponent<GameManagerScript>().GetAllColorsOnBoard();
        }
            
            if (colors.Count > 0)
            {
                return colors[ran.Next(0, colors.Count)];
            }
            else {
                return RandomNewColorForBoard();
            }
    }

    private static void UpdateCounter()
    {
        actualCounter = 0;
        counterTreshold = ran2.Next(1, ran2.Next(2,5));
    }

    private static void UpdateColor()
    {
        int index;

        do {
            index = ran.Next(0, ballsColors.Length);
        }while(actualColor == ballsColors[index]);

        actualColor = ballsColors[index];
    }

    public static int GetBallIndex()
    {
        int index;
        lock(indexLock)
        {
            index = ballIndex;
            ballIndex++;
        }

        return index;
    }
}
