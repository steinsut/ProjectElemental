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
    private bool airborne = false;

    /*protected override void Update()
    {
        base.Update();
        
    }*/

    protected override void EnemyAction(Vector2 Direction){
        if (Direction.magnitude < 0.5f)
        {
            StartCoroutine(Attack());
        }
        LayerMask mask = ~LayerMask.GetMask("Enemy");
        RaycastHit2D raycast = Physics2D.Raycast(transform.position + (Vector3) feetOffset, (player.transform.position - transform.position).normalized, 0.5f, mask);

        if(!airborne){
            StartCoroutine(Jump(raycast));}
        
        Vector2 directionX = new Vector2(Direction.x, 0);
        rigidBody.linearVelocityX = directionX.normalized.x * speed;
        
    }

    IEnumerator Jump(RaycastHit2D raycast){
        //airborne = true;
        if(raycast.collider != null && raycast.collider.gameObject.tag != "Player"){
            Debug.Log("jump");
            rigidBody.linearVelocityY = jumpAmount;
        }
        yield return null;
    }

    IEnumerator Attack()
    {
        priority = 1;
        Debug.Log("Attack player.");
        //Swing animation
        yield return new WaitForSeconds(0.3f);
        priority = 0;
    }

}
