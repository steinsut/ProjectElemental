using System.Collections;
using UnityEngine;

public abstract class IEnemy : MonoBehaviour
{
    public int health;
    protected int priority = 0;
    public GameObject player;

    void Update()
    {
        if(priority <= 0){
            FindPlayer();
        }
        

    }

    void FindPlayer(){
        LayerMask mask = ~LayerMask.GetMask("Enemy");
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, 100f, mask);

        if(raycast.collider != null && raycast.collider.gameObject.tag == "Player"){
            Vector2 Direction = player.transform.position - transform.position;
            Debug.Log(Vector2.Dot(Direction, transform.right));
            if(Vector2.Dot(Direction, transform.right) > 0){
                    transform.Rotate(new Vector3(0,180,0));
            }
            EnemyAction(Direction);
        }
    }

    abstract protected void EnemyAction(Vector2 Direction);

    protected virtual void TakeDamage(int damage){
        
        health -= damage;
        Debug.Log("MeleeEnemy took " + damage + " damage.");
    }

    protected virtual void Die(){
        Destroy(gameObject);
    }
    
}
