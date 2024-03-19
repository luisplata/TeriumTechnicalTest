using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SL;
using UnityEngine;
using UnityEngine.InputSystem;

public class TableUi : MonoBehaviourPunCallbacks
{
    
    [Range(0,3)]
    [SerializeField] private float time = 0.5f;
    private Dictionary<string, PlayerUiCustom> playerUiCustoms = new();
    [SerializeField] private GameObject playerUiPrefab;
    [SerializeField] private GameObject tableUi;


    private void Start()
    {
        tableUi.SetActive(false);
    }

    [PunRPC]
    public void UpdateTableUi()
    {
        StartCoroutine(UpdateTableCoroutine());
    }
    
    public void OnShowHideTable(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            tableUi.SetActive(!tableUi.activeSelf);
        }
    }
    
    private IEnumerator UpdateTableCoroutine()
    {
        yield return new WaitForSeconds(time);
        
        ServiceLocator.Instance.GetService<IDebug>().Log($"Debug: {PhotonNetwork.PlayerList.Length}");
        foreach (var player in PhotonNetwork.PlayerList)
        {
            //player.CustomProperties["Life"]
            //player.CustomProperties["NickName"]
            ServiceLocator.Instance.GetService<IDebug>()
                .Log($"Player: {player.CustomProperties["NickName"]} Life: {player.CustomProperties["Life"]}");
            var playerNick = player.CustomProperties["NickName"].ToString();
            var playerLife = player.CustomProperties["Life"].ToString();
            if(playerNick == null || playerLife == null) continue;
            if (!playerUiCustoms.ContainsKey(playerNick))
            {
                var playerUiCustom = Instantiate(playerUiPrefab, tableUi.transform).GetComponent<PlayerUiCustom>();
                playerUiCustoms.Add(playerNick, playerUiCustom);
            }

            playerUiCustoms[playerNick].UpdatePlayerUi(playerNick, playerLife);
        }

        ServiceLocator.Instance.GetService<IDebug>().Log($"Count: {PhotonNetwork.PlayerList.Length}");
    }
}