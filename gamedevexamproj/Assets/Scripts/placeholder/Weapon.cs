using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetAttackDamage(){
        return damage;
    }
}
