using UnityEngine;

public class BossSoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip warningSound;
    private AudioClip awakeSound;
    private AudioClip deathSound;
    private AudioClip enrageSound;
    private AudioClip shootingSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        warningSound = Resources.Load<AudioClip>("Sounds/warningSound");
        awakeSound = Resources.Load<AudioClip>("Sounds/awakeSound");
        deathSound = Resources.Load<AudioClip>("Sounds/deathSound");
        enrageSound = Resources.Load<AudioClip>("Sounds/enrageSound");
        shootingSound = Resources.Load<AudioClip>("Sounds/shootingSound");
    }

    public void PlayAwakeSound(){
        audioSource.PlayOneShot(awakeSound);
    }

    public void PlayDeathSound(){
        audioSource.PlayOneShot(deathSound);
    }

    public void PlayWarningSound(){
        audioSource.PlayOneShot(warningSound);
    }

    public void PlayEnrageSound(){
        audioSource.PlayOneShot(enrageSound);
    }

    public void PlayShootingSound(){
        audioSource.PlayOneShot(shootingSound);
    }



}
