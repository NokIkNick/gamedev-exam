using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/player.data";

    public static void Save(PlayerData data){
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        System.IO.File.WriteAllText(savePath, json);
    }

    public static PlayerData LoadPlayerData(){
        if(System.IO.File.Exists(savePath)){
            string json = System.IO.File.ReadAllText(savePath);
            return JsonConvert.DeserializeObject<PlayerData>(json);
        }else {
            return new PlayerData();
        }
    }
}
