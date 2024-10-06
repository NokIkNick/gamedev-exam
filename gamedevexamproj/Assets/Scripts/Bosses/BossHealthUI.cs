using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    private Health healthScript;
    private GameObject bossUI;
    private Slider healthBar;
    [SerializeField] private string bossName;
    private TextMeshProUGUI bossNameText;
    void Start()
    {
        healthScript = gameObject.GetComponent<Health>();
        healthBar = transform.parent.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Slider>();
        bossUI = transform.parent.GetChild(1).gameObject;
        healthBar.maxValue = healthScript.GetMaxHealth();
        healthBar.value = healthScript.GetHealth();
        bossNameText = transform.parent.GetChild(1).transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        bossNameText.text = bossName;

    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = healthScript.GetHealth();
        if(healthScript.GetHealth() <= 0){
            bossUI.SetActive(false);
        }
    }
}
