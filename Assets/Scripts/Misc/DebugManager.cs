using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Specialized;
using System.Diagnostics;

public class DebugManager : MonoBehaviour
{
    private bool debugging = false;

    [SerializeField] private Text txt_debug;
    [SerializeField] private GameObject debugObj;

    private float refreshRate = 5f;
    private float refreshTimer = 0f;

    private float fpsRefreshTimer = 0f;
    private int lastFPS = 0;
    private int frameCounter = 0;

    public static DebugManager instance { get; private set; }


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += UpdateCamera;
        ToggleDebug(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            ToggleDebug();

        // refresh e counters
        if (!debugging) return;
        refreshTimer -= 1f * Time.deltaTime;
        fpsRefreshTimer += 1f * Time.deltaTime;
        frameCounter++;
        if (fpsRefreshTimer >= 1f)
        {
            lastFPS = frameCounter;
            fpsRefreshTimer = 0;
            frameCounter = 0;
        }
        if (refreshTimer > 0) return;

        string _text = "";
        int _ping = 0;
        if (PhotonNetwork.IsConnected)
            _ping = PhotonNetwork.GetPing();

        _text += $"FPS: {lastFPS}";
        _text += $" | PING: {_ping}";

        txt_debug.text = _text;
        refreshTimer = 1f / refreshRate;
    }

    private void UpdateCamera(Scene current, Scene next)
    {
        Camera _cam = Camera.main;
        if (_cam != null)
            this.GetComponent<Canvas>().worldCamera = Camera.main;
    }


    private void ToggleDebug(int _force = -1)
    {
        debugging = !debugging;

        if (_force == 0)
            debugging = false;
        else if (_force == 1)
            debugging = true;

        debugObj.SetActive(debugging);
    }

}
