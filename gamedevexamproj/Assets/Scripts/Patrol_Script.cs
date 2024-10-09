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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        DetectPlayer();

        if (isChasing)
        {
            
            float distance = Vector2.Distance(player.transform.position, transform.position);
            if(distance > chasingDistance){
                isChasing = false;
                Debug.Log("I am no longer chasing");
            }

            Chase();
        }
        else 
        {
            Patrol();
        }
        
        

    }

    //Nicklas ændring: Genbrugelig metode til at detektere om der er jord under fjenden.
    public bool IsDetectingGround(){
        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, rayDist,groundLayer);
        return groundCheck.collider != null;
    }

    //Nicklas ændring: Genbrugelig metode til at detektere om der er en væg foran fjenden.
    public bool IsDetectingWall(){
        //Nicklas ændring: Raycasten skyder nu bare ud fra fjendens positon, i stedet for et overflødigt usynligt objekt. Har fjernet unødigt spiller check.
        RaycastHit2D wallDetector = Physics2D.Raycast(transform.position, transform.right, wayRallDist, groundLayer);
        return wallDetector.collider != null;
    }

    //Nicklas ændring: Metode til at detektere spilleren, da det er en god idé at sepereere det fra Patrol metoden.
    public void DetectPlayer(){
        //Nicklas ændring: Raycasten skyder nu bare ud fra fjendens positon, i stedet for et overflødigt usynligt objekt. Har fjernet unødigt spiller check.
        RaycastHit2D playerDetector = Physics2D.Raycast(transform.position, transform.right, playDetecDist, playerLayer);
        
        if(playerDetector.collider == true && isChasing == false && playerDetector.collider.tag == "Player"){
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

    public void Chase() 
    {
        Debug.Log("I am chasing");

        //Nicklas ændring: Retningen er nu fra player til enemy, da der tidligere kun blev tjekket på spillerens position.
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        transform.Translate(direction * speed * Time.deltaTime);


    }
    

    //Gammel metode til at vende fjenden:
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


    //Nicklas ændring: Ny flip metode, der også roterer fjenden, så den vender rigtigt.
    public void Flip(){
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if(movingRight){
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            
        }else if(!movingRight){
            transform.localScale = flipped;
            transform.Rotate(0f, -180f, 0f);
        }
        movingRight = !movingRight;
    }


}

