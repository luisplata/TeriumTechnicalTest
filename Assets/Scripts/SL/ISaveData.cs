using Photon.Pun;

public interface ISaveData
{
    void SaveNickName(string nickName, PhotonView photonView);
    void SaveNickName(string nickName, bool isDebug);
    string GetNickName();
    void SaveLife(float life, PhotonView photonView);
}

