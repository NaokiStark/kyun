using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using System.Text.RegularExpressions;

public static class StringHelper
{
    public static string SanitizeUnicode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";
        
        return Regex.Replace(input, @"[^\u0000-\u007F]+", string.Empty);
    }


    /// <summary>
    /// http://stackoverflow.com/questions/15986473/how-do-i-implement-word-wrap
    /// </summary>
    /// <param name="spriteFont"></param>
    /// <param name="text"></param>
    /// <param name="maxLineWidth"></param>
    /// <returns></returns>
    public static string WrapText(SpriteFont spriteFont, string text, float maxLineWidth, float scale = 1)
    {
        string[] words = text.Split();
        StringBuilder sb = new StringBuilder();
        float lineWidth = 0f;
        float spaceWidth = (spriteFont.MeasureString(" ") * scale).X;

        foreach (string word in words)
        {
            Vector2 size = spriteFont.MeasureString(word);

            if (lineWidth + size.X < maxLineWidth)
            {
                sb.Append(word + " ");
                lineWidth += size.X + spaceWidth;
            }
            else
            {
                sb.Append("\n" + word + " ");
                lineWidth = size.X + spaceWidth;
            }
        }

        return sb.ToString();
    }

    //From opsu!

    /**
     * Clamps a value between a lower and upper bound.
     * @param val the value to clamp
     * @param low the lower bound
     * @param high the upper bound
     * @return the clamped value
     * @author fluddokt
     */
    public static int clamp(int val, int low, int high)
    {
        if (val < low)
            return low;
        if (val > high)
            return high;
        return val;
    }

    /**
	 * Clamps a value between a lower and upper bound.
	 * @param val the value to clamp
	 * @param low the lower bound
	 * @param high the upper bound
	 * @return the clamped value
	 * @author fluddokt
	 */
    public static float clamp(float val, float low, float high)
    {
        if (val < low)
            return low;
        if (val > high)
            return high;
        return val;
    }

    /**
	 * Clamps a value between a lower and upper bound.
	 * @param val the value to clamp
	 * @param low the lower bound
	 * @param high the upper bound
	 * @return the clamped value
	 */
    public static double clamp(double val, double low, double high)
    {
        if (val < low)
            return low;
        if (val > high)
            return high;
        return val;
    }

    /**
	 * Returns the distance between two points.
	 * @param x1 the x-component of the first point
	 * @param y1 the y-component of the first point
	 * @param x2 the x-component of the second point
	 * @param y2 the y-component of the second point
	 * @return the Euclidean distance between points (x1,y1) and (x2,y2)
	 */
    public static float distance(float x1, float y1, float x2, float y2)
    {
        float v1 = x1 - x2;
        float v2 = y1 - y2;
        return (float)Math.Sqrt(v1 * v1 + v2 * v2);
    }

    public static double distance(double x1, double y1, double x2, double y2)
    {
        double dx = x1 - x2;
        double dy = y1 - y2;
        return Math.Sqrt(dx * dx + dy * dy);
    }

}