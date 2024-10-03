using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health = 3;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage){
        if(health > 0 && health - damage > 0){
            health -= damage;
        }else {
            Die();
        }
    }

    public void flash(){
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("resetColor", 0.1f);
    }

    public void resetColor(){
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Die(){
        Destroy(gameObject);
    }
}
