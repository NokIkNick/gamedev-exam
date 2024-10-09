using Unity.VisualScripting;
using UnityEngine;

public class BossState : StateMachineBehaviour
{
    private Rigidbody2D rb;
    private BossBehaviour bossBehaviour;
    [SerializeField] private float shootRange = 6f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float enragedSpeed = 3.5f;
    [SerializeField] private float rollRange = 8f;
    [SerializeField] private float rollSpeed = 10f;
    [SerializeField] private float rollDuration = 4f;
    [SerializeField] private float rollCooldown = 8f;
    private bool isEnraged;
    


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        bossBehaviour = animator.GetComponent<BossBehaviour>();
        bossBehaviour.Initialize(rb, animator, rollSpeed, rollDuration);
        isEnraged = bossBehaviour.GetIsEngraged();

        bossBehaviour.Flash();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        float distance = bossBehaviour.GetDistanceToPlayer();
        
        bossBehaviour.LookAtPlayer();
        bool canRoll = bossBehaviour.GetCanRoll();

        if(distance >= rollRange && isEnraged && !animator.GetBool("isRolling") && canRoll){
            animator.SetTrigger("Roll");
            bossBehaviour.StartRollCooldown();


        }else if(distance >= shootRange){
            animator.SetTrigger("Shoot");


        }else if(distance <= meleeRange){
            animator.SetTrigger("Melee");

        }else if (distance > meleeRange && distance < shootRange){
            if (isEnraged) {
                bossBehaviour.MoveTowardsPlayer(enragedSpeed);;

            } else {
                bossBehaviour.MoveTowardsPlayer(speed);

            };
        }

        if(animator.GetComponent<Health>().GetHealth() <= animator.GetComponent<Health>().GetMaxHealth() / 3 && !isEnraged){
            animator.SetTrigger("Enrage");
        }

        if(animator.GetComponent<Health>().GetHealth() <= 0){
            animator.SetTrigger("Die");
        }
        

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Shoot");
        animator.ResetTrigger("Melee");
        animator.ResetTrigger("Enrage");
        animator.ResetTrigger("Die");
        animator.ResetTrigger("Roll");
    }

}
