using System.Collections;
using UnityEngine;
using DG.Tweening;
using Image = UnityEngine.UI.Image;

public class MinigameTargetScript : MonoBehaviour
{
    ElementType type;
    float minScale = 0.4f;
    bool active = false;
    MinigameScript manager;
    public float totalDuration;
    public void ActivateTarget(MinigameScript manager){
        this.manager = manager;
        active = true;
        transform.DOScale(minScale,0);
        transform.DOScale(1, totalDuration).OnComplete(() => {
            transform.DOScale(minScale,totalDuration).OnComplete(() => {
                gameObject.SetActive(false);
            });

        });
    }

    public void SetElement(ElementType type){
        Color targetColor = Color.clear;
        switch (type){
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
        GetComponent<Image>().color = targetColor;
        this.type = type;
    }

    public void OnClick(){
        if(active){
            StopAllCoroutines();
            manager.TargetHit(type);
            gameObject.SetActive(false);
        }
    }
}
