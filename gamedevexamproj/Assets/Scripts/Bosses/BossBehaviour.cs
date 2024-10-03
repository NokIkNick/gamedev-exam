using UnityEngine;

public class BossBehaviour : MonoBehaviour, IBossBehaviour
{
    private Transform player;
    private bool isFlipped = false;
    [SerializeField] private Vector2 attackOffset = new Vector2(2f, 0.5f);
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float knockbackForce = 5f;
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

    public bool GetIsFlipped(){
        return isFlipped;
    }


    public void Attack(){
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if(colInfo != null){
            Debug.Log("Hit: " + colInfo.name);
            colInfo.GetComponent<Health>()?.TakeDamage(attackDamage);
            Vector2 hitDirection = colInfo.transform.position - transform.position;
            colInfo.GetComponent<Rigidbody2D>()?.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}
