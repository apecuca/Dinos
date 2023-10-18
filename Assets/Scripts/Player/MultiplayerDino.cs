using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerDino : Dino
{
    private float moveSpeed = 10f;
    public bool freeMove { get; private set; } = false;
    private bool winner = false;
    public bool dead { get; private set; } = false;

    [Header("Multiplayer stuff")]
    [SerializeField] private TextMesh txt_nickname;
    //[SerializeField] private SpriteRenderer spr;
    [SerializeField] private PhotonView pv;

    private float x = 0;

    private string nickname = "";
    private float score = 0;

    public bool ready { get; private set; } = false;

    protected override void Start()
    {
        //base.Start();
        rb = GetComponent<Rigidbody2D>();
        txt_nickname.gameObject.SetActive(!SaveInfo.GetInstance().GetHideNick());

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

        if (!freeMove)
            return;

        FreeMoveHandler();
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

    // Chamado do MultiplayerManager
    public void OnGameEnded(int _position)
    {
        pv.RPC("RPC_OnGameEnded", RpcTarget.All, _position);
    }

    [PunRPC]
    private void RPC_OnGameEnded(int _position)
    {
        bool _alreadyDied = dead;

        if (_position == 0)
        {
            winner = true;
            dead = false;

            Color _c = spr.color;
            _c.a = 1f;
            spr.color = _c;
        }

        if (!pv.IsMine) return;

        if (_position < 3)
        {
            AddToScore(3f - _position);
            UpdateScore();

            if (!_alreadyDied)
                OnPlayerGameEnded();

            int _coinsToGain = (3 - _position) * 150;

            SaveInfo _instance = SaveInfo.GetInstance();
            _instance.AddCoins(_coinsToGain);
            _instance.Salvar();
        }

        MultiplayerManager.instance.UpdateEndgameText(_position);
    }


    
    // coisas como aplicar highscore e ganhar coins
    // rodar quando ganhar ou perder
    protected void OnPlayerGameEnded()
    {
        if (score > SaveInfo.GetInstance().GetHighscore())
            SaveInfo.GetInstance().SetHighscore((int)score);
        SaveInfo.GetInstance().AddCoins((int)score);

        SaveInfo.GetInstance().Salvar();
    }

    #endregion

    #region DURING GAME

    private void FreeMoveHandler()
    {
        //float _x = Input.GetAxisRaw("Horizontal");
        if (pcInputs)
            x = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        // FLIP AQUI
        if (x == 0)
            return;

        if (x != spr.transform.localScale.x)
            FlipDino(x);
    }

    public void FlipDino(float _dir)
    {
        pv.RPC("RPC_FlipDino", RpcTarget.All, _dir);
    }

    [PunRPC]
    private void RPC_FlipDino(float _x)
    {
        spr.transform.localScale = new Vector3(_x, 1, 1);
    }



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

        OnPlayerGameEnded();
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
        
        pv.RPC("RPC_UpdateCosmetics", RpcTarget.All,
            nickname, SaveInfo.GetInstance().GetSelectedSkin());
    }

    [PunRPC]
    public void RPC_UpdateCosmetics(string _nickname, int _skinID)
    {
        nickname = _nickname;
        txt_nickname.text = $"{_nickname}";

        SkinInfo _skin = cosmeticsData.GetSkinInfo(_skinID);
        spr.sprite = _skin.preview;
        anim.runtimeAnimatorController = _skin.anim;
        anim.SetTrigger("Started");
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

        Camera.main.GetComponent<MultiplayerCamera>().SetFollow(true, this.transform);
        SetFreeMove(true);

        score = 0;
        dead = false;
        winner = false;
        ready = false;

        UpdateScore();
        UpdateReady();
    }

    public void SetFreeMove(bool _vl)
    {
        pv.RPC("RPC_SetFreeMove", RpcTarget.All, _vl);
    }

    [PunRPC]
    private void RPC_SetFreeMove(bool _vl)
    {
        if (!pv.IsMine)
            return;
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        freeMove = _vl;
        if (_vl)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation & RigidbodyConstraints2D.FreezePositionX;
            FlipDino(1);
        }

            
    }

    public void SetX(float _vl)
    {
        x = _vl;
    }


    protected override void OnTriggerEnter2D(Collider2D _col)
    {
        if (winner) return;
        if (dead) return;
        if (!pv.IsMine) return;

        base.OnTriggerEnter2D(_col);
    }
}
