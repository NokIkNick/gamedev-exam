using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance { get { return instance; } }

    private GameObject mainMenu;
    private GameObject playerUI;
    private GameObject loadingScreen;

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
        mainMenu.SetActive(false);
        playerUI.SetActive(false);
        loadingScreen.SetActive(true);
    }

    
}
