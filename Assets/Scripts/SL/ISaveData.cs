using Photon.Pun;

public interface ISaveData
{
    void SaveNickName(string nickName, float life, PhotonView photonView);
    void SaveNickName(string nickName, bool isDebug);
    string GetNickName();
}

