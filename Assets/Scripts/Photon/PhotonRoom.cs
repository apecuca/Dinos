using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    [Header("Assignables")]
    [SerializeField] private PhotonView pv;

    // scenes
    private int currentScene = 0;
    private int menuScene = 0;
    private int multiplayerScene = 2;

    // room settings
    public int maxPlayers { get; private set; } = 4;

    public static PhotonRoom room;

    private void Awake()
    {
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        LoadMultiplayer();
    }

    private void LoadMultiplayer()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.LoadLevel(multiplayerScene);
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;

        if (currentScene != multiplayerScene) return;
        // coisas para todos os players aqui
        StartCoroutine(LevelLoader());

        if (!PhotonNetwork.IsMasterClient) return;
        // coisas que o host faz ao entrar na sala aqui
    }

    private IEnumerator LevelLoader()
    {
        while (!PhotonNetwork.InRoom)
        {
            yield return new WaitForEndOfFrame();
        }

        SpawnPlayer();
    }


    public void SpawnPlayer()
    {
        Vector2 _newPos = new Vector2(0, -2.93375f);
        if (!PhotonNetwork.IsMasterClient)
            _newPos.x += Random.Range(-2f, 2f);
        GameObject _newDino = PhotonNetwork.Instantiate("MultiplayerDino",
            _newPos, Quaternion.identity);

        MultiplayerManager.instance.OnPlayerSpawned(_newDino.GetComponent<MultiplayerDino>());
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Destroy(PhotonRoom.room.gameObject);
        SceneManager.LoadScene(menuScene);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        //UpdatePlayerLists();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        //if (!otherPlayer.IsMasterClient)
        //    UpdatePlayerLists();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        //DisconnectPlayer();
    }

    public void DisconnectPlayer()
    {
        PhotonNetwork.OpCleanRpcBuffer(pv);
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }
}
