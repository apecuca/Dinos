using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerDino : Dino
{
    [Header("Multiplayer stuff")]
    [SerializeField] private TextMesh txt_nickname;
    [SerializeField] private PhotonView pv;

    private string nickname = "";

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
        //base.Update();
        grounded = Physics2D.Raycast(feet.position, Vector2.down, 0.1f, groundLayer);

        PCInputs();
        GravityHandler();
        CollidersHandler();
        JumpHandler();

        AnimationsHandler();
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
        pv.RPC("RPC_SetReady", RpcTarget.All, ready);
    }

    [PunRPC]
    private void RPC_SetReady(bool _vl)
    {
        ready = _vl;
        MultiplayerManager.instance.UpdateReadyCount();
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
