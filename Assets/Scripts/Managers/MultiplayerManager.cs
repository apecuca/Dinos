using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : GameManager
{
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

    public void OnPlayerSpawned(MultiplayerDino _dino)
    {
        myDino = _dino;
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
