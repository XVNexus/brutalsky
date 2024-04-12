using System.Text.RegularExpressions;
using UnityEngine;
using Utils;

public class Isolated : MonoBehaviour
{
    private void Start()
    {
        var part0 =
            "!#BRUTALSKY;#XVEON:40,20;FFFFFFCC\n" +
            "$-15,1:\n" +
            "$15,1:\n" +
            "#S;SPINNER-LEFT-BG:-2,,;4,5;2,,,10,,;4C4C4C;;\n" +
            "#S;SPINNER-RIGHT-BG:2,,;4,5;2,,,10,,;4C4C4C;;\n" +
            "#S;WALL-LEFT:-20,,;3,2,22;2,,,10,,;B2B2B2;1;1\n" +
            "#S;WALL-RIGHT:20,,;3,2,22;2,,,10,,;B2B2B2;1;1";
        Debug.Log(part0);
        var part1 = Regex.Replace(part0, ",{1,13}", match => $"{(char)(match.Length + 96)}");
        var part2 = Regex.Replace(part1, ";{1,13}", match => $"{(char)(match.Length + 109)}");
        Debug.Log(part2);
        var part3 = Regex.Replace(part2, "[a-m]",
            match => BsUtils.RepeatChar(',', match.Value[0] - 96));
        var part4 = Regex.Replace(part3, "[n-z]",
            match => BsUtils.RepeatChar(';', match.Value[0] - 109));
        Debug.Log(part4);
    }
}
