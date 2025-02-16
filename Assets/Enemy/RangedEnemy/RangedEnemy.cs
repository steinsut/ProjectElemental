using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class RangedEnemy : IEnemy
{
    
    static int IdleAnim = Animator.StringToHash("RangedIdle");
    static int HurtAnim = Animator.StringToHash("RangedHurt");
    static int DeathAnim = Animator.StringToHash("RangedDeath");
    static int AttackAnim = Animator.StringToHash("RangedAttack");
    protected override int GetDeathAnim(){return DeathAnim;}
    protected override int GetHurtAnim(){return HurtAnim;}
    protected override void EnemyAction(Vector2 Direction){
        StartCoroutine(Shoot(Direction));
    }
    [SerializeField]
    Vector2 projectileOffset;
    IEnumerator Shoot(Vector2 Direction)
    {
        if(IncreasePriority(4)){
            animator.CrossFade(AttackAnim,0);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            GameObject projectile = ProjectilePooling.SingletonInstance.GetProjectile();
            if(projectile != null){
                projectile.transform.position = transform.position + (Vector3) projectileOffset;
                projectile.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, (player.transform.position - transform.position).normalized));
                projectile.SetActive(true);            
            }
            
            projectile.GetComponent<ProjectileScript>().SetDirection((player.transform.position - (transform.position + (Vector3) projectileOffset)).normalized);
            yield return new WaitForSeconds(1f);
            priority = 0;
        }
        
        
    }

}
