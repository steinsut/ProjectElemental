using NUnit.Framework;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private GameObject H1,H2,H3;

    [SerializeField]
    private AnimationClip HeartLose, HeartGain;

    private int heartCount = 3;

    public void addHeart()
    {
        switch (heartCount)
        {
            case 0:
                H3.GetComponent<Animator>().Play(HeartGain.name);
                break;
            case 1:
                H2.GetComponent<Animator>().Play(HeartGain.name);
                break;
            case 2:
                H1.GetComponent<Animator>().Play(HeartGain.name);
                break;
            default:
                break;
        }

        heartCount += 1;
        if (heartCount > 3)
        {
            heartCount = 3;
        }
    }

    public void removeHeart()
    {
        switch (heartCount)
        {
            case 1:
                H3.GetComponent<Animator>().Play(HeartLose.name);
                break;
            case 2:
                H2.GetComponent<Animator>().Play(HeartLose.name);
                break;
            case 3:
                H1.GetComponent<Animator>().Play(HeartLose.name);
                break;
            default:
                break;
        }

        heartCount -= 1;
        if (heartCount < 0)
        {
            heartCount = 0;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
