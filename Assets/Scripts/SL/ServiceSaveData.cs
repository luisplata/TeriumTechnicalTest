using Photon.Pun;
using SL;
using UnityEngine;

public class ServiceSaveData : ISaveData
{
    public void SaveNickName(string nickName, float life, PhotonView photonView)
    {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
        {
            { "NickName", nickName },
            { "Life", life },
            { "Ready", false }
        };
        photonView.Owner.SetCustomProperties(hash);
        ServiceLocator.Instance.GetService<IDebug>().Log("Nick Name Saved");
    }


    public void SaveNickName(string nickName, bool isDebug)
    {
        PlayerPrefs.SetString("NickName", nickName);
        ServiceLocator.Instance.GetService<IDebug>().Log("Nick Name Saved");
        //ServiceLocator.Instance.GetService<IPhotonRPC>().UpdateTable();
    }

    public string GetNickName()
    {
        return PlayerPrefs.GetString("NickName");
    }

    public string GetNickName(PhotonView photonView)
    {
        return photonView.Owner.CustomProperties["NickName"].ToString();
        
    }

    public string GetNickName(bool isDebug)
    {
        return PlayerPrefs.GetString("NickName");
    }
}