using System.Collections;
using UnityEngine;

public class RuneScript : MonoBehaviour
{
    [SerializeField]
    public ElementType runeType;
    public Color targetColor;
    public GameObject minigamePanel;
    GameObject player;
    public int targetAmount;
    public int totalAmount;
    private bool active = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {    
        switch (runeType){
            case ElementType.DIRT:
                targetColor = new Color(0.59f, 0.29f, 0.0f); // brown
                break;
            case ElementType.FIRE:
                targetColor = Color.red;
                break;
            case ElementType.WATER:
                targetColor = Color.blue;
                break;
            case ElementType.AIR:
                targetColor = Color.gray;
                break;
            case ElementType.WOOD:
                break;
        }
        GetComponent<SpriteRenderer>().color = targetColor;
        player = GameObject.FindGameObjectWithTag("Player");
        minigamePanel = GameObject.FindGameObjectWithTag("Minigame");
    }

    // Update is called once per frame
    void Update()
    {
        if(active && Input.GetKeyDown(KeyCode.F)){
            minigamePanel.GetComponent<MinigameScript>().StartMinigame(targetAmount, totalAmount,runeType, gameObject, player.GetComponent<PlayerController>());
            
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.CompareTag("Player")){
            active = true;
        }

    }

    void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.CompareTag("Player")){
            active = false;
        }
    }
}
