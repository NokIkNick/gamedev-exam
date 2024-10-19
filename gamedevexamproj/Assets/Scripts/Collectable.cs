using Unity.VisualScripting;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public abstract void PickUpEffect(GameObject player);

    private void OnCollisionEnter2D(Collision2D other){
        if(other.collider.CompareTag("Player")){
            Debug.Log("Picked up");
            PickUpEffect(other.gameObject);
            gameObject.SetActive(false);
        }
    }
}
