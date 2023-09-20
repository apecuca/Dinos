using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float score = 0f;
    private int highscore = 0;
    private bool started = false;
    private bool paused = false;

    [SerializeField] private Dino myDino;
    [SerializeField] private ParallaxEffect pEffect;

    [Header("HUDs")]
    [SerializeField] private GameObject HUD_paused;
    [SerializeField] private GameObject HUD_ingame;
    [SerializeField] private GameObject HUD_gameover;
    [SerializeField] private GameObject HUD_startGame;

    [Header("Elements")]
    [SerializeField] private Text lb_currentScore;
    [SerializeField] private Text lb_highscore;
    
    Transform cam = null;
    private float increaseDiffTimer = 0f;
    private float maxDifficulty = 7.5f;

    public static float difficulty { get; private set; } = 0f;
    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        RepositionDino();

        HUD_ingame.SetActive(true);
        HUD_gameover.SetActive(false);
        HUD_startGame.SetActive(true);
        HUD_paused.SetActive(false);

        PauseUnpause(false);

        difficulty = 0f;
        highscore = 0;
        if (SaveGame.TemSave())
            highscore = SaveInfo.GetInstance().GetHighscore();
        ScoreHandler();
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
        if (_intScore > highscore)
            highscore = _intScore;

        string _scoreTxt = $"00000";
        _scoreTxt = _scoreTxt.Remove(0, _intScore.ToString().Length);
        string _highscoreTxt = $"00000";
        _highscoreTxt = _highscoreTxt.Remove(0, highscore.ToString().Length);

        lb_currentScore.text = $"{_scoreTxt}{_intScore}";
        lb_highscore.text = $"{_highscoreTxt}{highscore}";
        
    }

    private void DifficultyHandler()
    {
        if (difficulty >= maxDifficulty) return;

        increaseDiffTimer += 1f * Time.deltaTime;

        if (increaseDiffTimer < 1f) return;

        difficulty += 0.0065f;
        increaseDiffTimer = 0f;
    }

    // One-frame methods

    private void RepositionDino()
    {
        cam = Camera.main.transform;

        if (cam == null)
            throw new Exception("Erro ao validar a cam");

        Vector2 _screenBorders = Camera.main.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height)) - cam.position;
        Vector3 _newPos = Vector3.zero;
        _newPos.x = -_screenBorders.x + 5.5f;
        _newPos.y = myDino.transform.position.y;

        myDino.transform.position = _newPos;

        myDino.SetOgPos();
    }


    public void StartGame()
    {
        increaseDiffTimer = -4f;
        difficulty = 1;

        HUD_startGame.SetActive(false);

        pEffect.SetStatus(true);

        score = 0f;

        started = true;

        myDino.ResetDino();
    }

    public void RestartGame()
    {
        pEffect.DestroyAllObstacles();

        HUD_ingame.SetActive(true);
        HUD_gameover.SetActive(false);
        HUD_startGame.SetActive(false);
        HUD_paused.SetActive(false);

        PauseUnpause(false);
        myDino.ResetDino();
        StartGame();
    }


    public void Die()
    {
        started = false;

        pEffect.SetStatus(false);
        HUD_ingame.SetActive(false);
        HUD_gameover.SetActive(true);
        difficulty = 0;

        if (highscore > SaveInfo.GetInstance().GetHighscore())
            SaveInfo.GetInstance().SetHighscore(highscore);

        SaveInfo.GetInstance().Salvar();
    }

    public void PauseUnpause(bool _vl)
    {
        paused = _vl;

        HUD_paused.SetActive(paused);

        Time.timeScale = Convert.ToInt32(!paused);
    }

    public void GOTO_menu()
    {
        SceneManager.LoadScene(0);
    }
}
