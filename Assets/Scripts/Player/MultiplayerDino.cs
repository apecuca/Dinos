using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerDino : Dino
{
    [Header("Multiplayer stuff")]
    [SerializeField] private PhotonView pv;

    protected override void Start()
    {
        base.Start();

        // tirar isso aqui dps
        ResetDino();

        if (!pv.IsMine)
        {
            DisableEverything();
            return;
        }

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

    private void DisableEverything()
    {
        this.enabled = false;

        rb.simulated = false;
    }

    //[PunRPC]
    public override void ResetDino()
    {
        //base.ResetDino();

        anim.SetTrigger("Started");
    }

}
