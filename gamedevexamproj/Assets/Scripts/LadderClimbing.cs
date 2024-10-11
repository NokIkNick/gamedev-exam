using UnityEngine;

public class LadderClimbing : MonoBehaviour
{
    private float originalGravityScale;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){
            GameObject player = other.gameObject;
            originalGravityScale = player.GetComponent<Rigidbody2D>().gravityScale;
            player.GetComponent<Rigidbody2D>().gravityScale = 0;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().angularVelocity = 0;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){
            GameObject player = other.gameObject;
            player.GetComponent<Rigidbody2D>().gravityScale = originalGravityScale;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }
    }
}
