using UnityEngine;
using UnityEngine.Audio;

public class Health : MonoBehaviour
{   
    [SerializeField] int maxHealth = 3;
    [SerializeField] int health = 3;
    [SerializeField] bool isDamagable = true;
    private AudioSource audioSource;
    private AudioClip hurtSound;

    void Start()
    {
        health = maxHealth;
        audioSource = GetComponent<AudioSource>();
        hurtSound = Resources.Load<AudioClip>("Sounds/hurtSound");
        Debug.Log(hurtSound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Heal(int amount){
        if(health + amount > maxHealth){
            health = maxHealth;
        }
        else{
            health += amount;
        }
    }

    private void PlayHurtSound(){
        audioSource.PlayOneShot(hurtSound);
    }

    public void TakeDamage(int damage){
        if(!isDamagable){
            return;
        }

        PlayHurtSound();

        if(health > 0 && health - damage > 0 && isDamagable){
            health -= damage;
            Flash();
            Invincibility();
        }else{
            if(gameObject.tag == "Boss"){
                health -= damage;
                return;
            }
            
            if(gameObject.tag == "Player"){
                GameManager.Instance.ResetAndKillPlayer();
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
        gameObject.SetActive(false);
    }

    public void Invincibility(){
        isDamagable = false;
        Invoke("ResetInvincibility", 1.5f);
    }

    public void Invincibility(float duration){
        isDamagable = false;
        Invoke("ResetInvincibility", duration);
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

    public void SetHealth(int newHealth){
        health = newHealth;
    }
}
