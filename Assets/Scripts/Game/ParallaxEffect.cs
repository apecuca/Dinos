using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] protected float vel = 10f;

    [SerializeField] protected GameObject[] obstacles;
    [SerializeField] protected Transform[] grounds;
    [SerializeField] protected Transform[] backgroundObjs;
    [SerializeField] protected LayerMask groundLayer;

    // ground
    protected float groundSize = 38.0625f;
    // -50f og
    protected float groundMaxScroll = -50f;
    protected Vector2 groundNewPos = Vector2.zero;

    // obstacle
    protected Vector2 placeRaycastPos = new Vector2(49f, 0);
    protected float obstaclePlaceTimer = 0f;
    protected float lastObstTimer = 0f;
    protected bool spawningObstacle = false;

    // background
    // -15f og
    protected float backgroundMaxScroll = -20f;
    protected float backgroundReplaceTimer = 0f;


    protected virtual void Awake()
    {
        this.enabled = false;
    }

    protected virtual void Update()
    {
        GroundParallax();
        ObstacleHandler();
    }

    protected virtual void LateUpdate()
    {
        BackgroundParallax();
    }

    protected virtual void BackgroundParallax()
    {
        // WAW
        if (backgroundObjs.Length <= 0) return;

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


    protected virtual void ObstacleHandler()
    {
        if (obstacles.Length <= 0) return;
        if (spawningObstacle) return;

        obstaclePlaceTimer -= 1f * GameManager.difficulty * Time.deltaTime;

        if (obstaclePlaceTimer > 0f)
            return;

        RaycastHit2D _hit = Physics2D.Raycast(placeRaycastPos, Vector2.down, Mathf.Infinity, groundLayer);

        if (!_hit)
            throw new Exception("sem chão??");

        spawningObstacle = true;
        StartCoroutine(SpawnObstacle(_hit));
    }

    protected virtual IEnumerator SpawnObstacle(RaycastHit2D _hit)
    {
        // y = -2.93375
        Vector2 _newPos = new Vector2(_hit.point.x, -2.93375f);
        int _chosenObstID = UnityEngine.Random.Range(0, obstacles.Length);
        float _timeToWait = (0.2f * GameManager.difficulty);

        if (_chosenObstID >= 6)
            yield return new WaitForSeconds(_timeToWait);

        Instantiate(obstacles[_chosenObstID],
            _newPos, Quaternion.identity, _hit.transform);

        obstaclePlaceTimer = UnityEngine.Random.Range(0.75f, 1.25f) * GameManager.difficulty;
        //obstaclePlaceTimer = 0.55f * GameManager.difficulty;
        //obstaclePlaceTimer = 1.25f * GameManager.difficulty;
        if (_chosenObstID >= 6)
            obstaclePlaceTimer -= _timeToWait;

        lastObstTimer = obstaclePlaceTimer;
        spawningObstacle = false;
    }



    protected void GroundParallax()
    {
        if (grounds.Length <= 0) return;

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

    /*
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector3(groundMaxScroll, 0, 0), Vector3.down * 5f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(placeRaycastPos, Vector3.down * 5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Vector3(backgroundMaxScroll, 0, 0), Vector3.down * 5f);
    }
    */
}
