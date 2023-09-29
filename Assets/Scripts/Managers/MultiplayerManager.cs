using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerManager : GameManager
{
    [Header("Multiplayer stuff")]
    [SerializeField] private Text[] leaderboardNicknames;
    [SerializeField] private Text[] leaderboardScores;
    [SerializeField] private Text txt_endgame;
    [SerializeField] private PhotonView pv;
    [SerializeField] private MultiplayerParallaxEffect multPEffect;

    [Header("Pre-game assignables")]
    [SerializeField] private Text txt_playersReady;
    [SerializeField] private GameObject startGameButton;

    private MultiplayerDino myMultiplayerDino;
    private List<MultiplayerDino> dinos = new List<MultiplayerDino>();

    private float updateScoreTimer = 0f;

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

        difficulty = 0;
        highscore = 0;
        if (SaveGame.TemSave())
            highscore = SaveInfo.GetInstance().GetHighscore();
    }

    protected override void Update()
    {
        //base.Update();

        if (!started) return;

        ScoreHandler();
        //UpdateLeaderboard();
    }

    #region AO ENTRAR/SAIR

    public void OnMyDinoSpawned(MultiplayerDino _dino)
    {
        myMultiplayerDino = _dino;
        RepositionCamera();
        ScoreHandler();
    }

    private void RepositionCamera()
    {
        cam = Camera.main.transform;

        if (cam == null)
            throw new Exception("Erro ao validar a cam");

        Vector2 _screenBorders = Camera.main.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height));
        Vector3 _newPos = myMultiplayerDino.transform.position;
        _newPos.z = cam.position.z;
        _newPos.y = cam.position.y;
        _newPos.x += _screenBorders.x - 5.5f;

        cam.position = _newPos;
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
        myMultiplayerDino.UpdateReady();
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
        if (leaderboardNicknames.Length < 8) return;

        for (int i = 0; i < leaderboardNicknames.Length; i++)
        {
            if (i < dinos.Count)
            {
                leaderboardNicknames[i].gameObject.SetActive(true);
                leaderboardNicknames[i].text = $"{dinos[i].GetDinoNickname()}";

                int _intScore = (int)dinos[i].GetScore();
                string _scoreTxt = $"00000";
                _scoreTxt = _scoreTxt.Remove(0, _intScore.ToString().Length);
                leaderboardScores[i].text = $"{_scoreTxt}{_intScore}";
                continue;
            }

            leaderboardNicknames[i].gameObject.SetActive(false);
        }

        UpdateReadyCount();
    }

    public void UpdateLeaderboardStats()
    {
        if (!PhotonNetwork.IsConnected) return;
        if (!PhotonNetwork.InRoom) return;
        if (leaderboardScores.Length < 8) return;

        for (int i = 0; i < leaderboardScores.Length; i++)
        {
            if (i >= dinos.Count)
            {
                i = leaderboardScores.Length;
                continue;
            }

            int _intScore = (int)dinos[i].GetScore();
            string _scoreTxt = $"00000";
            _scoreTxt = _scoreTxt.Remove(0, _intScore.ToString().Length);

            leaderboardScores[i].text = $"{_scoreTxt}{_intScore}";
        }

    }


    protected override void ScoreHandler()
    {
        //base.ScoreHandler();
        if (myMultiplayerDino.dead) return;

        myMultiplayerDino.AddToScore(scorePerFrame * difficulty * Time.deltaTime);

        updateScoreTimer += 1f * difficulty * Time.deltaTime;

        if (updateScoreTimer < 1f) return;

        myMultiplayerDino.UpdateScore();
        updateScoreTimer = 0f;
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

        pv.RPC("RPC_StartGame", RpcTarget.All, PhotonNetwork.GetPing(), PhotonNetwork.ServerTimestamp);
    }

    [PunRPC]
    private void RPC_StartGame(int _hostLatency, int _sentTimestamp)
    {
        difficulty = 1f;
        if (!PhotonNetwork.IsMasterClient)
            multPEffect.CompensateForLag(_hostLatency, _sentTimestamp);

        HUD_startGame.SetActive(false);
        startGameButton.SetActive(false);
        HUD_ingame.SetActive(true);

        multPEffect.SetStatus(true);

        started = true;
        this.enabled = true;
    }

    public void OnDinoDied()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int _dinosAlive = 0;
        foreach (MultiplayerDino _d in dinos)
            if (!_d.dead) _dinosAlive++;

        if (_dinosAlive == 1)
            EndGame();
    }


    private void EndGame()
    {
        pv.RPC("RPC_EndGame", RpcTarget.All);
    }


    [PunRPC]
    private void RPC_EndGame()
    {
        if (!started) return;

        started = false;
        this.enabled = false;
        difficulty = 0;
        multPEffect.SetStatus(false);

        UpdateLeaderboardStats();
        string _txt = $"GAME OVER!\n";
        if (!myMultiplayerDino.dead)
            _txt += $"YOU WON!";
        else
            _txt += $"YOU LOST :(";
        int _intScore = (int)myMultiplayerDino.GetScore();
        string _scoreTxt = $"00000";
        _scoreTxt = _scoreTxt.Remove(0, _intScore.ToString().Length);
        _txt += $"\n\nSCORE: {_scoreTxt}{_intScore}";

        txt_endgame.text = _txt;
        HUD_ingame.SetActive(true);
        HUD_paused.SetActive(false);
        HUD_gameover.SetActive(true);

        if (!PhotonNetwork.IsMasterClient)
            return;

        // Reviver caso o vencedor morra depois do jogo acabar
        // pode acontecer com muito lag
        foreach (MultiplayerDino _d in dinos)
        {
            if (!_d.dead) _d.ReviveWinner();
            break;
        } 
    }

    public void ChangeTextToWinner()
    {
        string _txt = $"GAME OVER!\n";
        _txt += $"YOU WON!";
        int _intScore = (int)myMultiplayerDino.GetScore();
        string _scoreTxt = $"00000";
        _scoreTxt = _scoreTxt.Remove(0, _intScore.ToString().Length);
        _txt += $"\n\nSCORE: {_scoreTxt}{_intScore}";

        txt_endgame.text = _txt;
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
