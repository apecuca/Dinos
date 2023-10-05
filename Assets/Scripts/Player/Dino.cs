using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino : MonoBehaviour
{
    [SerializeField] private bool immortal = false;
    [SerializeField] private bool pcInputs = false;

    protected Rigidbody2D rb;

    [SerializeField] private GameObject standingCol;
    [SerializeField] private GameObject crouchingCol;

    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected Transform feet;
    [SerializeField] protected Animator anim;
    [SerializeField] protected SpriteRenderer spr;
    [SerializeField] protected SO_Cosmetics cosmeticsData;
    public bool grounded { get; protected set; } = false;
    public bool crouching { get; protected set; } = false;

    private float jumpMaxTime = 0.155f; //0.175f
    private float jumpForce = 18.5f; //16.5f
    private float gravityScale = 8.5f; // 8.5f
    private float jumpTimer = 0f;
    public bool jumping { get; private set; } = false;
    private Vector2 ogPos;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        SkinInfo _cSkin = cosmeticsData.GetSkinInfo(
            SaveInfo.GetInstance().GetSelectedSkin());
        spr.sprite = _cSkin.preview;
        anim.runtimeAnimatorController = _cSkin.anim;
    }

    protected virtual void Update()
    {
        grounded = Physics2D.Raycast(feet.position, Vector2.down, 0.1f, groundLayer);

        if (pcInputs)
            PCInputs();
        GravityHandler();
        CollidersHandler();
        JumpHandler();

        AnimationsHandler();
    }

    protected void PCInputs()
    {
        JumpInteraction(Input.GetKey(KeyCode.UpArrow));
        SetCrouching(Input.GetKey(KeyCode.DownArrow));
    }


    protected void GravityHandler()
    {
        if (jumping)
        {
            rb.gravityScale = 0f;
            return;
        }
        
        if (!grounded)
        {
            if (crouching)
                rb.gravityScale = gravityScale * 5; // * 5
            else
                rb.gravityScale = gravityScale;
            return;
        }
        else
        {
            rb.gravityScale = gravityScale;
            return;
        }

    }


    private void PrepareJump()
    {
        if (!grounded) return;
        if (jumping) return;

        jumping = true;
        rb.gravityScale = 0f;
        SoundManager.instance.PlayJump();
    }

    protected void JumpHandler()
    {
        if (!jumping) return;

        jumpTimer += 1f * Time.deltaTime;

        if (jumpTimer >= jumpMaxTime)
        {
            CancelJump();
            return;
        }

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }


    protected void CollidersHandler()
    {
        if (!crouching)
        {
            if (!standingCol.activeInHierarchy)
            {
                crouchingCol.SetActive(false);
                standingCol.SetActive(true);
            }

            return;
        }

        // só chegará aqui caso esteja deitado
        if (grounded)
        {
            if (!crouchingCol.activeInHierarchy)
            {
                standingCol.SetActive(false);
                crouchingCol.SetActive(true);
            }

            return;
        }

        // só chegará aqui caso esteja deitado e não esteja no chão
        if (!standingCol.activeInHierarchy)
        {
            crouchingCol.SetActive(false);
            standingCol.SetActive(true);
        }
    }

    protected void AnimationsHandler()
    {
        anim.SetBool("Grounded", grounded);
        anim.SetBool("Crouching", crouching);
    }


    // One-frame methods

    public void SetCrouching(bool _vl)
    {
        crouching = _vl;

        if (jumping && _vl)
            CancelJump(true);
    }

    public void JumpInteraction(bool _vl)
    {
        if (_vl)
            PrepareJump();
        else
            CancelJump();
    }


    private void CancelJump(bool _force = false)
    {
        if (!jumping) return;

        jumpTimer = 0f;
        jumping = false;

        if (_force)
        {
            Vector3 _newVel = rb.velocity;
            _newVel.y = 0;
            rb.velocity = _newVel;
        }
            
    }

    protected virtual void Die()
    {
        GameManager.instance.Die();
        this.enabled = false;
        Vector3 _newVel = rb.velocity;
        _newVel.y = 0;
        rb.velocity = _newVel;
        rb.simulated = false;

        SoundManager.instance.PlayDied();

        anim.SetTrigger("Died");
    }

    public virtual void ResetDino()
    {
        transform.position = ogPos;
        this.enabled = true;
        Vector3 _newVel = rb.velocity;
        _newVel.y = 0;
        rb.velocity = _newVel;
        rb.simulated = true;

        crouching = false;
        jumping = false;
        grounded = false;

        anim.SetTrigger("Started");
    }

    public void SetOgPos()
    {
        ogPos = transform.position;
    }


    protected virtual void OnTriggerEnter2D(Collider2D _col)
    {
        if (immortal) return;
        if (!_col.CompareTag("Obstacle")) return;

        Die();
    }

}
