using System.Collections;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BomberEnemy : IEnemy
{
    public float speed;

    public float patrolRange;
    Vector2 patrolTarget;

    protected override void EnemyAction(Vector2 Direction){
        priority = 2;
        patrolTarget = Vector2.zero;
        if (Mathf.Abs(Direction.x) < 0.3f)
        {
            StartCoroutine(Shoot());
        }
        else{
            
            Vector2 directionX = new Vector2(Direction.x, 0);
            Move(directionX.normalized);
        }
        
    }


    IEnumerator Shoot()
    {
        priority = 3;
        rigidBody.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        for (int i = -2; i <= 2; i++)
        {
            GameObject bomb = ProjectilePooling.SingletonInstance.GetBomb();
            if (bomb != null)
            {
                bomb.transform.position = transform.position;
                float angle = 30f * i;
                Vector2 forceDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;
                bomb.GetComponent<BombScript>().initialForce = forceDirection;
                bomb.transform.rotation = Quaternion.identity;
                bomb.SetActive(true);
            }
        }
        yield return new WaitForSeconds(1f);
        priority = 0;
        
    }

    protected override void Patrol()
    {
        if(patrolTarget == Vector2.zero){
            LayerMask mask = ~LayerMask.GetMask("Enemy");
            Vector2 direction = Random.Range(0,1) < 0.5f?transform.right:-transform.right;
            RaycastHit2D raycast = Physics2D.Raycast(transform.position, direction, patrolRange, mask);
            Vector2 target;
            if(raycast.collider == null){
                target = transform.position + (Vector3)direction * patrolRange;
            }else{
                target = raycast.point;
            }
            StartCoroutine(ChangePatrolDirection(target));
        }
        if((transform.position - (Vector3)patrolTarget).magnitude < 0.5f){
            LayerMask mask = ~LayerMask.GetMask("Enemy");
            Vector2 direction = new Vector2((transform.position - (Vector3)patrolTarget).x, 0).normalized; 
            RaycastHit2D raycast = Physics2D.Raycast(transform.position, 
                direction, patrolRange, mask);
            Vector2 target;
            if(raycast.collider == null){
                target = transform.position + (Vector3)direction * patrolRange;
            }else{
                target = raycast.point;
            }
            StartCoroutine(ChangePatrolDirection(target));
        }
        Vector2 directionX = new Vector2(((Vector3) patrolTarget - (transform.position)).x, 0);
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

}
