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
    public Transform groundDetect;
    public Transform wallDetect;
    public Transform playerDetect;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing)
        {
            Chase();
        }
        else 
        {
            Patrol();
        }
        

    }
    public void Patrol()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        RaycastHit2D groundCheck = Physics2D.Raycast(groundDetect.position, Vector2.down, rayDist,groundLayer);
        RaycastHit2D wallDetector = Physics2D.Raycast(wallDetect.position, transform.right, wayRallDist,groundLayer);
        RaycastHit2D playerDetector = Physics2D.Raycast(wallDetect.position, transform.right, playDetecDist,playerLayer);
        Debug.Log("here is wallDetector: " + wallDetect);
        if (groundCheck.collider == false || wallDetector.collider == true && movingRight == true)
        {

            Debug.Log("I have detected something that should make me turn");       
      
            Turnaround();
            

        }
        else if (groundCheck.collider == false || wallDetector.collider == true && movingRight == false)
        {
            
            Turnaround();
             

        }
        /*else if (wallDetector.collider != false) 
        {
            Turnaround();
        }*/

        

    }

    public void Chase() 
    {
        Vector2 direction = player.transform.position;
        direction.Normalize();
        transform.Translate(direction*speed*Time.deltaTime);
        RaycastHit2D groundCheck = Physics2D.Raycast(groundDetect.position, Vector2.down, rayDist, groundLayer);
        RaycastHit2D wallDetector = Physics2D.Raycast(wallDetect.position, transform.right, wayRallDist, groundLayer);
    }
    
    public void Turnaround()
    {
        Debug.Log("i am inside turn around");
        if (movingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingRight = false;
            Debug.Log("I should be moving left now");
            
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingRight = true;
            Debug.Log("I should be moving right");
        }
    }
}

