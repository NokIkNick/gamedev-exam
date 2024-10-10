using UnityEngine;

public class BossRocketScript : MonoBehaviour
{
    private Rigidbody2D m_rigidbody2D;
    [SerializeField] private float m_speed = 10f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float circlePos = 1f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float circleOffsetX = 0.5f;
    [SerializeField] private float circleOffsetY = 0.5f;
    [SerializeField] private LayerMask layerMask;
    private Vector2 m_direction;
    private Vector2 m_targetPosition;
    private float travelTime = 0f;
    void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsDirection();
        Explode();
        travelTime += Time.deltaTime;
    }

    public void Initialize (Vector2 direction, Vector2 startPosition, Vector2 targetPosition)
    {
        // Set the direction of the rocket
        transform.right = direction;
        m_direction = direction;
        transform.position = startPosition;
        m_targetPosition = targetPosition;
        travelTime = 0f;
        gameObject.SetActive(true);
    }

    public void MoveTowardsDirection(){
        Vector2 newPos = m_rigidbody2D.position + m_direction * m_speed * Time.fixedDeltaTime;
        m_rigidbody2D.MovePosition(newPos);
    }

    public void Explode()
{
    
    Vector2 circlePosition = new Vector2(transform.position.x + circleOffsetX, transform.position.y + circleOffsetY);
    Collider2D colInfo = Physics2D.OverlapCircle(circlePosition, attackRange, layerMask);
    if (colInfo != null){
        Debug.Log("Hit: " + colInfo.name);
        colInfo.GetComponent<Health>()?.TakeDamage(attackDamage);
        Vector2 hitDirection = colInfo.transform.position - transform.position;
        colInfo.GetComponent<Rigidbody2D>()?.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);
        gameObject.SetActive(false);

    }else {
        if(travelTime > 3f){
            gameObject.SetActive(false);
        }
    }
}

private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Vector3 pos = transform.position;
    pos += transform.right * circleOffsetX;
    pos += transform.up * circleOffsetY;
    Gizmos.DrawWireSphere(pos, attackRange);
}
}
