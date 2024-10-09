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
    private Rigidbody2D m_rb;
    private bool isDead = false;
    private bool isInitialized = false;
    private Animator m_animator;
    private float m_rollSpeed;
    private float m_rollDuration;
    private bool canRool = true;

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

    public void Initialize(Rigidbody2D rb, Animator animator, float rollSpeed, float rollDuration){
        if(isInitialized) return;
        
        m_rb = rb;
        isInitialized = true;
        m_animator = animator;
        m_rollSpeed = rollSpeed;
        m_rollDuration = rollDuration;
    }

    public void MoveTowardsPlayer(float speed){
        Vector2 target = new Vector2(player.position.x, m_rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(m_rb.position, target, speed * Time.fixedDeltaTime);
        m_rb.MovePosition(newPos);
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


    public void RollTowardsPlayer(){
        StartCoroutine(Roll(m_rb, m_rollSpeed, m_rollDuration, m_animator));
    }

    public void StartRoll(){
        m_animator.SetBool("isRolling", true);
    }

    private IEnumerator Roll(Rigidbody2D rb, float speed, float duration, Animator animator){
        
        Debug.Log(animator.GetBool("isRolling"));
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

    public bool GetCanRoll(){
        return canRool;
    }

    public void SetCanRoll(bool canRool){
        this.canRool = canRool;
    }

    public void StartRollCooldown(){
        StartCoroutine(RollCooldown());
    }

    private IEnumerator RollCooldown(){
        yield return new WaitForSeconds(5f);
        canRool = true;
    }
}
