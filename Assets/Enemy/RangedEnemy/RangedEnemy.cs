using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class RangedEnemy : IEnemy
{
    public GameObject projectilePrefab;
    public GameObject weaponPrefab;
    
    protected override void EnemyAction(Vector2 Direction){
        StartCoroutine(Shoot(Direction));
    }

    IEnumerator Shoot(Vector2 Direction)
    {
        LayerMask mask = ~LayerMask.GetMask("Enemy");
        RaycastHit2D raycast = Physics2D.Raycast(weaponPrefab.transform.position, (player.transform.position - weaponPrefab.transform.position).normalized, 100f, mask);

        if(raycast.collider != null && raycast.collider.gameObject.tag == "Player"){
            priority = 1;
            GameObject projectile = ProjectilePooling.SingletonInstance.GetProjectile();
            if(projectile != null){
                projectile.transform.position = weaponPrefab.transform.position;
                projectile.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, (player.transform.position - weaponPrefab.transform.position).normalized));
                projectile.SetActive(true);            
            }
            
            projectile.GetComponent<ProjectileScript>().SetDirection((player.transform.position - transform.position).normalized);
            yield return new WaitForSeconds(1f);
            priority = 0;
        }
    }

}
