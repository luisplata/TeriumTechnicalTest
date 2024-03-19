using Photon.Pun;
using SL;
using UnityEngine;

public class ServiceSaveData : ISaveData
{
    public void SaveNickName(string nickName, PhotonView photonView)
    {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("NickName", nickName);
        hash.Add("Life", 100f);
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

    public void SaveLife(float life, PhotonView photonView)
    {
        var hash = new ExitGames.Client.Photon.Hashtable { { "Life", life } };
        photonView.Owner.SetCustomProperties(hash);
        ServiceLocator.Instance.GetService<IDebug>().Log("Life Saved");
    }
}