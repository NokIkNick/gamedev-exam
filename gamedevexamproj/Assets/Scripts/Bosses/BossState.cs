using UnityEngine;

public class BossState : StateMachineBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private BossBehaviour bossBehaviour;
    [SerializeField] private float shootRange = 6f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float speed = 2.5f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        bossBehaviour = animator.GetComponent<BossBehaviour>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossBehaviour.LookAtPlayer();

        float distance = Vector2.Distance(player.position, rb.position);
        if(distance >= shootRange){
            animator.SetTrigger("Shoot");
        }else if(distance <= meleeRange){
            animator.SetTrigger("Melee");
        }else if (distance > meleeRange && distance < shootRange){
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }


        

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Shoot");
        animator.ResetTrigger("Melee");
    }

}
