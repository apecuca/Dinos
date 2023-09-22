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
    }

    protected virtual void Update()
    {
        grounded = Physics2D.Raycast(feet.position, Vector2.down, 0.1f, groundLayer);

        PCInputs();
        GravityHandler();
        CollidersHandler();
        JumpHandler();

        AnimationsHandler();
    }

    protected void PCInputs()
    {
        if (!pcInputs) return;

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
        
        if (!grounded && crouching)
        {
            rb.gravityScale = gravityScale * 5; // * 4
            return;
        }

        if (!grounded)
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
            rb.velocity = Vector3.zero;
    }

    private void Die()
    {
        GameManager.instance.Die();
        this.enabled = false;
        rb.velocity = Vector3.zero;
        rb.simulated = false;

        anim.SetTrigger("Died");
    }

    public virtual void ResetDino()
    {
        transform.position = ogPos;
        this.enabled = true;
        rb.velocity = Vector3.zero;
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



    private void OnTriggerEnter2D(Collider2D _col)
    {
        if (immortal) return;
        if (!_col.CompareTag("Obstacle")) return;

        Die();
    }

}
