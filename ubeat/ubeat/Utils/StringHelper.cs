using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System.Text.RegularExpressions;

public static class StringHelper
{
    public static string SanitizeUnicode(string input)
    {
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
}