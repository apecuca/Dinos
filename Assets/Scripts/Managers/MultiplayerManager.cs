﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerManager : GameManager
{
    [Header("Multiplayer stuff")]
    [SerializeField] private Text[] leaderboardElements;
    [SerializeField] private PhotonView pv;

    [Header("Pre-game assignables")]
    [SerializeField] private Text txt_playersReady;
    [SerializeField] private GameObject startGameButton;

    private MultiplayerDino myMultiplayerDino;
    private List<MultiplayerDino> dinos = new List<MultiplayerDino>();

    public static MultiplayerManager instance { get; private set; }

    protected override void Awake()
    {
        //base.Awake();

        instance = this;
    }

    protected override void Start()
    {
        //base.Start();

        HUD_ingame.SetActive(true);
        HUD_gameover.SetActive(false);
        HUD_startGame.SetActive(true);
        HUD_paused.SetActive(false);

        this.enabled = false;
    }

    protected override void Update()
    {
        //base.Update();
    }

    #region AO ENTRAR/SAIR
    public void OnMyDinoSpawned(MultiplayerDino _dino)
    {
        myMultiplayerDino = _dino;
    }

    public void OnDinoJoined()
    {
        pv.RPC("RPC_OnDinoJoined", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_OnDinoJoined()
    {
        // isso aqui roda em todos os GameManagers assim que um dino termina de nascer
        if (myMultiplayerDino == null) return;

        myMultiplayerDino.UpdateCosmetics();
        UpdateDinoList();
    }

    public void UpdateDinoList()
    {
        pv.RPC("RPC_UpdateDinoList", RpcTarget.All);
    }

    #endregion

    #region LEADERBOARD

    [PunRPC]
    public void RPC_UpdateDinoList()
    {
        GameObject[] _allDinos = GameObject.FindGameObjectsWithTag("Player");

        List<MultiplayerDino> _tempDinos = new List<MultiplayerDino>();
        foreach (GameObject _d in _allDinos)
            _tempDinos.Add(_d.GetComponent<MultiplayerDino>());

        for (int i = 0; i < dinos.Count; i++)
        {
            if (!_tempDinos.Contains(dinos[i]))
                dinos.Remove(dinos[i]);
        }

        for (int i = 0; i < _tempDinos.Count; i++)
        {
            if (!dinos.Contains(_tempDinos[i]))
                dinos.Add(_tempDinos[i]);
        }

        UpdateLeaderboard();
    }

    public void UpdateLeaderboard()
    {
        if (!PhotonNetwork.IsConnected) return;
        if (!PhotonNetwork.InRoom) return;
        if (leaderboardElements.Length < 8) return;

        for (int i = 0; i < leaderboardElements.Length; i++)
        {
            if (i < dinos.Count)
            {
                leaderboardElements[i].gameObject.SetActive(true);
                leaderboardElements[i].text = $"{dinos[i].GetDinoNickname()}";
                continue;
            }

            leaderboardElements[i].gameObject.SetActive(false);
        }

        UpdateReadyCount();
    }

    #endregion

    #region GAME HANDLERS

    public void ToggleReady()
    {
        myMultiplayerDino.ToggleReady();
    }

    public void UpdateReadyCount()
    {
        int _pReady = 0;
        foreach (MultiplayerDino _d in dinos)
            if (_d.ready) _pReady++;

        txt_playersReady.text = $"PLAYERS READY\n{_pReady}/{dinos.Count}";

        if (!PhotonNetwork.IsMasterClient)
            return;

        if (_pReady == dinos.Count &&
            _pReady >= 2)
            startGameButton.SetActive(true);
        else
            startGameButton.SetActive(false);
    }

    public override void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;

        pv.RPC("RPC_StartGame", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_StartGame()
    {
        HUD_startGame.SetActive(false);
        startGameButton.SetActive(false);
        HUD_ingame.SetActive(true);
    }


    #endregion

    public MultiplayerDino GetMyDino()
    {
        return myMultiplayerDino;
    }


    public override void PauseUnpause(bool _vl)
    {
        //base.PauseUnpause(_vl);

        HUD_ingame.SetActive(!_vl);
        HUD_paused.SetActive(_vl);
    }


    public override void GOTO_menu()
    {
        //base.GOTO_menu();
        if (PhotonRoom.room == null)
            return;

        PhotonRoom.room.DisconnectPlayer();
    }
}