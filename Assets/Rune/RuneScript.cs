using System.Collections;
using UnityEngine;

public class RuneScript : MonoBehaviour
{
    [SerializeField]
    public ElementType runeType;
    public Color targetColor;
    public GameObject minigamePanel;
    public GameObject animations;

    GameObject player;
    public int targetAmount;
    public int totalAmount;
    private bool active = false;
    public Animator animator;
    static int RunePickup = Animator.StringToHash("Pickup");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {    
        switch (runeType){
            case ElementType.DIRT:
                targetColor = new Color(0.59f, 0.29f, 0.0f); // brown
                animations.GetComponent<Animator>().SetTrigger("DirtAnimations");
                break;
            case ElementType.FIRE:
                targetColor = Color.red;
                animations.GetComponent<Animator>().SetTrigger("FireAnimations");
                break;
            case ElementType.WATER:
                targetColor = Color.blue;
                animations.GetComponent<Animator>().SetTrigger("WaterAnimations");
                break;
            case ElementType.AIR:
                targetColor = Color.gray;
                animations.GetComponent<Animator>().SetTrigger("AirAnimations");
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

    public IEnumerator CollectRune(){
        animator.CrossFade(RunePickup, 0,0);
        animations.SetActive(false);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);

    }
}
