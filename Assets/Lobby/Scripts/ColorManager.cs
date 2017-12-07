using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorManager  {
    static private List<Color> colors = new List<Color>(){ Color.blue, Color.red, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white };

    static public Color GiveNewColor()
    {
        Color givenColor = new Color();
        if (colors.Count > 0)
        {
            System.Random pickAColor = new System.Random();
            givenColor = colors[pickAColor.Next(colors.Count)];
            colors.Remove(givenColor);
        }
        else
        {
            givenColor = Random.ColorHSV();
        }
        return givenColor;
    }

    static public void ReturnColor(Color colorReturned)
    {
        if (!colors.Contains(colorReturned) && colorReturned != Color.clear)
        {
            colors.Add(colorReturned);
        }
    }

    static public Color ChangeColor(Color previousColor)
    {
        ReturnColor(previousColor);
        return GiveNewColor();
    }
}
