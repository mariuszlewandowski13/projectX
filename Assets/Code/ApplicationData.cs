using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ApplicationData {

    public static Color[] ballsColors = new Color[] { Color.red, Color.green, Color.yellow, Color.blue, Color.white };


    public static Color RandomNewColor()
    {
        System.Random ran = new System.Random();
        int index = ran.Next(0, ballsColors.Length);
        return ballsColors[index];
    }
}
