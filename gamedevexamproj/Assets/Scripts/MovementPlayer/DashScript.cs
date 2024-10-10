using UnityEngine;
using System.Collections;

public class DashScript : MonoBehaviour
{
    [Header("Dash Settings")]
    
    private Rigidbody2D m_Rigidbody2D;
    private bool m_IsDashing = false;
    private float m_LastDashTime = -Mathf.Infinity;
    private bool m_FacingRight = true;
    private ParticleSystem m_dashParticles;

    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        if (m_Rigidbody2D == null) {
            Debug.LogError("Rigidbody2D component not found! Make sure the GameObject has a Rigidbody2D component attached.");
        }
    }

    public void Dash(bool isFacingRight, float m_DashSpeed, float m_DashDuration, float m_DashCooldown, ParticleSystem dashParticles) {
        if (Time.time < m_LastDashTime + m_DashCooldown || m_IsDashing) {
            return;
        }
        m_LastDashTime = Time.time;
        m_FacingRight = isFacingRight;
        m_dashParticles = dashParticles;
        StartCoroutine(PerformDash(m_DashSpeed, m_DashDuration));
    }

    private IEnumerator PerformDash(float m_DashSpeed, float m_DashDuration) {

        PlayDashParticles();
        m_IsDashing = true;
        float originalGravity = m_Rigidbody2D.gravityScale;
        m_Rigidbody2D.gravityScale = 0;
        float dashDirection = m_FacingRight ? 1 : -1;
        m_Rigidbody2D.AddForce(new Vector2(dashDirection * m_DashSpeed, 0f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(m_DashDuration);
        m_Rigidbody2D.gravityScale = originalGravity;
        m_IsDashing = false;
    }

    private void PlayDashParticles() {
        if (m_dashParticles != null) {
            ParticleSystem particles = Instantiate(m_dashParticles, transform.position, Quaternion.identity);
            particles.transform.SetParent(transform); 
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
        }
    }

    public bool GetIsDashing(){
        return m_IsDashing;
    }
}

