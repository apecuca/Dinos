using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool offlineMode;

    [SerializeField] private Button btn_multiplayer;
    [SerializeField] private MenuManager mng_menu;
    [SerializeField] private Text lb_playerCount;

    private string joinRoomText = "Conectando a uma sala...";
    private string createRoomText = "Nenhuma sala disponível, criando uma...";
    private string failedCreateRoomText = "Falha na criação de sala.";
    private string notConnectedText = "Desconectado do servidor master";

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(ConnectToMaster());
        mng_menu.ToggleTextOverlay(false, null);
    }

    private IEnumerator ConnectToMaster()
    {
        btn_multiplayer.interactable = false;
        Image _btnImg = btn_multiplayer.GetComponent<Image>();
        Color _c = _btnImg.color;
        _c.a = 0.5f;
        _btnImg.color = _c;


        if (PhotonNetwork.IsConnected)
        {
            if (!PhotonNetwork.IsConnectedAndReady)
                yield return new WaitForEndOfFrame();

            OnConnectedToMaster();
            yield break;
        }
        if (offlineMode)
            yield break;

        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        btn_multiplayer.interactable = true;
        Image _btnImg = btn_multiplayer.GetComponent<Image>();
        Color _c = _btnImg.color;
        _c.a = 1f;
        _btnImg.color = _c;

        lb_playerCount.gameObject.SetActive(true);
        UpdatePlayerCount();
    }

    public void UpdatePlayerCount()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        lb_playerCount.text = $"Players online: {PhotonNetwork.CountOfPlayers}";
    }


    public void OnMultiplayerBtnClicked()
    {
        mng_menu.ToggleTextOverlay(true, joinRoomText);

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            OnJoinRandomFailed(-1, notConnectedText);
            return;
        }
            

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        mng_menu.ToggleTextOverlay(false, null);
        CreateRoom();
    }

    private void CreateRoom()
    {
        mng_menu.ToggleTextOverlay(true, createRoomText);

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            OnCreateRoomFailed(-1, notConnectedText);
            return;
        }

        string _roomName;
        string _nickname = SaveInfo.GetInstance().GetNickname();
        if (_nickname != "" &&
            _nickname != null)
        {
            int _roomNumber = Random.Range(0, 1000);
            _roomName = $"{_nickname}'s room {_roomNumber}";
        }
        else
            _roomName = $"Rando's room {Random.Range(0, 1000)}";
        PhotonNetwork.CreateRoom(_roomName, GetRoomOps());
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
        mng_menu.ToggleTextOverlay(true, $"{failedCreateRoomText}\n{message}, errCod: {returnCode}", true);
    }

    

}
