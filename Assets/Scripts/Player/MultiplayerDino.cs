using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerDino : Dino
{
    public bool dead { get; private set; } = false;

    [Header("Multiplayer stuff")]
    [SerializeField] private TextMesh txt_nickname;
    [SerializeField] private PhotonView pv;

    private string nickname = "";
    public float score = 0;

    public bool ready { get; private set; } = false;

    protected override void Start()
    {
        base.Start();

        if (!pv.IsMine)
        {
            DisableEverything();
            return;
        }

        this.enabled = true;
        anim.SetTrigger("Started");
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnDestroy()
    {
        if (!PhotonNetwork.InRoom) return;
        if (pv.IsMine) return;

        MultiplayerManager.instance.UpdateDinoList();
    }

    #region PRE-POST GAME

    public void ToggleReady()
    {
        ready = !ready;
        UpdateReady();
    }

    public void UpdateReady()
    {
        pv.RPC("RPC_SetReady", RpcTarget.All, ready);
    }

    [PunRPC]
    private void RPC_SetReady(bool _vl)
    {
        ready = _vl;
        MultiplayerManager.instance.UpdateReadyCount();
    }

    #endregion

    #region DURING GAME

    public void AddToScore(float _vl)
    {
        score += _vl;
    }

    public void UpdateScore()
    {
        pv.RPC("RPC_UpdateScore", RpcTarget.All, score);
    }

    [PunRPC]
    private void RPC_UpdateScore(float _vl)
    {
        if (!pv.IsMine)
            score = _vl;
        MultiplayerManager.instance.UpdateLeaderboardStats();
    }

    public float GetScore()
    {
        return score;
    }


    #endregion

    #region COSMETICS

    public void UpdateCosmetics()
    {
        if (nickname == "")
        {
            if (SaveInfo.GetInstance().IsNicknameValid())
                nickname = SaveInfo.GetInstance().GetNickname();
            else
                nickname = $"Randola {Random.Range(0, 100)}";
        }
        
        pv.RPC("RPC_UpdateCosmetics", RpcTarget.All, nickname);
    }

    [PunRPC]
    public void RPC_UpdateCosmetics(string _nickname)
    {
        nickname = _nickname;
        txt_nickname.text = $"{_nickname}";
        if (!pv.IsMine)
            anim.SetTrigger("Started");
    }

    #endregion
    

    // MISC STUFF

    public string GetDinoNickname()
    {
        return nickname;
    }

    private void DisableEverything()
    {
        this.enabled = false;

        rb.simulated = false;
    }


    /*
    [PunRPC]
    public override void ResetDino()
    {
        //base.ResetDino();

        anim.SetTrigger("Started");
    }
    */

}
