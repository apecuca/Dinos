using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private bool started = false;

    [SerializeField] private float vel;
    [SerializeField] private Transform[] targets;
    [SerializeField] private float objSize = 21.4242f;
    [SerializeField] private float maxScroll = 0f;

    private void Update()
    {
        if (!started) return;

        for (int i = 0; i < targets.Length; i++)
        {
            Vector2 _newPos = targets[i].position;

            // fazer a troca antes de aplicar velocidade para evitar posição errada
            if (_newPos.x < maxScroll)
            {
                int _nextPosIndex = i - 1;
                if (_nextPosIndex < 0) _nextPosIndex = targets.Length - 1;

                _newPos = targets[_nextPosIndex].position;
                _newPos.x += (objSize - 0.1f);
            }

            //_newPos.x -= (vel * GameManager.difficulty) * Time.deltaTime;
            // multiplicar pela dificuldade quando for implementada
            _newPos.x -= vel * Time.deltaTime;

            targets[i].position = _newPos;
        }

    }

    public void SetStatus(bool _vl)
    {
        started = _vl;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(maxScroll, transform.position.y, 0), 1f);
        Gizmos.DrawWireSphere(new Vector3(-maxScroll, transform.position.y, 0), 1f);
    }
}
