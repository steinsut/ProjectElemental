using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> hearts;

    [SerializeField]
    private AnimationClip HeartLose, HeartGain;

    [SerializeField]
    private AudioClip HeartLoseAudio, HeartGainAudio;

    private AudioSource AudioPlayer;

    private int heartCount = 5;

    private int aimedHeart = 3;

    public void incrementHeart()
    {
        aimedHeart += 1;
    }

    public void damage()
    {
        aimedHeart -= 1;
    }

    public void setHeartCount(int hc)
    {
        aimedHeart = hc;
        if (aimedHeart < 0)
        {
            aimedHeart = 0;
        }
    }

    public void addHeart()
    {
        hearts[heartCount++].GetComponent<Animator>().Play(HeartGain.name);
        AudioPlayer.PlayOneShot(HeartGainAudio);
    }

    public void removeHeart()
    {
        hearts[--heartCount].GetComponent<Animator>().Play(HeartLose.name);
        AudioPlayer.PlayOneShot(HeartLoseAudio);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioPlayer = this.GetComponent<AudioSource>();
    }

    private float deltaSum = 0;
    private float timeBetweenUpdates = 0.2f;

    // Update is called once per frame
    void Update()
    {
        if (heartCount > aimedHeart)
        {
            deltaSum += Time.deltaTime;
            if (deltaSum > timeBetweenUpdates)
            {
                removeHeart();
                deltaSum = 0;
            }
        } else if (heartCount < aimedHeart)
        {
            deltaSum += Time.deltaTime;
            if (deltaSum > timeBetweenUpdates / 2)
            {
                addHeart();
                deltaSum = 0;
            }
        }
    }
}
