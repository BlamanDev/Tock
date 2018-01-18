using System.Collections.Generic;
using UnityEngine;

public static class ColorManager  {
    static private List<Color> colors = new List<Color>(){ Color.blue, Color.red, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.gray, Color.black};
    static private List<Color> colorsAlreadyUsed = new List<Color>();

    static public Color GiveADefinedColor()
    {
        Color givenColor = new Color();
        if (colors.Count > 0)
        {
            givenColor = colors[0];
            colors.Remove(givenColor);
            colorsAlreadyUsed.Add(givenColor);
        }
        else
        {
            givenColor = Random.ColorHSV();
        }
        return givenColor;
    }

    static public Color GiveRandomColor()
    {
        return Random.ColorHSV(0f, 255/255, 100/255, 255 / 255, 255 / 255, 255 / 255);
    }

    static public void ReturnColor(Color colorReturned)
    {
        if (!colors.Contains(colorReturned) && colorReturned != Color.clear && colorsAlreadyUsed.Contains(colorReturned))
        {
            colors.Add(colorReturned);
            colorsAlreadyUsed.Remove(colorReturned);
        }
    }

    static public Color ChangeColor(Color previousColor, bool random)
    {
        ReturnColor(previousColor);
        Color newColor;
        if (random)
        {
            newColor = GiveRandomColor();
        }
        else
        {
            newColor = GiveADefinedColor();
        }
        return newColor;
    }
}
