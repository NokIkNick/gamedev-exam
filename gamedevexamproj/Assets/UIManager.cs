using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance { get { return instance; } }

    private GameObject mainMenu;
    private GameObject playerUI;
    private GameObject loadingScreen;
    private Text playerHealth;
    private Text playerGemCount;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        loadingScreen = gameObject.transform.GetChild(0).gameObject;
        playerUI = gameObject.transform.GetChild(1).gameObject;
        mainMenu = gameObject.transform.GetChild(2).gameObject;

        playerHealth = playerUI.transform.GetChild(0).GetComponent<Text>();
        playerGemCount = playerUI.transform.GetChild(1).GetComponent<Text>();

    }

    void Update(){
        if(playerUI.activeSelf){
            playerHealth.text = "Health: " + GameManager.Instance.GetPlayer().GetComponent<Health>().GetHealth();
            playerGemCount.text = "Gems: " + GameManager.Instance.GetPlayerData().gemCount;
        }
    }


    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        playerUI.SetActive(false);
        loadingScreen.SetActive(false);
    }

    public void ShowPlayerUI()
    {
        mainMenu.SetActive(false);
        playerUI.SetActive(true);
        loadingScreen.SetActive(false);
    }

    public void ShowLoadingScreen()
    {
        Debug.Log("Showing loading screen");
        mainMenu.SetActive(false);
        playerUI.SetActive(false);
        loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        Debug.Log("Hiding loading screen");
        loadingScreen.SetActive(false);
    }


}
