using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float vel = 10f;

    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private Rigidbody2D[] grounds;
    private float groundSize = 38.0625f;
    private float obstacleMaxOffset = 8.5f;
    //5.5f eles ficam colados
    private float groundMaxScroll = -30f;

    private int nextGroundToTP = 0;
    private bool generatedDoubleObstacles = false;

    private void Awake()
    {
        foreach (Rigidbody2D _g in grounds)
        {
            if (_g.transform.childCount == 0)
                continue;

            Destroy(_g.transform.GetChild(0).gameObject);
        }

        this.enabled = false;
    }

    private void LateUpdate()
    {
        GroundParallax();
    }

    private void GroundParallax()
    {
        if (grounds[nextGroundToTP].position.x < groundMaxScroll)
        {
            ResetObstacle(nextGroundToTP);

            int _nextPosIndex = nextGroundToTP - 1;
            if (_nextPosIndex < 0) _nextPosIndex = grounds.Length - 1;

            Vector2 _newPos = grounds[_nextPosIndex].position;
            _newPos.x += groundSize - 0.1f;
            _newPos += grounds[nextGroundToTP].velocity * Time.deltaTime;
            _newPos.y = grounds[nextGroundToTP].position.y;
            grounds[nextGroundToTP].transform.position = _newPos;

            nextGroundToTP++;
            if (nextGroundToTP >= grounds.Length) nextGroundToTP = 0;
        }
    }


    public void UpdateVel()
    {
        for (int i = 0; i < grounds.Length; i++)
        {
            Vector2 _newVel = new Vector2((-vel) * GameManager.difficulty, 0);
            grounds[i].velocity = _newVel;
        }
    }

    private void ResetObstacle(int _gIndex)
    {
        if (grounds[_gIndex].transform.childCount != 0)
        {
            for (int i = 0; i < grounds[_gIndex].transform.childCount; i++)
            {
                Destroy(grounds[_gIndex].transform.GetChild(i).gameObject);
            }

        }

        if (generatedDoubleObstacles)
        {
            Vector2 _newPos = grounds[_gIndex].position;
            _newPos.x += UnityEngine.Random.Range(-obstacleMaxOffset, obstacleMaxOffset);
            _newPos.y = -2.93375f;
            GameObject _no = Instantiate(obstacles[UnityEngine.Random.Range(0, obstacles.Length)],
                 _newPos, Quaternion.identity);
            _no.transform.parent = grounds[_gIndex].transform;

            generatedDoubleObstacles = false;

            return;
        }
        // caso não for só 1, continua aqui

        // posições base
        Vector2 _pos1 = grounds[_gIndex].position;
        _pos1.x -= groundSize / 2;
        _pos1.x += groundSize / 3;
        _pos1.y = -2.93375f;
        Vector2 _pos2 = _pos1;
        _pos2.x += groundSize / 3;
        // offset
        float _added1 = UnityEngine.Random.Range(-obstacleMaxOffset, obstacleMaxOffset);
        float _added2 = UnityEngine.Random.Range(-obstacleMaxOffset, obstacleMaxOffset);
        if (Math.Abs((_pos2.x + _added2) - (_pos1.x + _added1)) < 6f)
        {
            _added1 = 5.5f;
            _added2 = -5.5f;
        }
        _pos1.x += _added1;
        _pos2.x += _added2;

        // o resto
        // no = new obstacle
        GameObject _no1 = Instantiate(obstacles[UnityEngine.Random.Range(0, obstacles.Length - 3)],
                _pos1, Quaternion.identity);
        GameObject _no2 = Instantiate(obstacles[UnityEngine.Random.Range(0, obstacles.Length - 3)],
                _pos2, Quaternion.identity);
        _no1.transform.parent = grounds[_gIndex].transform;
        _no2.transform.parent = grounds[_gIndex].transform;

        generatedDoubleObstacles = true;
    }

    public void DestroyAllObstacles()
    {
        foreach (Rigidbody2D _g in grounds)
        {
            if (_g.transform.childCount == 0)
                continue;

            //Destroy(_g.GetChild(0).gameObject);
            for (int i = 0; i < _g.transform.childCount; i++)
            {
                Destroy(_g.transform.GetChild(i).gameObject);
            }
        }
    }


    public void SetStatus(bool _vl)
    {
        //started = _vl;
        this.enabled = _vl;

        foreach (Rigidbody2D _g in grounds)
        {
            _g.simulated = _vl;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(groundMaxScroll, transform.position.y, 0), 1f);
        Gizmos.DrawWireSphere(new Vector3(-groundMaxScroll, transform.position.y, 0), 1f);
    }
}
