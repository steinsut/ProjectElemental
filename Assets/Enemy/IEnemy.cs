using System.Collections;
using UnityEngine;

public abstract class IEnemy : MonoBehaviour
{
    public int health;
    protected int priority = 0;
    public GameObject player;
    public Vector2 headOffset;

    protected virtual void Update()
    {
        if(priority <= 0){
            FindPlayer();
        }
        

    }

    void FindPlayer(){
        LayerMask mask = ~LayerMask.GetMask("Enemy");
        RaycastHit2D raycast = Physics2D.Raycast(transform.position + (Vector3) headOffset, (player.transform.position - transform.position).normalized, 100f, mask);

        if(raycast.collider != null && raycast.collider.gameObject.tag == "Player"){
            Vector2 Direction = player.transform.position - transform.position;
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
