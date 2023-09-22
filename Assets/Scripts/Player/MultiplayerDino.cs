using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerDino : Dino
{
    [Header("Multiplayer stuff")]
    [SerializeField] private TextMesh txt_nickname;
    [SerializeField] private PhotonView pv;

    protected override void Start()
    {
        base.Start();

        if (!pv.IsMine)
        {
            DisableEverything();
            return;
        }
        
        this.enabled = false;
        //pv.RPC("ResetDino", RpcTarget.All);
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
        if (pv.isMine) return;

        MultiplayerManager.instance.UpdateDinoList();
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
