using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float vel = 10f;

    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private Transform[] grounds;
    [SerializeField] private Transform[] backgroundObjs;

    // ground
    private float groundSize = 38.0625f;
    // -50f og
    private float groundMaxScroll = -50f;
    private Vector2 groundNewPos = Vector2.zero;

    // obstacle
    private Vector2 placeRaycastPos = new Vector2(49f, 0);
    private float obstaclePlaceTimer = 0f;

    // background
    // -15f og
    private float backgroundMaxScroll = -20f;
    private float backgroundReplaceTimer = 0f;


    private void Awake()
    {
        this.enabled = false;
    }

    private void Update()
    {
        GroundParallax();
        ObstacleHandler();
    }

    private void LateUpdate()
    {
        BackgroundParallax();
    }

    private void BackgroundParallax()
    {
        // WAW
        backgroundReplaceTimer += 1f * Time.deltaTime;

        if (backgroundReplaceTimer < 1f)
            return;

        for (int i = 0; i < backgroundObjs.Length; i++)
        {
            if (backgroundObjs[i].position.x > backgroundMaxScroll)
                continue;

            float _xDiff = Mathf.Abs(backgroundObjs[i].position.x - backgroundMaxScroll);
            Vector2 _newPos = new Vector2(-backgroundMaxScroll - _xDiff, backgroundObjs[i].position.y);

            backgroundObjs[i].transform.position = _newPos;
        }


        backgroundReplaceTimer = 0f;
    }


    private void ObstacleHandler()
    {
        obstaclePlaceTimer -= 1f * Time.deltaTime;

        if (obstaclePlaceTimer > 0f)
            return;

        RaycastHit2D _hit = Physics2D.Raycast(placeRaycastPos, Vector2.down, Mathf.Infinity);

        if (!_hit)
            throw new Exception("sem chão??");

        // y = -2.93375
        Vector2 _newPos = new Vector2(_hit.point.x, -2.93375f);
        Instantiate(obstacles[UnityEngine.Random.Range(0, obstacles.Length)], 
            _newPos, Quaternion.identity, _hit.transform);

        obstaclePlaceTimer = UnityEngine.Random.Range(0.5f, 1.25f) + (GameManager.difficulty / 2);
    }


    private void GroundParallax()
    {
        for (int i = 0; i < grounds.Length; i++)
        {
            groundNewPos = grounds[i].position;
            groundNewPos.x -= vel * GameManager.difficulty * Time.deltaTime;

            if (groundNewPos.x <= groundMaxScroll)
            {
                int _nextPosIndex = i - 1;
                if (_nextPosIndex < 0) _nextPosIndex = grounds.Length - 1;

                groundNewPos = grounds[_nextPosIndex].position;
                groundNewPos.x += groundSize - 0.1f;
                groundNewPos.x -= vel * GameManager.difficulty * Time.deltaTime;

                for (int c = 0; c < grounds[i].childCount; c++)
                    if (grounds[i].GetChild(c).CompareTag("Obstacle"))
                        Destroy(grounds[i].GetChild(c).gameObject);

            }

            grounds[i].transform.position = groundNewPos;
        }
    }


    // One-frame methods

    public void SetStatus(bool _vl)
    {
        this.enabled = _vl;
    }

    public void DestroyAllObstacles()
    {
        foreach (Transform _g in grounds)
        {
            if (_g.transform.childCount == 0)
                continue;

            for (int c = 0; c < _g.childCount; c++)
                if (_g.GetChild(c).CompareTag("Obstacle"))
                    Destroy(_g.GetChild(c).gameObject);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(new Vector3(groundMaxScroll, transform.position.y, 0), 1f);
        //Gizmos.DrawWireSphere(new Vector3(-groundMaxScroll, transform.position.y, 0), 1f);
        Gizmos.DrawRay(new Vector3(groundMaxScroll, 0, 0), Vector3.down * 5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Vector3(backgroundMaxScroll, 0, 0), Vector3.down * 5f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(placeRaycastPos, Vector3.down * 5f);
    }
}
