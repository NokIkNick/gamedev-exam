using UnityEngine;

public class ConditionalDoor : MonoBehaviour
{   
    [SerializeField] private bool isOpen = false;
    [SerializeField] private GameObject[] conditions;
    private BoxCollider2D boxCollider;
    private AudioSource audioSource;
    private AudioClip openSound;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = true;
        audioSource = GetComponent<AudioSource>();
        openSound = Resources.Load<AudioClip>("Sounds/World/ConditionalDoors/doorOpen");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(conditions.Length == 0){
            return;
        }
        
        if(CheckConditions() && !isOpen){
            isOpen = true;
        }

        if(isOpen && boxCollider.enabled){
            boxCollider.enabled = false;
            spriteRenderer.color = Color.black;
            PlayOpenSound();
        }
    }


    private bool CheckConditions(){
        int count = 0;
        foreach(GameObject condition in conditions){
            if(!condition.activeSelf){
                count ++;
            }
        }
        if(count == conditions.Length){
            return true;
        }
        return false;
    }

    private void PlayOpenSound(){
        audioSource.PlayOneShot(openSound);
    }
}
