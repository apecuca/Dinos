using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;

    private void Update()
    {
        rb.velocity = new Vector2(-speed * GameManager.difficulty, 0);
    }
}
