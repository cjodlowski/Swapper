using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorPicker : MonoBehaviour
{
    private Color[] colors;

    private static Color defaultColor = new Color(0,0,0); // purple

    private List<Color> colorsLeft;

    public static PlayerColorPicker Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        colors = new Color[7] {
            hexToColor("#fe00fe"), // pink
            hexToColor("#ff2a6d"), // red
            hexToColor("#ff8600"), // orange
            hexToColor("#defe47"), // yellow
            hexToColor("#01fe01"), // green
            hexToColor("#0016ee"), // blue
            hexToColor("#7700a6") // purple
        };
        colorsLeft = new List<Color>(colors);

        Instance = this;
    }
    private Color hexToColor(string s)
    {
        if (ColorUtility.TryParseHtmlString(s, out Color color))
        {
            return color;
        }
        return defaultColor;
    }

    public Color PickColor()
    {
        if (colorsLeft.Count == 0)
        {
            return defaultColor;
        }
        int randomIdx = Random.Range(0, colorsLeft.Count);
        Color c = colorsLeft[randomIdx];
        colorsLeft.RemoveAt(randomIdx);
        return c;
    }
}
