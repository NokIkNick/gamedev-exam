using UnityEngine;

public class DeathArea : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player")){
            GameManager.Instance.ResetToLastCheckpoint();
        }
    }
}
