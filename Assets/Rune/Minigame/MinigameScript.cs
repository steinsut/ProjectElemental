using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using System.Collections;
using System.Collections.Generic;

public class MinigameScript : MonoBehaviour
{
    Image panelImage;
    private bool active = false;
    private int targetAmount;
    private int currentHit;
    private int totalTargets;
    [SerializeField]
    float targetSpawnRate = 0.1f;
    private ElementType targetElement;
    PlayerController player;
    GameObject currRune;
    public GameObject minigameTarget;
    public List<GameObject> targetPool;
    public void StartMinigame(int targetAmount, int totalAmount, ElementType target, GameObject rune, PlayerController player){
        active = true;
        this.totalTargets = totalAmount;
        if(targetPool == null){
            targetPool = new List<GameObject>();
        }
        GameObject tmp;
        while(targetPool.Count < totalTargets)
        {
            tmp = Instantiate(minigameTarget);
            tmp.SetActive(false);
            targetPool.Add(tmp);
        }
        this.targetElement = target;
        this.currRune = rune;
        this.player = player;
        this.targetAmount = targetAmount;
        currentHit = 0;
        InitializeElements();
        panelImage = GetComponentInChildren<Image>();
        panelImage.raycastTarget = true;
        panelImage.color = new Color(panelImage.color.r,panelImage.color.g,panelImage.color.b,0.95f);
        Time.timeScale = 0.2f;
        StartCoroutine(SpawnTargets( totalTargets));
    }

    IEnumerator SpawnTargets(int totalTargets){
        Rect constraints = panelImage.rectTransform.rect;
        float maxX = constraints.width / 2;
        float minX = -constraints.width / 2;
        float maxY = constraints.height / 2;
        float minY = -constraints.height / 2;
        for(int i = 0; i < totalTargets; i++){
            if(active){
                GameObject currTarget = targetPool[i];
                if(currTarget != null){
                    currTarget.transform.position = transform.position;
                    currTarget.transform.SetParent(panelImage.gameObject.transform);
                    Vector2 randomLocation = new Vector2(Random.Range(minX + 50,maxX - 50), Random.Range(minY + 50,maxY - 50));
                    currTarget.GetComponent<RectTransform>().localPosition = randomLocation;
                    currTarget.SetActive(true);
                    currTarget.GetComponent<MinigameTargetScript>().ActivateTarget(this);
                }
                yield return new WaitForSeconds(targetSpawnRate);
            }
            
        }
        if(active){
            RestoreGameState();
        }
        yield return null;
    }
    

    GameObject GetTarget(){
        for(int i = 0; i < targetPool.Count; i++)
        {
            if(!targetPool[i].activeInHierarchy){
                return targetPool[i];
            }
        }
        return null;
    }

    void InitializeElements(){
        for(int i = 0; i < targetAmount; i++){
            targetPool[i].GetComponent<MinigameTargetScript>().SetElement(targetElement);
        }
        for(int i = targetAmount; i < totalTargets; i++){
            ElementType randomElement = (ElementType)Random.Range(1, 4);
            targetPool[i].GetComponent<MinigameTargetScript>().SetElement(randomElement);
        }
        for (int i = 0; i < totalTargets; i++) {
            GameObject temp = targetPool[i];
            int randomIndex = Random.Range(i, totalTargets);
            targetPool[i] = targetPool[randomIndex];
            targetPool[randomIndex] = temp;
        }

    }

    public void TargetHit(ElementType type){
        if(type != targetElement){
            RestoreGameState();
            return;
        }
        currentHit++;
        if(currentHit >= targetAmount){
            player.GetComponent<PlayerController>().SetElement(targetElement);
            RestoreGameState();
        }
    }

    void RestoreGameState(){
        active = false;
        for(int i = 0; i < targetPool.Count; i++){
            targetPool[i].SetActive(false);
        }
        StartCoroutine(currRune.GetComponent<RuneScript>().CollectRune());
        panelImage.raycastTarget = false;
        panelImage.color = new Color(panelImage.color.r,panelImage.color.g,panelImage.color.b,0f);
        Time.timeScale = 1;
    }
}
