using UnityEngine;

public class Health : MonoBehaviour
{
    private int health = 3;
    
    public void TakeDamage(int damage){
        health -= damage;
        if(health <= 0){
            Die();
        }
    }

    private void Die(){
        Destroy(gameObject);
    }
}
