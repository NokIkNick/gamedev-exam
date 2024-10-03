using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    private Health healthScript;
    private Slider healthBar;
    void Start()
    {
        healthScript = gameObject.GetComponent<Health>();
        healthBar = transform.parent.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Slider>();
        healthBar.maxValue = healthScript.GetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = healthScript.GetHealth();
    }
}
