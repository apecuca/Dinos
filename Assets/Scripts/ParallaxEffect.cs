using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float vel;
    [SerializeField] private Transform[] targets;
    [SerializeField] private float objSize = 21.4242f;
    [SerializeField] private float maxScroll = 0f;

    private void Update()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Vector2 _newPos = targets[i].position;
            //_newPos.x -= (vel * GameManager.difficulty) * Time.deltaTime;
            // multiplicar pela dificuldade quando for implementada
            _newPos.x -= vel * Time.deltaTime;

            if (_newPos.x < maxScroll)
            {
                int _nextPosIndex = i - 1;
                if (_nextPosIndex < 0) _nextPosIndex = targets.Length - 1;

                _newPos = targets[_nextPosIndex].position;
                _newPos.x += (objSize - 0.1f);
            }

            targets[i].position = _newPos;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(maxScroll, -5.35f, 0), 1f);
    }
}
