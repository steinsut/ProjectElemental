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
        if (Direction.magnitude < 0.5f)
        {
            StartCoroutine(Attack());
        }
        LayerMask mask = ~LayerMask.GetMask("Enemy");
        RaycastHit2D raycast = Physics2D.Raycast(transform.position + (Vector3) feetOffset, -transform.right, 0.5f, mask);
        RaycastHit2D floorRaycast = Physics2D.Raycast(transform.position + (Vector3) feetOffset, Vector2.down, 0.1f, mask);
        if(raycast.collider != null && raycast.collider.gameObject.tag != "Player" && floorRaycast.collider != null){
            rigidBody.linearVelocityY = jumpAmount;
        }
        
        Vector2 directionX = new Vector2(Direction.x, 0);
        rigidBody.linearVelocityX = directionX.normalized.x * speed;
        
    }


    IEnumerator Attack()
    {
        priority = 2;
        Debug.Log("Attack player.");
        //Swing animation
        yield return new WaitForSeconds(0.3f);
        priority = 0;
    }

    protected override void Patrol()
    {
        if(patrolTarget == Vector2.zero){
            LayerMask mask = ~LayerMask.GetMask("Enemy");
            RaycastHit2D raycast = Physics2D.Raycast(transform.position, Random.Range(0,1) < 0.5f?transform.right:-transform.right, patrolRange, mask);
            patrolTarget = raycast.point;
        }
        if((transform.position - (Vector3)patrolTarget).magnitude < 0.5f){
            LayerMask mask = ~LayerMask.GetMask("Enemy");
            RaycastHit2D raycast = Physics2D.Raycast(transform.position, transform.position - (Vector3)patrolTarget, patrolRange, mask);
            StartCoroutine(ChangePatrolDirection(raycast.point));
        }
        Vector2 directionX = new Vector2(((Vector3) patrolTarget - transform.position).x, 0);
        rigidBody.linearVelocityX = directionX.normalized.x * speed;
        return;

    }

    IEnumerator ChangePatrolDirection(Vector2 target){
        priority = 1;
        rigidBody.linearVelocityX = 0;
        yield return new WaitForSeconds(1f);
        patrolTarget = target;
        priority = 0;
        yield return null;
    }

}
