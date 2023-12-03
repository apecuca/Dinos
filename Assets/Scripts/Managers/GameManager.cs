using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float score = 0f;
    protected int highscore = 0;
    protected bool started = false;
    private bool paused = false;

    [SerializeField] private Dino myDino;
    [SerializeField] private ParallaxEffect pEffect;

    [Header("HUDs")]
    [SerializeField] protected GameObject HUD_paused;
    [SerializeField] protected GameObject HUD_ingame;
    [SerializeField] protected GameObject HUD_gameover;
    [SerializeField] protected GameObject HUD_startGame;

    [Header("Elements")]
    [SerializeField] private Text lb_currentScore;
    [SerializeField] private Text lb_highscore;
    [SerializeField] private GameObject[] mobileElements;
    
    protected Transform cam = null;
    protected float increaseDiffTimer = 0f;
    protected float maxDifficulty = 5f;
    protected float scorePerFrame = 4.5f;
    protected float difficultyPerSec = 0.015f; // 0.0065 // 0.0085

    private float nextMark = 100;

    public static float difficulty { get; protected set; } = 0f;
    public static GameManager instance { get; private set; }

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this;
    }

    protected virtual void Start()
    {
        RepositionDino();

        HUD_ingame.SetActive(false);
        HUD_gameover.SetActive(false);
        HUD_startGame.SetActive(true);
        HUD_paused.SetActive(false);

        PauseUnpause(false);

        score = 0f;
        difficulty = 0f;
        highscore = 0;
        nextMark = 100;

        if (SaveGame.TemSave())
            highscore = SaveInfo.GetInstance().GetHighscore();
        ScoreHandler();
    }

    protected virtual void Update()
    {
        if (!started) return;

        DifficultyHandler();
        ScoreHandler();
    }

    protected virtual void ScoreHandler()
    {
        score += scorePerFrame * difficulty * Time.deltaTime;
        int _intScore = (int)score;
        if (_intScore > highscore)
            highscore = _intScore;

        string _scoreTxt = $"00000";
        _scoreTxt = _scoreTxt.Remove(0, _intScore.ToString().Length);
        string _highscoreTxt = $"00000";
        _highscoreTxt = _highscoreTxt.Remove(0, highscore.ToString().Length);

        lb_currentScore.text = $"{_scoreTxt}{_intScore}";
        if (lb_currentScore != null)
            lb_highscore.text = $"{_highscoreTxt}{highscore}";

        // MARCO DE SCORE
        if (score >= nextMark)
        {
            nextMark *= 2;
            SoundManager.instance.PlayVictory();
        }

    }

    protected virtual void DifficultyHandler()
    {
        if (difficulty >= maxDifficulty) return;

        increaseDiffTimer += 1f * Time.deltaTime;

        if (increaseDiffTimer < 1f) return;

        difficulty += difficultyPerSec;
        increaseDiffTimer = 0f;
    }

    // One-frame methods

    protected virtual void RepositionDino()
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


    // GAME STUFF

    public virtual void StartGame()
    {
        increaseDiffTimer = -4f;
        difficulty = 1;

        HUD_startGame.SetActive(false);
        HUD_ingame.SetActive(true);

        pEffect.SetStatus(true);

        score = 0f;
        nextMark = 100;

        started = true;

        myDino.ResetDino();
    }

    public virtual void RestartGame()
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

        SaveInfo _saveInstance = SaveInfo.GetInstance();
        if (highscore > _saveInstance.GetHighscore())
            _saveInstance.SetHighscore(highscore);
        _saveInstance.AddCoins((int)(score / 5));
        _saveInstance.Salvar();
    }

    public virtual void PauseUnpause(bool _vl)
    {
        paused = _vl;

        HUD_paused.SetActive(paused);

        Time.timeScale = Convert.ToInt32(!paused);
    }

    public virtual void GOTO_menu()
    {
        PauseUnpause(false);
        SceneManager.LoadScene(0);
    }

    // INPUTS

    public virtual void BtnJump(bool _vl)
    {
        myDino.JumpInteraction(_vl);
    }

    public virtual void BtnCrouch(bool _vl)
    {
        myDino.SetCrouching(_vl);
    }

    public void DisableMobileUIElements()
    {
        for (int i = 0; i < mobileElements.Length; i++)
        {
            mobileElements[i].SetActive(false);
        }
    }
}
