using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("HUDs")]
    [SerializeField] private GameObject HUD_Main;
    [SerializeField] private GameObject HUD_Settings;
    [SerializeField] private GameObject HUD_Customization;

    private int singleplayerSceneID = 1;

    private void Start()
    {
        GotoHUD(0);
    }

    public void GotoSingleplayer()
    {
        SceneManager.LoadScene(singleplayerSceneID);
    }

    public void GotoMultiplayer()
    {
        // WAM
    }

    private void DisableAllHUDs()
    {
        HUD_Main.SetActive(false);
        HUD_Settings.SetActive(false);
        HUD_Customization.SetActive(false);
    }


    public void GotoHUD(int _id)
    {
        DisableAllHUDs();

        // 0 - Main
        // 1 - Settings
        // 2 - Customization

        switch (_id)
        {
            case 0:
                HUD_Main.SetActive(true);
                break;

            case 1:
                HUD_Settings.SetActive(true);
                break;

            case 2:
                HUD_Customization.SetActive(true);
                break;

            default:
                break;
        }

    }

}
