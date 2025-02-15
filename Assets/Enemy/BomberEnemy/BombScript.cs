using System.Collections;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public Vector2 initialForce;
    public float explodeRadius;
    bool triggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GetComponent<Rigidbody2D>().AddForce(initialForce * 3f, ForceMode2D.Impulse);
        triggered = false;
    }

    IEnumerator Explode(){
        triggered = true;
        GameObject player = GameObject.FindWithTag("Player");
        yield return new WaitForSeconds(0.3f);
        LayerMask mask = ~LayerMask.GetMask("Enemy", "Projectile");
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, explodeRadius, mask);
        if(raycast.collider != null && raycast.collider.gameObject.CompareTag("Player")){
            //Debug.Log("player damaged");
        }

        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(){
        if(!triggered)
            StartCoroutine(Explode());
    }
}
