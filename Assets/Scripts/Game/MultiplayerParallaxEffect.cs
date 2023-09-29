using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class MultiplayerParallaxEffect : ParallaxEffect
{
    [Header("Multiplayer stuff")]
    [SerializeField] private PhotonView pv;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        //base.Update();
        GroundParallax();

        if (!PhotonNetwork.IsMasterClient)
            return;

        ObstacleHandler();
    }

    public void CompensateForLag(int _hostLatency, int _sentTimestamp)
    {
        // _roundTripTime is in ms
        float _timeBeforeExecution = ((float)(PhotonNetwork.ServerTimestamp - _sentTimestamp))/100;
        _timeBeforeExecution += _hostLatency / 100;

        if (grounds.Length <= 0) return;

        for (int i = 0; i < grounds.Length; i++)
        {
            groundNewPos = grounds[i].position;
            groundNewPos.x -= vel * GameManager.difficulty * _timeBeforeExecution;
        }
    }

    /*
    protected override void ObstacleHandler()
    {
        //base.ObstacleHandler();
        if (obstacles.Length <= 0) return;

        obstaclePlaceTimer -= 1f * Time.deltaTime;

        if (obstaclePlaceTimer > 0f)
            return;

        RaycastHit2D _hit = Physics2D.Raycast(placeRaycastPos, Vector2.down, Mathf.Infinity);

        if (!_hit)
            throw new Exception("sem chão??");

        spawningObstacle = true;
        StartCoroutine(SpawnObstacle(_hit));
    }
    */

    protected override IEnumerator SpawnObstacle(RaycastHit2D _hit)
    {
        Vector2 _newPos = new Vector2(_hit.point.x, -2.93375f);
        int _chosenObstID = UnityEngine.Random.Range(0, obstacles.Length);
        float _timeToWait = 0.75f * GameManager.difficulty;
        int _hitGroundID = -1;

        if (_chosenObstID >= 6)
            yield return new WaitForSeconds(_timeToWait);

        for (int i = 0; i < grounds.Length; i++)
        {
            if (grounds[i] != _hit.transform)
                continue;

            _hitGroundID = i;
            i = grounds.Length;
        }

        pv.RPC("RPC_SpawnObstacle", RpcTarget.All, _chosenObstID, _newPos, _hitGroundID);

        obstaclePlaceTimer = UnityEngine.Random.Range(0.75f, 1.75f) * GameManager.difficulty;
        if (_chosenObstID >= 6)
            obstaclePlaceTimer -= _timeToWait / 2;
        spawningObstacle = false;
    }


    [PunRPC]
    protected void RPC_SpawnObstacle(int _obstID, Vector2 _pos, int _groundID)
    {
        if (_groundID == -1)
        {
            print("Erro no ground caralho");
            return;
        }

        Instantiate(obstacles[_obstID],
            _pos, Quaternion.identity, grounds[_groundID]);
        //
    }

}
