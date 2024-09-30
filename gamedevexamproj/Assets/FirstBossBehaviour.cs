using UnityEngine;

public class FirstBossBehaviour : MonoBehaviour
{
    private Transform player;
    private bool isFlipped = false;
    [SerializeField] private Vector2 attackOffset = new Vector2(1f, 0.5f);
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask attackMask;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        
    }

    public void LookAtPlayer(){
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if( transform.position.x > player.position.x && isFlipped){
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }else if(transform.position.x < player.position.x && !isFlipped){
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public bool getIsFlipped(){
        return isFlipped;
    }


    public void Attack(){
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if(colInfo != null){
            colInfo.GetComponent<Health>()?.TakeDamage(attackDamage);
        }
    }
}
