using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using SL;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviourPunCallbacks, IGameManager
{
    [SerializeField] private PhotonView playerPrefab;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private GameLoop gameLoop;
    private List<Player> players = new();

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<IGameManager>(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.RemoveService<IGameManager>();
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        players.Remove(player);
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
        yield return new WaitForSeconds(1);
        ServiceLocator.Instance.GetService<IDebug>().Log("GameManager PhotonNetwork.IsMasterClient");
        //get a random spawn point
        var spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        //instantiate the player
        var player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.transform.position, spawnPoint.transform.rotation);
        player.GetComponent<Player>().OnPlayerTakeDamage += OnPlayerTakeDamage;
    }

    private void OnPlayerTakeDamage()
    {
        
    }

    public int GetCountOfPlayers()
    {
        return players.Count;
    }

    public int HowMuchPlayerAlive()
    {
        var count = 0;
        foreach (var player in players)
        {
            if (player.GetLife() > 0)
            {
                count++;
            }
        }
        return count;
    }

    public void StopInput()
    {
        foreach (var player in players)
        {
            player.StopInput();
        }
    }

    public void StartInput()
    {
        foreach (var player in players)
        {
            player.StartInput();
        }
    }

    public void InstantatePlayer()
    {
        
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
}

public interface IGameManager
{
    void AddPlayer(Player player);
    void RemovePlayer(Player player);
}