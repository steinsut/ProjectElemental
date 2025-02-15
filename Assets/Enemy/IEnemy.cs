using System.Collections;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public abstract class IEnemy : MonoBehaviour
{
    public int health;

    [SerializeField]
    protected int priority = 0;
    public GameObject player;
    public Vector2 headOffset;
    [SerializeField]
    protected Rigidbody2D rigidBody;
    protected LayerMask mask;
    public Animator animator;

    protected abstract int GetDeathAnim();
    protected abstract int GetHurtAnim();

    void Awake(){
        mask = ~LayerMask.GetMask("Enemy", "EnemyProjectile" , "Rune", "Player", "Windblower");
    }
    protected virtual void Update()
    {
        if(priority < 4){
            if(FindPlayer()){
                Debug.Log("player found");
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
        LayerMask mask = ~LayerMask.GetMask("Enemy", "EnemyProjectile" , "Rune", "Windblower");
        RaycastHit2D raycast = Physics2D.Raycast(transform.position + (Vector3) headOffset, (player.transform.position - transform.position + (Vector3) headOffset).normalized, 100f, mask);
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
        if(IncreasePriority(5)){
            
            health -= damage;
            if(health <= 0){
                yield return Die();
            }else{
                animator.CrossFade(GetHurtAnim(),0);
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
                if(priority == 5)
                    priority = 0;
                yield return null;
            }

        }
    }

    public virtual IEnumerator Stun(float seconds){
        if(IncreasePriority(6)){    
            yield return new WaitForSeconds(seconds);
            if(priority == 6)
                priority = 0;
        }
    }

    virtual protected void Patrol(){}

    abstract protected void EnemyAction(Vector2 Direction);

    protected virtual IEnumerator Die(){
        IncreasePriority(7);
        rigidBody.linearVelocity = Vector3.zero;
        animator.CrossFade(GetDeathAnim(),0);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    public void Push(Vector2 force){
        IncreasePriority(2);
        rigidBody.AddForce(force, ForceMode2D.Force);
    }
    public void StopPush(){
        if(priority == 2)
            priority = 0;
    }

    protected bool IncreasePriority(int priority){
        if(this.priority > priority){return false;}
        this.priority = priority;
        return true;
    }
    
}
