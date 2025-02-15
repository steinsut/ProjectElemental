using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MinigameTargetScript : MonoBehaviour
{
    public ElementType type;
    float minScale = 0.4f;
    bool active = false;
    MinigameScript manager;
    public void ActivateTarget(MinigameScript manager){
        this.manager = manager;
        active = true;
        transform.DOScale(minScale,0);
        transform.DOScale(1, 0.4f).OnComplete(() => {
            transform.DOScale(minScale,0.4f).OnComplete(() => {
                gameObject.SetActive(false);
            });

        });
    }

    public void OnClick(){
        if(active){
            StopAllCoroutines();
            manager.TargetHit();
            gameObject.SetActive(false);
        }
    }
}
