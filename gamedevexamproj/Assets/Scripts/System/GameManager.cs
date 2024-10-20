using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private PlayerData playerData;
    private GameObject player;
    private bool isLoading = false;

    private bool firstStart = true;
    //private ValueTuple<System.Attribute, System.Reflection.FieldInfo>[] dataFields;

    private void Awake(){
        if(instance != null && instance != this){
            Destroy(this.gameObject);
        }else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start(){

        Initialize();
    }

    private void Initialize(){
        Time.timeScale = 0;
        playerData = SaveSystem.LoadPlayerData();
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Player data loaded"+ playerData.gemCount);

        if(!string.IsNullOrEmpty(playerData.levelName)){
            StartCoroutine(LoadSceneAndSetPosition(playerData.levelName));
        }

        Debug.Log("Players weapon:" + player.transform.parent.GetChild(1).gameObject.name);
        if(playerData.hasWeapon != null){
            player.transform.parent.transform.GetChild(1).gameObject.SetActive((bool) playerData.hasWeapon);
        }else {
            playerData.hasWeapon = false;
        }
        player.transform.parent.transform.GetChild(1).gameObject.SetActive((bool) playerData.hasWeapon);

        if(playerData.health != null){
            player.GetComponent<Health>().SetHealth((int) playerData.health);
        }

        if(!isLoading){
            UIManager.Instance.ShowMainMenu();
        }

    }

    private IEnumerator LoadSceneAndSetPosition(string sceneName)
    {
        isLoading = true;
        UIManager.Instance.ShowLoadingScreen();

        yield return null;

        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Set the player's position after the scene has loaded
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3((float) playerData.lastCheckpointX, (float) playerData.lastCheckpointY, (float) playerData.lastCheckpointZ);
        
        UIManager.Instance.HideLoadingScreen();
        
        if(firstStart){
            Time.timeScale = 0;
            UIManager.Instance.ShowMainMenu();
        }else {
            StartGame();
        }
    }

    /* PRØVEDE AT OPDATERE DATAEN VED HJÆLP AF REFLECTION. DET VIRKEDE IKKE...
    public void UpdateData(PlayerData data){
        System.Attribute[] attributes = System.Attribute.GetCustomAttributes(data.GetType());
        System.ValueTuple<System.Attribute, System.Reflection.FieldInfo>[] newDatafields = new System.ValueTuple<System.Attribute, System.Reflection.FieldInfo>[attributes.Length];
        for (int i = 0; i < attributes.Length; i++)
        {
            var newDatafield = newDatafields[i].Item2.GetValue(data);
            var datafield = dataFields[i].Item2.GetValue(this.playerData);
            if(newDatafield != datafield){
                dataFields[i].Item2.SetValue(playerData, newDatafield);
            }
        }
        
    }

    private void LoadDataFields(){
        System.Attribute[] attributes = System.Attribute.GetCustomAttributes(this.playerData.GetType());
        dataFields = new System.ValueTuple<System.Attribute, System.Reflection.FieldInfo>[attributes.Length];
    }
    */

    // Blev nødt til at gøre dette i stedet for at bruge reflection, ikke en optimal løsning. Dog er reflection meget langsomt og ikke optimalt at bruge i Unity.
    public void UpdateData(PlayerData data){
        if(data.gemCount != null){
            playerData.gemCount = data.gemCount;
            Debug.Log("Gem count updated to: " + playerData.gemCount);
        }

        if(data.lastCheckpointX != null){
            playerData.lastCheckpointX = data.lastCheckpointX;
            Debug.Log("Last checkpoint x updated to: " + playerData.lastCheckpointX);
        }

        if(data.lastCheckpointY != null){
            playerData.lastCheckpointY = data.lastCheckpointY;
            Debug.Log("Last checkpoint y updated to: " + playerData.lastCheckpointY);
        }

        if(data.lastCheckpointZ != null){
            playerData.lastCheckpointZ = data.lastCheckpointZ;
            Debug.Log("Last checkpoint z updated to: " + playerData.lastCheckpointZ);
        }

        if(data.levelName != null){
            playerData.levelName = data.levelName;
            Debug.Log("Level name updated to: " + playerData.levelName);
        }

        if(data.hasWeapon != null){
            playerData.hasWeapon = data.hasWeapon;
            Debug.Log("Has weapon updated to: " + playerData.hasWeapon);
        }

        if(data.health != null){
            playerData.health = data.health;
            Debug.Log("Health updated to: " + playerData.health);
        }

    }

    public void StartGame(){
        UIManager.Instance.ShowPlayerUI();
        Time.timeScale = 1;
        firstStart = false;

    }

    private void SavePlayerData(){
        SaveSystem.Save(playerData);
    }

    private void LoadPlayerData(){
        playerData = SaveSystem.LoadPlayerData();
    }

    public GameObject GetPlayer(){
        return player;
    }

    public PlayerData GetPlayerData(){
        return playerData;
    }

    public void OnApplicationQuit(){
        SavePlayerData();
    }

    public void ResetToLastCheckpoint(){
        if(playerData.lastCheckpointX != null && playerData.lastCheckpointY != null && playerData.lastCheckpointZ != null){
            player.transform.position = new Vector3((float) playerData.lastCheckpointX, (float) playerData.lastCheckpointY, (float) playerData.lastCheckpointZ);
            player.GetComponent<Health>().TakeDamage(1);
        }else {
            ResetAndKillPlayer();
        }
    }

    public void ResetAndKillPlayer(){
        Initialize();
    }
}
