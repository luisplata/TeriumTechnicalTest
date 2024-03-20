using System;
using Photon.Pun;
using SL;
using UnityEngine;

public class InitApp : MonoBehaviourPunCallbacks, IPhotonRPC
{
    [SerializeField] PhotonView photonView;
    [SerializeField] private Table table;
    [SerializeField] private RulesOfTitleRoom rulesOfTitleRoom;
    [SerializeField] private WeaponsConfiguration weaponsConfiguration;
    private void Awake()
    {
        rulesOfTitleRoom.GetCanvas().SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
        ServiceLocator.Instance.RegisterService<IPhotonRPC>(this);
        try
        {
            ServiceLocator.Instance.GetService<ISaveData>();
        }
        catch (Exception e)
        {
            ServiceLocator.Instance.RegisterService<ISaveData>(new ServiceSaveData());
        }
        
        try
        {
            ServiceLocator.Instance.GetService<IWeaponsFactory>();
        }
        catch (Exception e)
        {
            ServiceLocator.Instance.RegisterService<IWeaponsFactory>(new WeaponsFactory(Instantiate(weaponsConfiguration)));
        }
    }

    public void UpdateTable()
    {
        photonView.RPC(nameof(table.UpdateTable), RpcTarget.AllBuffered);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        //ServiceLocator.Instance.GetService<IDebug>().Log("Connected to master");
        rulesOfTitleRoom.GetCanvas().SetActive(true);
        ServiceLocator.Instance.GetService<ISoundAndMusic>().PlayMusicMenu();
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.RemoveService<IPhotonRPC>();
    }
}

public interface IPhotonRPC
{
    void UpdateTable();
}