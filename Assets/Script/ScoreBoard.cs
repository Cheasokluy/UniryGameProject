using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;



public class ScoreBoard:MonoBehaviour
{
    public TextMeshProUGUI[] scoresText_10Pairs;
    public TextMeshProUGUI[] dateText_10Pairs;

    public TextMeshProUGUI[] scoresText_15Pairs;
    public TextMeshProUGUI[] dateText_15Pairs;

    public TextMeshProUGUI[] scoresText_20Pairs;
    public TextMeshProUGUI[] dateText_20Pairs;

    private void Start()
    {
        UpdateScoreBoard();
        
    }

    public void UpdateScoreBoard()
    {
        Config.UpdateScoreList();

        DisplayScoreList(Config.ScoreTimeList10Pairs, Config.PairNumberList10Pairs, scoresText_10Pairs, dateText_10Pairs);
        DisplayScoreList(Config.ScoreTimeList15Pairs, Config.PairNumberList15Pairs, scoresText_15Pairs, dateText_15Pairs);
        DisplayScoreList(Config.ScoreTimeList20Pairs, Config.PairNumberList20Pairs, scoresText_20Pairs, dateText_20Pairs);

    }

    private void DisplayScoreList(float[] scoreTimeList, string[] pairNumberList, TextMeshProUGUI[] scoreText , TextMeshProUGUI[] dateText)
    {
        for(var index = 0; index < 3; index++)
        {
            if(scoreTimeList[index] > 0)
            {
                var dateTime = Regex.Split(pairNumberList[index], "T");
                var minutes = Mathf.Floor(scoreTimeList[index] / 60);
                float secounds = Mathf.RoundToInt(scoreTimeList[index] % 60);

                scoreText[index].text = minutes.ToString("00") + ":" + secounds.ToString("00");
                dateText[index].text = dateTime[0] + " " + dateTime[1];

            }
            else
            {
                scoreText[index].text = " ";
                dateText[index].text = " ";
            }
        }
    }

}
