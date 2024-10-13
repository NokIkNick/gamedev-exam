using Unity.VisualScripting;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public abstract void PickUpEffect();

    private void OnCollisionEnter2D(Collision2D other){
        if(other.collider.CompareTag("Player")){
            Debug.Log("Picked up");
            PickUpEffect();
            gameObject.SetActive(false);
        }
    }
}
