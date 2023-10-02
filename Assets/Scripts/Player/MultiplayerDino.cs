using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerDino : Dino
{
    private bool winner = false;
    public bool dead { get; private set; } = false;

    [Header("Multiplayer stuff")]
    [SerializeField] private TextMesh txt_nickname;
    [SerializeField] private SpriteRenderer spr;
    [SerializeField] private PhotonView pv;

    private string nickname = "";
    private float score = 0;

    public bool ready { get; private set; } = false;

    protected override void Start()
    {
        base.Start();
        anim.SetTrigger("Started");

        if (!pv.IsMine)
        {
            DisableEverything();
            return;
        }

        this.enabled = true;
        spr.sortingOrder += 1;
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
        MultiplayerManager.instance.OnDinoDied();
    }

    #region PRE-POST GAME

    public void ToggleReady(int _force = -1)
    {
        ready = !ready;
        if (_force == 0)
            ready = false;
        else if (_force == 1)
            ready = true;
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

    public void ReviveWinner()
    {
        pv.RPC("RPC_ReviveWinner", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_ReviveWinner()
    {
        winner = true;
        dead = false;

        Color _c = spr.color;
        _c.a = 1f;
        spr.color = _c;
        
        if (pv.IsMine)
            MultiplayerManager.instance.ChangeTextToWinner();
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

    protected override void Die()
    {
        //base.Die();
        UpdateScore();
        SoundManager.instance.PlayDied();

        pv.RPC("RPC_Die", RpcTarget.All);

        if (!SaveGame.TemSave()) return;
        if (score < SaveInfo.GetInstance().GetHighscore()) return;
        SaveInfo.GetInstance().SetHighscore((int)score);
        SaveInfo.GetInstance().Salvar();
    }

    [PunRPC]
    private void RPC_Die()
    {
        Color _c = spr.color;
        _c.a = 0.5f;
        spr.color = _c;

        dead = true;

        MultiplayerManager.instance.OnDinoDied();
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
    }

    #endregion
    

    // MISC STUFF

    public string GetDinoNickname()
    {
        return nickname;
    }

    public float GetScore()
    {
        return score;
    }

    private void DisableEverything()
    {
        this.enabled = false;
        rb.simulated = false;
    }

    public override void ResetDino()
    {
        //base.ResetDino();

        pv.RPC("RPC_ResetDino", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_ResetDino()
    {
        Color _c = spr.color;
        _c.a = 1f;
        spr.color = _c;

        score = 0;

        dead = false;
        winner = false;
        ready = false;

        UpdateScore();
        UpdateReady();
    }


    protected override void OnTriggerEnter2D(Collider2D _col)
    {
        if (winner) return;
        if (dead) return;
        if (!pv.IsMine) return;


        base.OnTriggerEnter2D(_col);
    }
}
