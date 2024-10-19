using UnityEngine;

public class GemCollectable : Collectable
{
    [SerializeField] private int value = 1;
    public override void PickUpEffect(GameObject player)
    {
        int? gemCount = GameManager.Instance.GetPlayerData().gemCount;
        if(gemCount == null){
            GameManager.Instance.GetPlayerData().gemCount = 0;
        }
        GameManager.Instance.UpdateData(new PlayerData { gemCount = GameManager.Instance.GetPlayerData().gemCount + value });
    }
}
