﻿using System.Collections;
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

    private string joinRoomText = "Conectando a uma sala...";

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        ConnectToMaster();
        mng_menu.ToggleTextOverlay(false, null);
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
        mng_menu.ToggleTextOverlay(true, joinRoomText);

        if (!PhotonNetwork.IsConnected)
        {
            OnJoinRandomFailed(-1, "Not connected to master server");
            return;
        }
            

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //print($"Falhou em entrar em sala aleatória.\n{message}, errCod: {returnCode}");

        mng_menu.ToggleTextOverlay(false, null);
        CreateRoom();
    }

    private void CreateRoom()
    {
        mng_menu.ToggleTextOverlay(true, "Nenhuma sala disponível, criando uma...");

        if (!PhotonNetwork.IsConnected)
        {
            OnCreateRoomFailed(-1, "Not connected to master server");
            return;
        }


        string _roomName;
        string _nickname = SaveInfo.GetInstance().GetNickname();
        if (_nickname != "" &&
            _nickname != null)
            _roomName = $"{_nickname}'s room";
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
        //print("Falhou em criar sala.");
        mng_menu.ToggleTextOverlay(true, $"Falha na criação de sala.\n{message}, errCod: {returnCode}", true);
    }

    

}
