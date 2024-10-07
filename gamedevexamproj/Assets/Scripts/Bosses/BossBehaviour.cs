using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private bool isEnraged = false;
    [SerializeField] private List<GameObject> projectiles;
    [SerializeField] private float projectileOffsetX = 1f;
    [SerializeField] private float projectileOffsetY = 1f;
    [SerializeField] private Color flashColor = new Color(0.678f, 0.847f, 0.902f);
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if(isDead){
            if(GetDistanceToPlayer() > 15){
                Destroy(gameObject);
            }
        }

    }

    public void MoveTowardsPlayer(Rigidbody2D rb, float speed){
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
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

    public void Enrage(){
        if(isEnraged) return;

        SetIsEnraged(true);
        attackDamage *=  2;
        attackRange = 2;
        knockbackForce *= 2;
    }

    public void SpawnProjectile(){
        foreach(GameObject projectile in projectiles){
            if(!projectile.activeInHierarchy){
                Vector2 direction = (player.position - transform.position).normalized;
                Vector2 pos = new Vector2(transform.position.x + projectileOffsetX, transform.position.y + projectileOffsetY);
                projectile.GetComponent<BossRocketScript>().Initialize(direction, pos, player.position);
                break;
            }
        }
    }


    public void RollTowardsPlayer(Rigidbody2D rb, float speed, float duration, Animator animator){
        StartCoroutine(Roll(rb, speed, duration, animator));
    }

    private IEnumerator Roll(Rigidbody2D rb, float speed, float duration, Animator animator){
        float elapsedTime = 0f;
        Vector2 direction = (player.position - transform.position).normalized;
        float offsetDistance = 4f;
        Vector2 target = new Vector2(player.position.x, rb.position.y) + direction * offsetDistance;
        while(elapsedTime < duration){
            
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            
        }
        animator.SetBool("isRolling", false);
    }

    public float GetDistanceToPlayer(){
        return Vector2.Distance(transform.position, player.position);
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

    public bool GetIsFlipped(){
        return isFlipped;
    }

    public bool GetIsEngraged(){
        return isEnraged;
    }

    public void SetIsEnraged(bool enraged){
        isEnraged = enraged;
    }

    public void EndBattle(){
        Debug.Log("Boss is dead!");
        Debug.Log("Dropping loot...");
        isDead = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;
        Gizmos.DrawWireSphere(pos, attackRange);
    }

    public void Flash(){
        GetComponent<SpriteRenderer>().color = flashColor;
        Invoke("ResetColor", 0.2f);
    }

    public void ResetColor(){
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
