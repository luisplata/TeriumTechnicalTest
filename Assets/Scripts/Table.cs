using System.Collections;
using Photon.Pun;
using SL;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Table : MonoBehaviourPunCallbacks
{
    [Range(0,3)]
    [SerializeField] private float time;
    [PunRPC]
    public void UpdateTable()
    {
        StartCoroutine(UpdateTableCoroutine());
    }

    private IEnumerator UpdateTableCoroutine()
    {
        yield return new WaitForSeconds(time);
        ServiceLocator.Instance.GetService<IDebug>().Log($"Debug: {PhotonNetwork.IsMasterClient}");
        foreach (var player in PhotonNetwork.PlayerList)
        {
            ServiceLocator.Instance.GetService<IDebug>().Log($"Player: {player.CustomProperties["NickName"]} Life: {player.CustomProperties["Life"]}");
        }
        ServiceLocator.Instance.GetService<IDebug>().Log($"Count: {PhotonNetwork.PlayerList.Length}");
    }
}