using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class RangedEnemy : IEnemy
{
    
    protected override void EnemyAction(Vector2 Direction){
        StartCoroutine(Shoot(Direction));
    }
    
    protected override int GetDeathAnim(){return 0;}
    protected override int GetHurtAnim(){return 0;}
    IEnumerator Shoot(Vector2 Direction)
    {
        if(IncreasePriority(4)){
            GameObject projectile = ProjectilePooling.SingletonInstance.GetProjectile();
            if(projectile != null){
                projectile.transform.position = transform.position;
                projectile.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, (player.transform.position - transform.position).normalized));
                projectile.SetActive(true);            
            }
            
            projectile.GetComponent<ProjectileScript>().SetDirection((player.transform.position - transform.position).normalized);
            yield return new WaitForSeconds(1f);
            priority = 0;
        }
        
        
    }

}
