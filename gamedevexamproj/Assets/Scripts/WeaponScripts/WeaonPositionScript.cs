using UnityEngine;

public class WeaonPositionScript : MonoBehaviour {
    [SerializeField] private Transform player;
    
     void Update() {
        transform.position = player.position;
    }
}
