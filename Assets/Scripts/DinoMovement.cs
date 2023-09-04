using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private GameObject standingCol;
    [SerializeField] private GameObject crouchingCol;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform feet;
    public bool grounded { get; private set; } = false;
    public bool crouching { get; private set; } = false;

    private float jumpMaxTime = 0.175f; //0.175f
    private float jumpForce = 16.5f;
    private float gravityScale = 8.5f; // 8.5f
    private float jumpTimer = 0f;
    public bool jumping { get; private set; } = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //gravityScale = rb.gravityScale;
    }

    private void Update()
    {
        grounded = Physics2D.Raycast(feet.position, Vector2.down, 0.1f, groundLayer);

        //InputHandler();
        GravityHandler();
        CollidersHandler();
        JumpHandler();
    }

    /*
    private void InputHandler()
    {
        // substituir esses Inputs por botões na tela? Talvez
        // ainda n sei
        if (Input.GetButtonDown("Fire1"))
            PrepareJump();

        if (Input.GetButtonUp("Fire1"))
            CancelJump();

        //crouching = Input.GetButton("Fire1");
        grounded = Physics2D.Raycast(feet.position, Vector2.down, 0.1f, groundLayer);
    }
    */

    private void GravityHandler()
    {
        if (jumping)
        {
            rb.gravityScale = 0f;
            return;
        }
        
        if (!grounded && crouching)
        {
            rb.gravityScale = gravityScale * 4;
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

    private void JumpHandler()
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


    private void CollidersHandler()
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
}
