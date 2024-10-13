using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private PlayerData playerData;
    private GameObject player;

    private void Awake(){
        if(instance != null && instance != this){
            Destroy(this.gameObject);
        }else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start(){
        playerData = SaveSystem.LoadPlayerData();
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Player data loaded"+ playerData.gemCount);

        if(!string.IsNullOrEmpty(playerData.levelName)){
            StartCoroutine(LoadSceneAndSetPosition(playerData.levelName));
        }
    }

    private IEnumerator LoadSceneAndSetPosition(string sceneName)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Set the player's position after the scene has loaded
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(playerData.lastCheckpointX, playerData.lastCheckpointY, playerData.lastCheckpointZ);
    }

    public void UpdateData(PlayerData data){
        playerData = data;
    }

    private void SavePlayerData(){
        SaveSystem.Save(playerData);
    }

    private void LoadPlayerData(){
        playerData = SaveSystem.LoadPlayerData();
    }


    public PlayerData GetPlayerData(){
        return playerData;
    }

    public void OnApplicationQuit(){
        SavePlayerData();
    }
}
