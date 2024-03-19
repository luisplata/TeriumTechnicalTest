using System.Collections;
using Photon.Pun;
using SL;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviourPunCallbacks, IGameManager
{
    [SerializeField] private PhotonView playerPrefab;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private GameLoop gameLoop;
    [SerializeField] private GameObject canvasToWinner;
    [SerializeField] private TextMeshProUGUI textWinner;
    private Player _player;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<IGameManager>(this);
        canvasToWinner.SetActive(false);
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.RemoveService<IGameManager>();
    }

    public void AddPlayer(Player player)
    {
        _player = player;
    }

    public void RemovePlayer(Player player)
    {
        _player = null;
    }

    private void Start()
    {
        ServiceLocator.Instance.GetService<IDebug>().Log("GameManager Start");
        gameLoop.Config(this);
        if (PhotonNetwork.IsConnected)
        {
            ServiceLocator.Instance.GetService<IDebug>().Log("GameManager PhotonNetwork.IsConnected");
            if (!PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.JoinRandomOrCreateRoom();
            }
        }
    }
    
    public override void OnJoinedRoom()
    {
        ServiceLocator.Instance.GetService<IDebug>().Log("GameManager OnJoinedRoom");
        gameLoop.StartGame();
    }

    private IEnumerator InstantiataPlayer()
    {
        ServiceLocator.Instance.GetService<IDebug>().Log("GameManager PhotonNetwork.IsMasterClient");
        //get a random spawn point
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        //instantiate the player
        var player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.transform.position, spawnPoint.transform.rotation);
        _player = player.GetComponent<Player>();
        _player.OnPlayerTakeDamage += OnPlayerTakeDamage;
        yield return new WaitForSeconds(1);
    }

    private void OnPlayerTakeDamage()
    {
        
    }

    public int GetCountOfPlayers()
    {
        return PhotonNetwork.PlayerList.Length;
    }

    public int HowMuchPlayerAlive()
    {
        var count = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Life"))
            {
                if (player.CustomProperties["Life"] != null && (float)player.CustomProperties["Life"] > 0)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public void StopInput()
    {
        _player.StopInput();
    }

    public void StartInput()
    {
        _player.StartInput();
    }

    public void Cinematic()
    {
        playableDirector.Play();
    }

    public float GetTimeFromCinematic()
    {
        return (float)playableDirector.duration;
    }

    public void SpawnPlayers()
    {
        StartCoroutine(InstantiataPlayer());
    }

    public bool AllPlayersIsReady()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Ready"))
            {
                if (player.CustomProperties["Ready"] != null && (bool)player.CustomProperties["Ready"])
                {
                    continue;
                }
            }
            return false;
        }
        return true;
    }

    public string GetWinner()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Life"))
            {
                if (player.CustomProperties["Life"] != null && (float)player.CustomProperties["Life"] > 0)
                {
                    return player.CustomProperties["NickName"].ToString();
                }
            }
        }
        return "No Winner";
    }

    public void ShowWinner(string winner)
    {
        textWinner.text = winner;
        canvasToWinner.SetActive(true);
    }
}

public interface IGameManager
{
    void AddPlayer(Player player);
    void RemovePlayer(Player player);
}