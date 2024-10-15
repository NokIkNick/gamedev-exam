using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackScript : MonoBehaviour {
    [SerializeField] private PlayerWeaponScript playerWeaponScript;

    void Awake() {
           if (playerWeaponScript == null) {
            GameObject weaponObject = GameObject.Find("WeaponModel");
            if (weaponObject != null) {
                playerWeaponScript = weaponObject.GetComponent<PlayerWeaponScript>();
                if (playerWeaponScript == null) {
                    Debug.LogError("PlayerWeaponScript component not found on the weapon object!");
                }
            } else {
                Debug.LogError("Weapon object not found!");
            }
        }
    }
    public void Attack(InputAction.CallbackContext context) {   
        if (context.performed) {
            playerWeaponScript.PerformAttack();
        }
    }
}
