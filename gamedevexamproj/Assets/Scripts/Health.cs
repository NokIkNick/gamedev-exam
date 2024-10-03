using UnityEngine;

public class Health : MonoBehaviour
{   
    [SerializeField] int maxHealth = 3;
    [SerializeField] int health = 3;
    [SerializeField] bool isDamagable = true;

    void Start()
    {
        health = maxHealth;
        TakeDamage(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TakeDamage(int damage){
        if(health > 0 && health - damage > 0 && isDamagable){
            health -= damage;
            flash();
            invincibility();
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

    private void invincibility(){
        isDamagable = false;
        Invoke("resetInvincibility", 1f);
    }

    private void resetInvincibility(){
        isDamagable = true;
    }

    public int GetHealth(){
        return health;
    }
}
