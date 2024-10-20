using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponScript : MonoBehaviour {

    [SerializeField] private Transform weaponRotationPoint;
    [SerializeField] private Transform weapon;
    [SerializeField] private float attackRange = 0.25f;
    [SerializeField] private LayerMask groundAndWallLayer; 
    [SerializeField] private LayerMask attackLayer; 
    [SerializeField] private int damageAmount = 1;
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
    [SerializeField] private AudioClip wallGroundHitSound;
  
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
        CheckAllHitAreas1();
        StartCoroutine(AttackAnimation());
    }
    /*
    private void CheckAllHitAreas() {
        HashSet<Collider2D> processedHits = new HashSet<Collider2D>();
        foreach (Transform hitArea in hitAreas) {
            Vector2 hitAreaPosition = weapon.TransformPoint(hitArea.localPosition);
            Debug.Log("Hit Area Position: " + hitAreaPosition);
            Collider2D[] hits = Physics2D.OverlapCircleAll(hitAreaPosition, attackRange, attackLayer);
            foreach (Collider2D hit in hits) {
                if (!processedHits.Contains(hit)) {
                    ProcessHit(hit, hitAreaPosition); 
                    processedHits.Add(hit); 
                    break;  
                }
            }
            Collider2D[] hitsWallGround = Physics2D.OverlapCircleAll(hitAreaPosition, attackRange, groundAndWallLayer);
            foreach (Collider2D hit in hitsWallGround) {
                if (!processedHits.Contains(hit)) {
                    weaponSwingEffect.transform.position = hitAreaPosition;  
                    weaponSwingEffect.Play();        
                    break;
                }
            }
        }
    }
    */
   private void CheckAllHitAreas1() {
        foreach (Transform hitArea in hitAreas) {
            Vector2 hitAreaPosition = weapon.TransformPoint(hitArea.localPosition);
            Collider2D[] hits = Physics2D.OverlapCircleAll(hitAreaPosition, attackRange, attackLayer);
            foreach (Collider2D hit in hits) {
                if (uniqueHits.Add(hit)) {
                    ProcessHit(hit, hitAreaPosition);
                    break;
                }
            }
            if(uniqueHits.Count > 0) break; // dont want it to also hit wall/ground if it has already hit an enemy
            Collider2D[] hitsWallGround = Physics2D.OverlapCircleAll(hitAreaPosition, attackRange, groundAndWallLayer);
            foreach (Collider2D hit in hitsWallGround) {
                if (uniqueHits.Add(hit)) {
                    ProcessWallGroundHit(hit, hitAreaPosition);       
                    break;
                }
            }
        }
    }

     private void ProcessHit(Collider2D hit, Vector2 hitAreaPosition) {
        Health healthScript = hit.GetComponent<Health>();
        if (healthScript != null) {
            healthScript.TakeDamage(damageAmount);
        }
        AttackAudioSource.PlayOneShot(hitSound);
        GameObject hitAnimInstance = hitEffectPool.GetPooledObject();
        Vector2 hitPosition = (hitAreaPosition + (Vector2)hit.transform.position) / 2f;
        
        hitAnimInstance.transform.position = hitPosition;
        StartCoroutine(ReturnHitEffectToPool(hitAnimInstance));
        Debug.Log("Hit: " + hit.name);
    }
    private void ProcessWallGroundHit(Collider2D hit,Vector2 hitAreaPosition) {
        AttackAudioSource.PlayOneShot(wallGroundHitSound);
        Vector2 hitPosition = (hitAreaPosition + (Vector2)hit.transform.position) / 2f;
        weaponSwingEffect.transform.position = hitPosition;
        weaponSwingEffect.Play();
    }   
    private IEnumerator ReturnHitEffectToPool(GameObject hitAnimInstance) {
        float animationDuration = hitAnimInstance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationDuration);
        hitEffectPool.ReturnObjectToPool(hitAnimInstance);
    }
    private IEnumerator AttackAnimation() {
        TrailRenderer trail = weapon.GetComponent<TrailRenderer>();
        trail.enabled = true;
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
            CheckAllHitAreas1();
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
