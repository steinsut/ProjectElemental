using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MeleeEnemy : IEnemy
{
    public float speed;
    public Rigidbody2D rigidBody;
    public Vector2 feetOffset;
    public float jumpAmount;

    public float patrolRange;
    Vector2 patrolTarget;

    protected override void EnemyAction(Vector2 Direction){
        priority = 2;
        patrolTarget = Vector2.zero;
        if (Direction.magnitude < 0.5f)
        {
            StartCoroutine(Attack());
        }
        else{
            
            Vector2 directionX = new Vector2(Direction.x, 0);
            Move(directionX.normalized);
        }
        
    }


    IEnumerator Attack()
    {
        priority = 3;
        rigidBody.linearVelocity = Vector2.zero;
        Debug.Log("Attack player.");
        //Swing animation
        yield return new WaitForSeconds(0.3f);
        priority = 0;
    }

    protected override void Patrol()
    {
        if(patrolTarget == Vector2.zero){
            LayerMask mask = ~LayerMask.GetMask("Enemy");
            Vector2 direction = Random.Range(0,1) < 0.5f?transform.right:-transform.right;
            RaycastHit2D raycast = Physics2D.Raycast(transform.position + (Vector3) feetOffset, direction, patrolRange, mask);
            Vector2 target;
            if(raycast.collider == null){
                target = transform.position + (Vector3) feetOffset + (Vector3)direction * patrolRange;
            }else{
                target = raycast.point;
            }
            StartCoroutine(ChangePatrolDirection(target));
        }
        if((transform.position - (Vector3)patrolTarget).magnitude < 0.5f){
            LayerMask mask = ~LayerMask.GetMask("Enemy");
            Vector2 direction = new Vector2((transform.position - (Vector3)patrolTarget).x, 0).normalized; 
            RaycastHit2D raycast = Physics2D.Raycast(transform.position + (Vector3) feetOffset, 
                direction, patrolRange, mask);
            Vector2 target;
            if(raycast.collider == null){
                target = transform.position + (Vector3)direction * patrolRange;
            }else{
                target = raycast.point;
            }
            StartCoroutine(ChangePatrolDirection(target));
        }
        Vector2 directionX = new Vector2(((Vector3) patrolTarget - (transform.position+ (Vector3) feetOffset)).x, 0);
        Move(directionX.normalized);
        return;

    }

    void Move(Vector2 directionX){
        if(Vector2.Dot(directionX, transform.right) > 0){
                transform.Rotate(new Vector3(0,180,0));
        }
        rigidBody.linearVelocityX = directionX.x * speed;
        
    }

    IEnumerator ChangePatrolDirection(Vector2 target){
        priority = 1;
        rigidBody.linearVelocityX = 0;
        yield return new WaitForSeconds(1f);
        patrolTarget = target;
        priority = 0;
        yield return null;
    }

    void OnTriggerEnter2D(Collider2D col){
        if(priority >= 2){
            rigidBody.AddForce(Vector2.up * jumpAmount);
        }
    }

}
