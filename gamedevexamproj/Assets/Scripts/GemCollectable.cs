using UnityEngine;

public class GemCollectable : Collectable
{
    [SerializeField] private int value = 1;
    public override void PickUpEffect()
    {
        GameManager.Instance.UpdateData(new PlayerData { gemCount = GameManager.Instance.GetPlayerData().gemCount + value });
    }
}
