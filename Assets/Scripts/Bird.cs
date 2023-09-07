using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    private void Update()
    {
        rb.velocity = new Vector2(-11f * GameManager.difficulty, 0);
    }
}
