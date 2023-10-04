using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Profiling;
using System.Diagnostics;
using Photon.Pun;

public class MenuManager : MonoBehaviour
{
    [Header("HUDs")]
    [SerializeField] private GameObject HUD_Main;
    [SerializeField] private GameObject HUD_Settings;
    [SerializeField] private GameObject HUD_Customization;

    [Header("Overlay")]
    [SerializeField] private GameObject textOverlayObj;
    [SerializeField] private Text txt_textOverlay;

    [Header("Settings")]
    [SerializeField] private Toggle tog_sfx;

    [Header("Customization")]
    [SerializeField] private InputField inp_nickname;
    [SerializeField] private Toggle tog_hideNick;

    [Header("Assignables")]
    [SerializeField] private PhotonLobby photonLobby;
    [SerializeField] private Text lb_version;

    private int singleplayerSceneID = 1;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        LoadSettings();
        LoadCustomization();

        string _versionText = $"Dinos ";
        if (SystemInfo.deviceType == DeviceType.Desktop)
            _versionText += "PC";
        else if (SystemInfo.deviceType == DeviceType.Handheld)
            _versionText += "MOBILE";
        _versionText += $"_{Application.version}";

        lb_version.text = _versionText;

        GotoHUD(0);
    }

    // HUDs

    public void GotoSingleplayer()
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        Destroy(PhotonRoom.room.gameObject);
        SceneManager.LoadScene(singleplayerSceneID);
    }

    public void GotoMultiplayer()
    {
        photonLobby.OnMultiplayerBtnClicked();
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

        // salvar as paradas aqui
        if (inp_nickname.text != null &&
            inp_nickname.text != "")
            SaveInfo.GetInstance().SetNickname(inp_nickname.text);

        SaveInfo.GetInstance().Salvar();
    }

    
    // configs

    private void LoadSettings()
    {
        tog_sfx.isOn = SaveInfo.GetInstance().GetSfxOn();
    }

    public void OnToggleSfxChanged()
    {
        SaveInfo.GetInstance().SetSfxOn(tog_sfx.isOn);
    }


    // customização

    private void LoadCustomization()
    {
        SaveInfo _instance = SaveInfo.GetInstance();

        string _savedNickname = _instance.GetNickname();
        if (_savedNickname != null &&
            _savedNickname != "" &&
            _savedNickname != " ")
        {
            inp_nickname.placeholder.GetComponent<Text>().text = _savedNickname;
        }
        else
        {
            inp_nickname.text = "";
            inp_nickname.placeholder.GetComponent<Text>().text = "NICKNAME...";
        }

        tog_hideNick.isOn = _instance.GetHideNick();
    }

    public void OnToggleHideNickChanged()
    {
        SaveInfo.GetInstance().SetHideNick(tog_hideNick.isOn);
    }


    // misc

    public void ToggleTextOverlay(bool _vl, string _connectionText, bool _autoClose = false)
    {
        textOverlayObj.SetActive(_vl);
        if (_connectionText != null ||
            _connectionText != "")
            txt_textOverlay.text = _connectionText;

        if (_autoClose)
            StartCoroutine(AutoCloseConnectionOverlay());
    }

    private IEnumerator AutoCloseConnectionOverlay()
    {
        yield return new WaitForSeconds(3f);

        ToggleTextOverlay(false, null);
    }


    public void ResetSave()
    {
        SaveInfo.GetInstance().ResetarSave();

        // carregar settings para aplicar todas as coisas zeradas
        LoadSettings();
        LoadCustomization();

        GotoHUD(0);
        ToggleTextOverlay(true, "Save deletado com sucesso", true);
    }

}
