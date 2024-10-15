using System;
using System.Collections;
using UnityEngine;

public class Patrol_Script : MonoBehaviour
{
    public float speed;
    public float rayDist;
    public float wayRallDist;
    public float playDetecDist;
    private bool movingRight = false;
    private bool isChasing = false;
    public GameObject player;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    [SerializeField] private float chasingDistance = 5f;
    private bool isflipped = false;
    //private bool canChase = true;
    private Rigidbody2D rb;

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

        DetectPlayer();

        if (isChasing){

            float distance = Vector2.Distance(player.transform.position, transform.position);
            if (distance > chasingDistance)
            {
                isChasing = false;
                //Debug.Log("I am no longer chasing");
            }

            Chase();

        }else {
            Patrol();
        }



    }

    //Nicklas ændring: Genbrugelig metode til at detektere om der er jord under fjenden.
    public bool IsDetectingGround()
    {
        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, rayDist, groundLayer);
        return groundCheck.collider != null;
    }

    //Nicklas ændring: Genbrugelig metode til at detektere om der er en væg foran fjenden.
    public bool IsDetectingWall()
    {
        //Nicklas ændring: Raycasten skyder nu bare ud fra fjendens positon, i stedet for et overflødigt usynligt objekt. Har fjernet unødigt spiller check.
        RaycastHit2D wallDetector = Physics2D.Raycast(transform.position, transform.right, wayRallDist, groundLayer);
        return wallDetector.collider != null;
    }

    //Nicklas ændring: Metode til at detektere spilleren, da det er en god idé at sepereere det fra Patrol metoden.
    public void DetectPlayer()
    {
        //Nicklas ændring: Raycasten skyder nu bare ud fra fjendens positon, i stedet for et overflødigt usynligt objekt. Har fjernet unødigt spiller check.
        RaycastHit2D playerDetector = Physics2D.Raycast(transform.position, transform.right, playDetecDist, playerLayer);

        if (playerDetector.collider == true && isChasing == false && playerDetector.collider.tag == "Player")
        {
            isChasing = true;
            player = playerDetector.collider.gameObject;
        }
    }
    public void Patrol()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        //Nicklas ændring: Hvis der ikke er jord under fjenden, eller der er en væg foran, så skal fjenden vende om.
        if (IsDetectingGround() == false || IsDetectingWall() == true)
        {
            Flip();

        }


    }

    private IEnumerator CanChaseAgainTimer(float timeoutDuration)
    {
        //Debug.Log("I am inside CanChaseAgain");
        yield return new WaitForSeconds(timeoutDuration);
        //canChase = true;
    }

    public void Chase()
    {

        if (IsDetectingGround() == false || IsDetectingWall() == true)
        {
            
            isChasing = false;
            //canChase = false;

            //Debug.Log("I should stop chasing player"); 
            //CanChaseAgainTimer(3f);
        }
        //Debug.Log("I am chasing");

        //Nicklas ændring: Retningen er nu fra player til enemy, da der tidligere kun blev tjekket på spillerens position.
        
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        transform.Translate(direction * speed * Time.deltaTime);



        

        
        Vector2 weaponPosition = weapon.position;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(weaponPosition, attackRange, playerLayer);
        foreach (Collider2D hit in hits)
        {
            Health healthScript = hit.GetComponent<Health>();
            if (healthScript != null) {
                healthScript.TakeDamage(damageAmount);
                String hitName = hit.name;  
                Vector2 knockbackDirection = (transform.position-hit.transform.position).normalized;
                rb.AddForce(new Vector2(knockbackDirection.x * -1f, knockbackDirection.y) * -5f);
                Debug.Log("Hit: " + hitName);
            }
        }

        //Debug.Log("here is your direction: "+direction);
        
    }


    //Gammel metode til at vende fjenden:
    public void Turnaround()
    {
        //Debug.Log("i am inside turn around");
        if (movingRight)
        {
            isflipped = true;
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingRight = false;
            //Debug.Log("I should be moving left now");

        }
        else
        {
            isflipped = false;
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingRight = true;
            //Debug.Log("I should be moving right");
        }
    }


    //Nicklas ændring: Ny flip metode, der også roterer fjenden, så den vender rigtigt.
    public void Flip()
    {
        //Debug.Log("I am flipping");
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (movingRight)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);

        }
        else if (!movingRight)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, -180f, 0f);
        }
        movingRight = !movingRight;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(weapon.position, attackRange);


        //draw raycasts:
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDist);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wayRallDist);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * playDetecDist);

    }


}
