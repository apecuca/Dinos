using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float score = 0f;
    private bool started = false;
    private bool paused = false;

    [SerializeField] private Dino dino;
    [SerializeField] private ParallaxEffect pEffect;

    [Header("HUDs")]
    [SerializeField] private GameObject HUD_paused;
    [SerializeField] private GameObject HUD_ingame;
    [SerializeField] private GameObject HUD_gameover;
    [SerializeField] private GameObject HUD_startGame;

    [Header("Elements")]
    [SerializeField] private Text lb_currentScore;
    
    Transform cam = null;
    private float increaseDiffTimer = 0f;
    [SerializeField] private float maxDifficulty = 20f;

    public static float difficulty { get; private set; } = 1f;
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
        HUD_startGame.SetActive(true);
        HUD_paused.SetActive(false);

        PauseUnpause(false);
    }

    private void Update()
    {
        if (!started) return;

        DifficultyHandler();
        ScoreHandler();
    }

    private void ScoreHandler()
    {
        score += 4.5f * difficulty * Time.deltaTime;
        int _intScore = (int)score;

        string ohmahgah = $"00000";
        ohmahgah = ohmahgah.Remove(0, _intScore.ToString().Length);

        lb_currentScore.text = $"{ohmahgah}{_intScore}";    
        
    }

    private void DifficultyHandler()
    {
        if (difficulty >= maxDifficulty) return;

        increaseDiffTimer += 1f * Time.deltaTime;

        if (increaseDiffTimer < 1f) return;

        difficulty += 0.0055f;
        increaseDiffTimer = 0f;
    }


    private void RepositionCamera()
    {
        cam = Camera.main.transform;

        if (cam == null)
            throw new Exception("Erro ao validar a cam");

        Vector2 _screenBorders = Camera.main.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height)) - cam.position;
        Vector3 _newPos = Vector3.zero;
        _newPos.x += _screenBorders.x -5.5f;
        _newPos.z = -10f;

        cam.position = _newPos;
    }

    public void StartGame()
    {
        increaseDiffTimer = -4f;
        difficulty = 1;

        HUD_startGame.SetActive(false);

        pEffect.SetStatus(true);

        score = 0f;

        started = true;
    }

    public void RestartGame()
    {
        pEffect.DestroyAllObstacles();

        HUD_ingame.SetActive(true);
        HUD_gameover.SetActive(false);
        HUD_startGame.SetActive(false);
        HUD_paused.SetActive(false);

        PauseUnpause(false);
        dino.ResetDino();
        StartGame();
    }


    public void Die()
    {
        started = false;

        pEffect.SetStatus(false);
        HUD_ingame.SetActive(false);
        HUD_gameover.SetActive(true);
        difficulty = 0;
    }

    public void PauseUnpause(bool _vl)
    {
        paused = _vl;

        HUD_paused.SetActive(_vl);

        Time.timeScale = Convert.ToInt32(!_vl);
    }

}
