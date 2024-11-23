using UnityEngine;

public static class StringUtils
{
    public static string GenerateColorString(Color color, string content)
    { 
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{content}</color>";
    }
}
