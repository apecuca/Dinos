using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool offlineMode;

    [SerializeField] private GameObject connectionOverlay;
    [SerializeField] private Button btn_multiplayer;
    [SerializeField] private Text txt_connectionOverlay;

    private string joinRoomText = "Conectando a uma sala...";

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        ConnectToMaster();
        ToggleConnectionOverlay(false, null);
    }

    private void ConnectToMaster()
    {
        if (!PhotonNetwork.IsConnected)
        {
            btn_multiplayer.interactable = false;
            Image _btnImg = btn_multiplayer.GetComponent<Image>();
            Color _c = _btnImg.color;
            _c.a = 0.5f;
            _btnImg.color = _c;

            //print("Iniciou conexão ao master...");

            if (offlineMode)
                return;

            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(RetryMasterConnection());
        }
    }

    private IEnumerator RetryMasterConnection()
    {
        yield return new WaitForSeconds(15f);
        
        if (!PhotonNetwork.IsConnected)
        {
            print("Falha na conexão ao master, tentando de novo");
            ConnectToMaster();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        btn_multiplayer.interactable = true;
        Image _btnImg = btn_multiplayer.GetComponent<Image>();
        Color _c = _btnImg.color;
        _c.a = 1f;
        _btnImg.color = _c;

        //print("Conectou no master :)");
    }

    public void OnMultiplayerBtnClicked()
    {
        ToggleConnectionOverlay(true, joinRoomText);
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.JoinRandomRoom();
        else
            OnJoinRandomFailed(-1, "Not connected to master server");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //print($"Falhou em entrar em sala aleatória.\n{message}, errCod: {returnCode}");

        ToggleConnectionOverlay(false, null);
        CreateRoom();
    }

    private void CreateRoom()
    {
        ToggleConnectionOverlay(true, "Nenhuma sala disponível, criando uma...");

        int _randomRoomName = Random.Range(0, 10);

        // RETIRAR, RETURN PARA FEATURE NÃO IMPLEMENTADA ATÉ O FINAL :)
        // somente por conveniência, pra eu n precisar reescrever essa parte
        ToggleConnectionOverlay(true, "Feature ainda não implementada, calminha aí ;)", true);
        return;

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.CreateRoom($"Room {_randomRoomName}", GetRoomOps());
        else
            OnCreateRoomFailed(-1, "Not connected to master server");
    }

    RoomOptions GetRoomOps()
    {
        return new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)PhotonRoom.room.maxPlayers
        };
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //print("Falhou em criar sala.");
        ToggleConnectionOverlay(true, $"Falha na criação de sala.\n{message}, errCod: {returnCode}", true);
    }

    public void ToggleConnectionOverlay(bool _vl, string _connectionText, bool _autoClose = false)
    {
        connectionOverlay.SetActive(_vl);
        if (_connectionText != null ||
            _connectionText != "")
            txt_connectionOverlay.text = _connectionText;

        if (_autoClose)
            StartCoroutine(AutoCloseConnectionOverlay());

    }

    private IEnumerator AutoCloseConnectionOverlay()
    {
        yield return new WaitForSeconds(3f);

        ToggleConnectionOverlay(false, null);
    }

}
