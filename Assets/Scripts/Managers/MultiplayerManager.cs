using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : GameManager
{
    [Header("Multiplayer stuff")]
    [SerializeField] private MultiplayerDino myMultiplayerDino;
    [SerializeField] private RectTransform leaderboard;
    [SerializeField] private PhotonView pv;
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
        HUD_startGame.SetActive(false);
        HUD_paused.SetActive(false);

        this.enabled = false;
    }

    protected override void Update()
    {
        //base.Update();
    }

    public void OnMyDinoSpawned(MultiplayerDino _dino)
    {
        myMultiplayerDino = _dino;
    }

    public void UpdateDinoList()
    {
        pv.RPC("RPC_UpdateDinoList", RpcTarget.All);
    }


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

        UpdateLeaderboardCount();
    }

    public void UpdateLeaderboardCount()
    {
        if (!PhotonNetwork.IsConnected) return;
        if (!PhotonNetwork.InRoom) return;
        if (leaderboard == null) return;

        for (int i = 0; i < leaderboard.childCount; i++)
        {
            if (i < dinos.Count)
            {
                leaderboard.GetChild(i).gameObject.SetActive(true);
                continue;
            }

            leaderboard.GetChild(i).gameObject.SetActive(false);
        }
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
