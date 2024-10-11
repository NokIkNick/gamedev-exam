using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int health = 30;
    private int damage = 5;
    public Transform player;
    public float chaseSpeed = 200f;
    public float jumpForce = 3f;
    public LayerMask groundlayer;
    public LayerMask playerLayer;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;
    private float timeSinceLastJump = 0f;
    [SerializeField] private Transform weapon;
    [SerializeField] private float attackRange = 0.15f;
    [SerializeField] private int damageAmount = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundlayer);
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << player.gameObject.layer);
        Debug.Log("state of grounded: "+isGrounded);
        if (isGrounded)
        {

            //Debug.Log("I am grounded");
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 3f, groundlayer);
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundlayer);
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, groundlayer);
            if (!groundInFront.collider && !gapAhead.collider)
            {
                //Debug.Log("I am in groundInfront and platformabove");
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
               // Debug.Log("hello I am in isPlayerAbove and platformAbove");
                shouldJump = true;
                
            }
            if (!isPlayerAbove && platformAbove.collider == false)
            {
                //Debug.Log("player isn't above me");
                shouldJump = false;
            }
            if(player.transform.position.y < transform.position.y)
            {
                shouldJump = false;
               // Debug.Log("Hello I shouldn't jump");
            }

        }
        Vector2 weaponPosition = weapon.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(weaponPosition, attackRange, playerLayer);
        foreach (Collider2D hit in hits)
        {
            Health healthScript = hit.GetComponent<Health>();
            if (healthScript != null)
            {
                healthScript.TakeDamage(damageAmount);
            }
            String hitName = hit.name;
            Vector2 knockbackDirection = (transform.position - hit.transform.position.normalized);
            rb.AddForce(knockbackDirection * -55f);
            Debug.Log("Hit: " + hitName);
        }

    }
    private void MakeEnemyJump() 
    {
        if (isGrounded && shouldJump && timeSinceLastJump <=0)
        {
            
            //Debug.Log("I am jumping and is in isGrounded and shouldJump");
            shouldJump = false;

            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 jumpDirection = direction * jumpForce;
            rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
            timeSinceLastJump = timeSinceLastJump - 1 * Time.deltaTime;
           // Debug.Log("Hello I have jumped: "+timeSinceLastJump);
            
        }
    }
    private void FixedUpdate()
    {
        MakeEnemyJump();
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(weapon.position, attackRange);
    }
}