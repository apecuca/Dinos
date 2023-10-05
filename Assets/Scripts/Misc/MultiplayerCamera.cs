using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerCamera : MonoBehaviour
{
    private float followDamp = 10;
    private bool follow = false;
    private Transform followTarget = null;

    private void Update()
    {
        FollowTargetHandler();
    }

    private void FollowTargetHandler()
    {
        if (!follow) return;
        if (followTarget == null) return;

        Vector2 _screenBorders = Camera.main.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height)) - transform.position;
        Vector3 _newPos = followTarget.position;
        _newPos.z = transform.position.z;
        _newPos.y = transform.position.y;
        _newPos.x += _screenBorders.x - 5.5f;

        transform.position = Vector3.Lerp(transform.position, _newPos, followDamp * Time.deltaTime);
    }


    public void SetFollow(bool _vl, Transform _target)
    {
        if (!_vl)
        {
            Vector2 _screenBorders = Camera.main.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height)) - transform.position;
            Vector3 _newPos = followTarget.position;
            _newPos.z = transform.position.z;
            _newPos.y = transform.position.y;
            _newPos.x += _screenBorders.x - 5.5f;

            transform.position = _newPos;
        }

        follow = _vl;
        if (followTarget == null)
            followTarget = _target;
        this.enabled = _vl;

    }

}
