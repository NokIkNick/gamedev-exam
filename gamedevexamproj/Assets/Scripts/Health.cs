using UnityEngine;

public class Health : MonoBehaviour
{   
    [SerializeField] int maxHealth = 3;
    [SerializeField] int health = 3;
    [SerializeField] bool isDamagable = true;

    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TakeDamage(int damage){
        if(health > 0 && health - damage > 0 && isDamagable){
            health -= damage;
            Flash();
            Invincibility();
        }else{
            if(gameObject.tag == "Boss"){
                return;
            }
            
            Flash();
            Die();
        }
    }

    public void Flash(){
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.1f);
    }

    public void ResetColor(){
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Die(){
        Destroy(gameObject);
    }

    public void Invincibility(){
        isDamagable = false;
        Invoke("ResetInvincibility", 1f);
    }

    private void ResetInvincibility(){
        isDamagable = true;
    }

    public int GetHealth(){
        return health;
    }

    public int GetMaxHealth(){
        return maxHealth;
    }
}
