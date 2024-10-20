using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite collectedCheckpoint;

    void Start(){
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player")){
            int health = other.GetComponent<Health>().GetHealth();
            PlayerData playerData = GameManager.Instance.GetPlayerData();
            playerData.levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            playerData.lastCheckpointX = transform.position.x;
            playerData.lastCheckpointY = transform.position.y;
            playerData.lastCheckpointZ = transform.position.z;
            playerData.health = health;
            GameManager.Instance.UpdateData(playerData);
            spriteRenderer.sprite = collectedCheckpoint;
        }
    }
}
