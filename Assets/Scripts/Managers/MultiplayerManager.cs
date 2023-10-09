using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerManager : GameManager
{
    [Header("Multiplayer stuff")]
    [SerializeField] private GameObject btn_restart;
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
    private MultiplayerCamera multCam;

    private float updateScoreTimer = 0f;

    public static new MultiplayerManager instance { get; private set; }

    protected override void Awake()
    {
        //base.Awake();

        instance = this;
    }

    protected override void Start()
    {
        HUD_ingame.SetActive(true);
        HUD_gameover.SetActive(false);
        HUD_startGame.SetActive(true);
        HUD_paused.SetActive(false);

        startGameButton.SetActive(false);

        this.enabled = false;

        difficulty = 0;

        if (!PhotonNetwork.IsMasterClient)
            return;

        btn_restart.SetActive(true);
    }

    protected override void Update()
    {
        //base.Update();

        if (!started) return;

        DifficultyHandler();
        ScoreHandler();
        //UpdateLeaderboard();
    }

    #region AO ENTRAR/SAIR

    public void OnMyDinoSpawned(MultiplayerDino _dino)
    {
        myMultiplayerDino = _dino;
        myMultiplayerDino.SetFreeMove(true);
        //RepositionCamera();

        multCam = Camera.main.gameObject.GetComponent<MultiplayerCamera>();
        multCam.SetFollow(true, myMultiplayerDino.transform);

        ScoreHandler();
    }

    /*
    private void RepositionCamera()
    {
        cam = Camera.main.transform;

        if (cam == null)
            throw new Exception("Erro ao validar a cam");

        Vector2 _screenBorders = Camera.main.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height)) - cam.position;
        Vector3 _newPos = myMultiplayerDino.transform.position;
        _newPos.z = cam.position.z;
        _newPos.y = cam.position.y;
        _newPos.x += _screenBorders.x - 5.5f;

        cam.position = _newPos;
    }
    */

    public void OnDinoJoined()
    {
        pv.RPC("RPC_OnDinoJoined", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_OnDinoJoined()
    {
        // isso aqui roda em todos os GameManagers assim que um dino termina de nascer
        if (PhotonNetwork.IsMasterClient)
        {
            Transform[] grounds = multPEffect.GetGrounds();
            Vector3[] poss = new Vector3[grounds.Length];
            for (int i = 0; i < grounds.Length; i++)
                poss[i] = grounds[i].position;
            pv.RPC("RPC_UpdateGroundPositions", RpcTarget.Others, poss);
        }


        if (myMultiplayerDino == null) return;

        myMultiplayerDino.UpdateCosmetics();
        myMultiplayerDino.UpdateReady();
        UpdateDinoList();
    }

    [PunRPC]
    private void RPC_UpdateGroundPositions(Vector3[] positions)
    {
        Transform[] grounds = multPEffect.GetGrounds();
        for (int i = 0; i < grounds.Length; i++)
            grounds[i].position = positions[i];
    }


    public void UpdateDinoList()
    {
        pv.RPC("RPC_UpdateDinoList", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_UpdateDinoList()
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

    public void OnMasterSwitched()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            btn_restart.SetActive(true);
        }
        else
        {
            btn_restart.SetActive(false);
        }
    }



    #endregion

    #region LEADERBOARD

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

    protected override void DifficultyHandler()
    {
        //base.DifficultyHandler();
        if (difficulty >= maxDifficulty) return;

        increaseDiffTimer += 1f * Time.deltaTime;

        if (increaseDiffTimer < 1f) return;

        difficulty += difficultyPerSec;
        increaseDiffTimer = 0f;

        pv.RPC("RPC_UpdateDifficulty", RpcTarget.Others, difficulty, PhotonNetwork.GetPing(), PhotonNetwork.ServerTimestamp);
    }

    [PunRPC]
    private void RPC_UpdateDifficulty(float _diff, int _hostLatency, int _sentTimestamp)
    {
        difficulty = _diff;
        multPEffect.CompensateForLag(_hostLatency, _sentTimestamp);
    }


    public void ToggleReady()
    {
        myMultiplayerDino.ToggleReady();
    }

    public void UpdateReadyCount()
    {
        int _pReady = 0;
        foreach (MultiplayerDino _d in dinos)
            if (_d.ready) _pReady++;
        if (myMultiplayerDino.ready)
            txt_playersReady.color = Color.green;
        else
            txt_playersReady.color = Color.red;

        txt_playersReady.text = $"PLAYERS READY\n{_pReady}/{dinos.Count}";

        if (!PhotonNetwork.IsMasterClient)
            return;

        if (_pReady == dinos.Count &&
            _pReady >= 2)
            StartGame();
    }

    public override void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;

        pv.RPC("RPC_StartGame", RpcTarget.All, PhotonNetwork.GetPing(), PhotonNetwork.ServerTimestamp);
    }

    [PunRPC]
    private void RPC_StartGame(int _hostLatency, int _sentTimestamp)
    {
        myMultiplayerDino.SetFreeMove(false);
        multCam.SetFollow(false, null);

        increaseDiffTimer = -4f;
        difficulty = 1f;
        if (!PhotonNetwork.IsMasterClient)
            multPEffect.CompensateForLag(_hostLatency, _sentTimestamp);

        myMultiplayerDino.ToggleReady(0);

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
        int _lastDino = -1;
        for (int i = 0; i < dinos.Count; i++)
        {
            if (dinos[i].dead)
                continue;

            _dinosAlive++;
            _lastDino = i;
        }

        if (_dinosAlive == 1)
            EndGame(_lastDino);
    }


    private void EndGame(int _lastDinoID)
    {
        pv.RPC("RPC_EndGame", RpcTarget.All, _lastDinoID);
    }


    [PunRPC]
    private void RPC_EndGame(int _lastDino)
    {
        if (!started) return;

        // parar jogo
        started = false;
        this.enabled = false;
        difficulty = 0;
        multPEffect.SetStatus(false);

        // trocar as HUDS
        myMultiplayerDino.UpdateScore();
        UpdateEndgameText(-1);
        HUD_ingame.SetActive(true);
        HUD_paused.SetActive(false);
        HUD_gameover.SetActive(true);

        // lidar com vencedores e perdedores
        // somente host faz isso para maior precisão
        if (!PhotonNetwork.IsMasterClient) return;

        List<MultiplayerDino> _dinoPositions = new List<MultiplayerDino>(
            dinos.Where((v, i) => i != _lastDino));
        _dinoPositions.Sort((x, y) => x.GetScore().CompareTo(y.GetScore()));
        _dinoPositions.Insert(0, dinos[_lastDino]);
        for (int i = 0; i < _dinoPositions.Count; i++)
            _dinoPositions[i].OnGameEnded(i);
    }

    public void UpdateEndgameText(int _position)
    {
        if (_position < 0)
        {
            txt_endgame.text = "GAME OVER!\nCALCULANDO VENCEDORES...";
            return;
        }

        string _txt = $"GAME OVER!\n";
        _txt += $"VOCÊ FICOU EM {_position + 1}º LUGAR!\n";

        int _intScore = (int)myMultiplayerDino.GetScore();
        string _scoreTxt = $"00000";
        _scoreTxt = _scoreTxt.Remove(0, _intScore.ToString().Length);
        _txt += $"\nSCORE: {_scoreTxt}{_intScore}";

        int _coinsToGain = (3 - _position) * 150;
        _txt += $"\nCOINS: {(int)myMultiplayerDino.GetScore() + _coinsToGain}";

        txt_endgame.text = _txt;
    }


    public override void RestartGame()
    {
        //base.RestartGame();
        pv.RPC("RPC_RestartGame", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_RestartGame()
    {
        multPEffect.DestroyAllObstacles();
        myMultiplayerDino.ResetDino();
        if (myMultiplayerDino.GetComponent<PhotonView>().IsMine)
            multCam.SetFollow(true, myMultiplayerDino.transform);

        HUD_ingame.SetActive(true);
        HUD_gameover.SetActive(false);
        HUD_startGame.SetActive(true);

        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.CurrentRoom.IsOpen = true;
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
