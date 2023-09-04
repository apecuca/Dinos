using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ParallaxEffect pEffect;
    [SerializeField] private GameObject btn_startGame;
    Transform cam = null;

    private void Start()
    {
        RepositionCamera();
    }

    private void RepositionCamera()
    {
        cam = Camera.main.transform;

        if (cam == null)
            throw new Exception("Erro ao validar a cam");

        Vector2 _screenBorders = Camera.main.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height)) - cam.position;
        Vector3 _newPos = Vector3.zero;
        _newPos.x += _screenBorders.x - 5f;
        _newPos.z = -10f;

        cam.position = _newPos;
    }

    public void StartGame()
    {
        if (pEffect != null)
            pEffect.SetStatus(true);

        btn_startGame.SetActive(false);
    }


}
