using TMPro;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{

    private int totalScore = 0;

    [SerializeField]
    private TextMeshProUGUI scoreboard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (scoreboard == null)
        {
            Debug.LogError("Scoreboard not set!");
        }
    }

    private int frameCount = 0;
    private int framePerCount = 20;

    public void addScore(int delta)
    {
        totalScore += delta;
    }

    public void reduceScore(int delta)
    {
        totalScore -= delta;
    }

    // Update is called once per frame
    void Update()
    {
        if (frameCount++ % framePerCount == 0)
        {
            scoreboard.text = totalScore.ToString();
            //scoreboard.SetText(totalScore.ToString());
        }
    }
}
