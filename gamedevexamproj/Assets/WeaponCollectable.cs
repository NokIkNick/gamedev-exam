using UnityEngine;

public class WeaponCollectable : Collectable
{
    public override void PickUpEffect(GameObject player)
    {
        player.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
        GameManager.Instance.UpdateData(new PlayerData { hasWeapon = true });
    }
}
