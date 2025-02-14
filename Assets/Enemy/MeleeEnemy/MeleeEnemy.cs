using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MeleeEnemy : IEnemy
{
    public int speed;

    protected override void EnemyAction(Vector2 Direction){
        if (Direction.magnitude < 0.5f)
        {
            StartCoroutine(Attack());
        }
        transform.position += (Vector3)Direction.normalized * speed * Time.deltaTime;
        
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
