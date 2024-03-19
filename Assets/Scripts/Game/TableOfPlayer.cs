using System;
using Photon.Pun;
using SL;
using UnityEngine;

public class TableOfPlayer : MonoBehaviourPunCallbacks, IPhotonRPC
{
    [SerializeField] private Table table;
    [SerializeField] private TableUi tableUi;

    private void Awake()
    {
        
        ServiceLocator.Instance.RegisterService<IPhotonRPC>(this);
    }
    
    private void OnDestroy()
    {
        ServiceLocator.Instance.RemoveService<IPhotonRPC>();
    }

    public void UpdateTable()
    {
        //photonView.RPC(nameof(table.UpdateTable), RpcTarget.AllBuffered);
        photonView.RPC(nameof(tableUi.UpdateTableUi), RpcTarget.AllBuffered);
    }
}