using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class MultiplayerParallaxEffect : ParallaxEffect
{
    [Header("Multiplayer stuff")]
    [SerializeField] private PhotonView pv;

    private float backgroundRightScroll = 30;

    protected override void Awake()
    {
        base.Awake();
        //-20 <
        // 30 >
    }

    protected override void Update()
    {
        //base.Update();
        GroundParallax();

        if (!PhotonNetwork.IsMasterClient)
            return;

        ObstacleHandler();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected override void BackgroundParallax()
    {
        //base.BackgroundParallax();
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
            Vector2 _newPos = new Vector2(backgroundRightScroll - _xDiff, backgroundObjs[i].position.y);

            backgroundObjs[i].transform.position = _newPos;
        }


        backgroundReplaceTimer = 0f;
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

    protected override IEnumerator SpawnObstacle(RaycastHit2D _hit)
    {
        // y = -2.93375
        Vector2 _newPos = new Vector2(_hit.point.x, -2.93375f);
        int _chosenObstID = UnityEngine.Random.Range(0, obstacles.Length);
        float _timeToWait = (1.5f * GameManager.difficulty) - lastObstTimer;
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

        obstaclePlaceTimer = UnityEngine.Random.Range(0.55f, 1.25f) * GameManager.difficulty;
        if (_chosenObstID >= 6)
            obstaclePlaceTimer -= _timeToWait;

        lastObstTimer = obstaclePlaceTimer;
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

    public Transform[] GetGrounds()
    {
        return grounds;
    }

    /*
    protected override void OnDrawGizmos()
    {
        backgroundMaxScroll = -20;
        base.OnDrawGizmos();
        Gizmos.DrawRay(new Vector3(backgroundRightScroll, 0, 0), Vector3.down * 5f);
    }
    */
}
