using UnityEngine;

public class AwakeBoss : StateMachineBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private GameObject healthbar;
    [SerializeField] private float awakeRange = 4f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        healthbar = animator.transform.parent.GetChild(1).gameObject;
        Debug.Log(healthbar.name);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float distance = Vector2.Distance(player.position, rb.position);

        bool isAwake = animator.GetBool("isAwake");

        if(!isAwake){
            if(distance <= awakeRange){
                healthbar.SetActive(true);
                animator.SetBool("isAwake", true);
                
            }
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

}
