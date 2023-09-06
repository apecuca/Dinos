using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    //private bool started = false;

    [SerializeField] private float vel = 15f;
    [SerializeField] private Transform[] grounds;
    [SerializeField] private float groundSize = 20;
    [SerializeField] private float obstacleMaxOffset = 10f;
    [SerializeField] private float groundMaxScroll = -19f;

    private void Awake()
    {
        foreach (Transform _g in grounds)
        {
            if (_g.childCount == 0)
                continue;

            _g.GetChild(0).gameObject.SetActive(false);
        }

        this.enabled = false;
    }

    private void Update()
    {
        GroundParallax();
    }

    private void GroundParallax()
    {
        for (int i = 0; i < grounds.Length; i++)
        {
            Vector2 _newPos = grounds[i].position;

            // fazer a troca antes de aplicar velocidade para evitar posição errada
            if (_newPos.x < groundMaxScroll)
            {
                int _nextPosIndex = i - 1;
                if (_nextPosIndex < 0) _nextPosIndex = grounds.Length - 1;

                _newPos = grounds[_nextPosIndex].position;
                _newPos.x += groundSize - 0.1f;
                ResetObstacle(i);
            }

            //_newPos.x -= (vel * GameManager.difficulty) * Time.deltaTime;
            // multiplicar pela dificuldade quando for implementada
            _newPos.x -= vel * Time.deltaTime;

            grounds[i].position = _newPos;
        }
    }

    private void ResetObstacle(int _i)
    {
        if (grounds[_i].childCount == 0) return;

        Transform _obs = grounds[_i].GetChild(0);
        _obs.gameObject.SetActive(true);
        Vector2 _newPos = grounds[_i].position;
        _newPos.y = _obs.position.y;
        _newPos.x += UnityEngine.Random.Range(-obstacleMaxOffset, obstacleMaxOffset);
        _obs.position = _newPos;

        _obs.GetComponent<Obstacle>().SetType(UnityEngine.Random.Range(0, 4));
    }


    public void SetStatus(bool _vl)
    {
        //started = _vl;
        this.enabled = _vl;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(groundMaxScroll, transform.position.y, 0), 1f);
        Gizmos.DrawWireSphere(new Vector3(-groundMaxScroll, transform.position.y, 0), 1f);
    }
}
