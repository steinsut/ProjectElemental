using System.Collections;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public abstract class IEnemy : MonoBehaviour
{
    public int health;
    protected int priority = 0;
    public GameObject player;
    public Vector2 headOffset;
    [SerializeField]
    protected Rigidbody2D rigidBody;
    protected LayerMask mask;


    void Awake(){
        mask = ~LayerMask.GetMask("Enemy", "Projectile" , "Rune");
    }
    protected virtual void Update()
    {
        if(priority < 4){
            if(FindPlayer()){
                return;
            }
            if(priority == 3){//Lost track of the player
                priority = 0;
            }
            if(priority <= 0){//If not changing directions, and can't find the player, patrol
                Patrol();
            }
        }
        

    }

    bool FindPlayer(){
        LayerMask mask = ~LayerMask.GetMask("Enemy");
        RaycastHit2D raycast = Physics2D.Raycast(transform.position + (Vector3) headOffset, (player.transform.position - transform.position).normalized, 100f, mask);

        if(raycast.collider != null && raycast.collider.gameObject.CompareTag("Player")){
            Vector2 Direction = player.transform.position - transform.position;
            if(Vector2.Dot(Direction, transform.right) > 0){
                    transform.Rotate(new Vector3(0,180,0));
            }
            EnemyAction(Direction);
            return true;
        }
        return false;
    }

    public virtual IEnumerator TakeDamage(int damage){
        IncreasePriority(5);
        yield return new WaitForSeconds(0.5f);
        health -= damage;
        if(health <= 0){
            yield return Die();
        }else{
                
            priority = 0;
            yield return null;
        }
    }

    public virtual IEnumerator Stun(float seconds){
        IncreasePriority(6);
        yield return new WaitForSeconds(seconds);
        priority = 0;
    }

    virtual protected void Patrol(){}

    abstract protected void EnemyAction(Vector2 Direction);

    protected virtual IEnumerator Die(){
        IncreasePriority(7);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    public void Push(Vector2 force){
        IncreasePriority(2);
        rigidBody.AddForce(force, ForceMode2D.Force);
    }
    public void StopPush(){
        priority = 0;
    }

    protected void IncreasePriority(int priority){
        this.priority = Mathf.Max(this.priority,priority);
    }
    
}
