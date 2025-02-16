using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MeleeEnemy : IEnemy
{
    public float speed;
    public Vector2 feetOffset;
    public float jumpAmount;

    public float patrolRange;

    bool airborne = false;
    Vector2 patrolTarget;
    int currentAttack = 0;
    float attackResetTimer = 0.5f;
    private float currentAttackTime = 0f;
    static int IdleAnim = Animator.StringToHash("MeleeIdle");
    static int WalkAnim = Animator.StringToHash("MeleeWalk");
    static int RunAnim = Animator.StringToHash("MeleeRun");
    static int JumpAnim = Animator.StringToHash("MeleeJump");
    static int HurtAnim = Animator.StringToHash("MeleeHurt");
    static int Attack1Anim = Animator.StringToHash("MeleeAttack");
    static int Attack2Anim = Animator.StringToHash("MeleeAttack2");
    static int Attack3Anim = Animator.StringToHash("MeleeAttack3");
    static int DeathAnim = Animator.StringToHash("MeleeDeath");



    protected override int GetDeathAnim(){return DeathAnim;}
    protected override int GetHurtAnim(){return HurtAnim;}
    protected override void EnemyAction(Vector2 Direction){
        if(!IncreasePriority(3))return;
        patrolTarget = Vector2.zero;
        if (Direction.magnitude < 1f)
        {
            StartCoroutine(Attack());
        }
        else{
            
            Vector2 directionX = new Vector2(Direction.x, 0);
            Move(directionX.normalized, true);
        }
        
    }
    protected override void Update()
    {
        base.Update();
        if(currentAttack != 0){
            currentAttackTime += Time.deltaTime;
            if(currentAttackTime >= attackResetTimer){
                currentAttack = 0;
                currentAttackTime = 0f;
            }
        }
    }

    IEnumerator Attack()
    {
        if(IncreasePriority(4)){
            rigidBody.linearVelocity = Vector2.zero;
            currentAttackTime = 0f;
            switch (currentAttack){
                case 0:
                    animator.CrossFade(Attack1Anim,0);
                    break;
                case 1:
                    animator.CrossFade(Attack2Anim,0);
                    break;
                case 2:
                    animator.CrossFade(Attack3Anim,0);
                    break;

            }
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            currentAttack = (currentAttack + 1) % 3;
            currentAttackTime = 0f;
            if(priority == 4)
                priority = 0;
        }
        
    }

    protected override void Patrol()
    {
        if(patrolTarget == Vector2.zero){
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
        if((transform.position - (Vector3)patrolTarget).magnitude < 1f){
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
        Move(directionX.normalized, false);
        return;

    }

    void Move(Vector2 directionX, bool towardsPlayer){
        if(Vector2.Dot(directionX, transform.right) > 0){
                transform.Rotate(new Vector3(0,180,0));
        }
        rigidBody.linearVelocityX = towardsPlayer? directionX.x * speed * 2 : directionX.x * speed;
        if(airborne){return;}

        if(towardsPlayer && animator.GetCurrentAnimatorStateInfo(0).shortNameHash != RunAnim){
            animator.CrossFade(RunAnim,0);
        }else if(!towardsPlayer && animator.GetCurrentAnimatorStateInfo(0).shortNameHash != WalkAnim){
            animator.CrossFade(WalkAnim,0);
        }
        
    }

    IEnumerator ChangePatrolDirection(Vector2 target){
        if(IncreasePriority(1)){
            rigidBody.linearVelocityX = 0;
            animator.CrossFade(IdleAnim,0);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            patrolTarget = target;
            if(priority == 1)
                priority = 0;
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        StartCoroutine(JumpCoroutine());
        
    }

    IEnumerator JumpCoroutine(){
        if(priority == 3 && !airborne){
            airborne = true;
            rigidBody.AddForce(Vector2.up * jumpAmount);
            animator.CrossFade(JumpAnim,0);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            airborne = false;
        }
    }

}
