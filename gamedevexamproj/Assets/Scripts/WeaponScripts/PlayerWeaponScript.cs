using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponScript : MonoBehaviour {

    [SerializeField] private Transform weaponRotationPoint;
    [SerializeField] private Transform weapon;
    [SerializeField] private float attackRange = 0.25f;
    [SerializeField] private LayerMask attackLayer; 
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] float attackDuration = 0.2f;
    [SerializeField] float swingAngle = 45f;
    [SerializeField] private float rotationOffset = 90f;
    [SerializeField] private ParticleSystem weaponSwingEffect;
    [SerializeField] private Transform player;
    [SerializeField] GameObject hitEffect;
    [SerializeField] private Transform[] hitAreas;
    [SerializeField] private ObjectPool hitEffectPool;
    [SerializeField] private AudioSource AttackAudioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;
  
    private bool canAttack = true;
    private Vector3 mousePosition;
    HashSet<Collider2D> uniqueHits = new HashSet<Collider2D>();

    void Update() {
        RotateWeaponTowardsMouse();
    }
    void RotateWeaponTowardsMouse() {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector3 direction = (mousePosition - weaponRotationPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - rotationOffset;
        weaponRotationPoint.rotation = Quaternion.Euler(0f, 0f, angle);
        //Debug.Log("Mouse Position: " + mousePosition + ", Calculated Angle: " + angle);
    }

    public void PerformAttack() {
        if (!canAttack) return;
        canAttack = false; 
        Debug.Log("PerformAttack");
        AttackAudioSource.PlayOneShot(attackSound);
        uniqueHits.Clear();
        StartCoroutine(AttackAnimation());
        CheckAllHitAreas();
    }
    private void CheckAllHitAreas() {
        foreach (Transform hitArea in hitAreas) {
            Vector2 hitAreaPosition = weapon.TransformPoint(hitArea.localPosition);
            Collider2D[] hits = Physics2D.OverlapCircleAll(hitAreaPosition, attackRange, attackLayer);

            foreach (Collider2D hit in hits) {
                if (uniqueHits.Add(hit)) {
                    ProcessHit(hit, hitAreaPosition);
                }
            }
        }
    }
     private void ProcessHit(Collider2D hit, Vector2 hitAreaPosition) {
        //Health healthScript = hit.GetComponent<Health>();
        //if (healthScript != null) {
        //    healthScript.TakeDamage(damageAmount);
        //}
        AttackAudioSource.PlayOneShot(hitSound);
        GameObject hitAnimInstance = hitEffectPool.GetPooledObject();
        Vector2 hitPosition = (hitAreaPosition + (Vector2)hit.transform.position) / 2f;
        
        hitAnimInstance.transform.position = hitPosition;
        StartCoroutine(ReturnHitEffectToPool(hitAnimInstance));
        Debug.Log("Hit: " + hit.name);
    }
    private IEnumerator ReturnHitEffectToPool(GameObject hitAnimInstance) {
        float animationDuration = hitAnimInstance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationDuration);
        hitEffectPool.ReturnObjectToPool(hitAnimInstance);
    }
    private IEnumerator AttackAnimation() {
        TrailRenderer trail = weapon.GetComponent<TrailRenderer>();
        trail.enabled = true;
        weaponSwingEffect.Play();
        bool isWeaponOnRight = mousePosition.x > player.position.x? true : false;
        Quaternion initialRotation, targetRotation;

        if (isWeaponOnRight) {
            // Weapon is on the right: Attack from top-right to bottom-right
            initialRotation = Quaternion.Euler(0f, 0f, weaponRotationPoint.eulerAngles.z - swingAngle);
            targetRotation = Quaternion.Euler(0f, 0f, weaponRotationPoint.eulerAngles.z);
        } else {
            // Weapon is on the left: Attack from top-left to bottom-left
            initialRotation = Quaternion.Euler(0f, 0f, weaponRotationPoint.eulerAngles.z + swingAngle);
            targetRotation = Quaternion.Euler(0f, 0f, weaponRotationPoint.eulerAngles.z);
        }
        float elapsedTime = 0f;
        // Perform the attack swing
        while (elapsedTime < attackDuration) {
            elapsedTime += Time.deltaTime;
            weaponRotationPoint.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / attackDuration);
            yield return null; // Wait for the next frame
        }
        yield return new WaitForSeconds(0.05f); 
        // Reset the weapon rotation back to its original position
        elapsedTime = 0f;
        while (elapsedTime < attackDuration) {
            elapsedTime += Time.deltaTime;
            weaponRotationPoint.rotation = Quaternion.Slerp(targetRotation, initialRotation, elapsedTime / attackDuration);
            yield return null;
        }
        trail.enabled = false;
        canAttack = true;
}


    private void OnDrawGizmosSelected() {
         Gizmos.color = Color.red;

        if (hitAreas != null) {
            foreach (Transform hitArea in hitAreas) {
                Gizmos.DrawWireSphere(hitArea.position, attackRange);
            }
        }
    }
}
