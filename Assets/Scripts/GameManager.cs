using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ParallaxEffect pEffect;
    [SerializeField] private GameObject HUD_ingame;
    [SerializeField] private GameObject HUD_gameover;
    [SerializeField] private GameObject btn_startGame;
    Transform cam = null;

    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        RepositionCamera();

        HUD_ingame.SetActive(true);
        HUD_gameover.SetActive(false);
        btn_startGame.SetActive(true);
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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void Die()
    {
        pEffect.enabled = false;
        HUD_ingame.SetActive(false);
        HUD_gameover.SetActive(true);
    }

}
