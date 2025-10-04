using System.Reflection;
using System.Text.RegularExpressions;

namespace cs2hvh_utilities.Extensions;
class Colors
{
    public static char Default = '\x01';
    public static char White = '\x01';
    public static char Darkred = '\x02';
    public static char Green = '\x04';
    public static char Lightyellow = '\x09';
    public static char Lightblue = '\x0B';
    public static char Olive = '\x05';
    public static char Lime = '\x06';
    public static char Red = '\x07';
    public static char Lightpurple = '\x03';
    public static char Purple = '\x0E';
    public static char Grey = '\x08';
    public static char Yellow = '\x09';
    public static char Gold = '\x10';
    public static char Silver = '\x0A';
    public static char Blue = '\x0B';
    public static char Darkblue = '\x0C';
    public static char Bluegrey = '\x0A';
    public static char Magenta = '\x0E';
    public static char Lightred = '\x0F';
    public static char Orange = '\x10';

    public static readonly Dictionary<string, char> ColorMap = new Dictionary<string, char>(StringComparer.OrdinalIgnoreCase)
    {
        { "Default", Colors.Default },
        { "White", Colors.White },
        { "Darkred", Colors.Darkred },
        { "Green", Colors.Green },
        { "Lightyellow", Colors.Lightyellow },
        { "Lightblue", Colors.Lightblue },
        { "Olive", Colors.Olive },
        { "Lime", Colors.Lime },
        { "Red", Colors.Red },
        { "Lightpurple", Colors.Lightpurple },
        { "Purple", Colors.Purple },
        { "Grey", Colors.Grey },
        { "Yellow", Colors.Yellow },
        { "Gold", Colors.Gold },
        { "Silver", Colors.Silver },
        { "Blue", Colors.Blue },
        { "Darkblue", Colors.Darkblue },
        { "Bluegrey", Colors.Bluegrey },
        { "Magenta", Colors.Magenta },
        { "Lightred", Colors.Lightred },
        { "Orange", Colors.Orange }
    };

    public static string FormatMessage(string message)
    {
        var colorFields = typeof(Colors).GetFields(BindingFlags.Public | BindingFlags.Static)
             .Where(f => f.FieldType == typeof(char))
             .ToArray();

        message = Regex.Replace(message, @"{(\w+)}", match =>
        {
            string tagName = match.Groups[1].Value;

            var colorField = colorFields.FirstOrDefault(f =>
                f.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            if (colorField != null)
            {
                return colorField.GetValue(null).ToString();
            }

            return match.Value;
        });

        return message;
    }

    public static string RemoveFromMessage(string message)
    {
        return Regex.Replace(message, @"{\w+}", "");
    }
}
