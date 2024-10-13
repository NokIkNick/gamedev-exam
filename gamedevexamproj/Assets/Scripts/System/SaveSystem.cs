using UnityEngine;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/player.data";

    public static void Save(PlayerData data){
        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(savePath, json);
    }

    public static PlayerData LoadPlayerData(){
        if(System.IO.File.Exists(savePath)){
            string json = System.IO.File.ReadAllText(savePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }else {
            return new PlayerData();
        }
    }
}
