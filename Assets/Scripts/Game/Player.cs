using System;
using System.Collections;
using Cinemachine;
using Photon.Pun;
using SL;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField] private CinemachineVirtualCamera cinemachineComponentBase;
    [SerializeField] private InputCustom inputCustom;
    [SerializeField] private GameObject pointShoot;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float speedRotation = 50f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private Rigidbody rb;

    private float life = 100f;
    private string nickName;
    
    public Action OnPlayerTakeDamage;
    
    // Start is called before the first frame update
    void Start()
    {
        cinemachineComponentBase.gameObject.SetActive(photonView.IsMine);
        
        if (photonView.IsMine)
        {
            inputCustom.OnFire += OnFire;
            nickName = ServiceLocator.Instance.GetService<ISaveData>().GetNickName();
            ServiceLocator.Instance.GetService<ISaveData>().SaveNickName(nickName, life, photonView);
            ServiceLocator.Instance.GetService<IDebug>().Log($"Data Saved {nickName} {life} {photonView.IsMine}");
            photonView.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                {"Ready", true}
            });
            ServiceLocator.Instance.GetService<IGameManager>().AddPlayer(this);
        }
        else
        {
            StartCoroutine(ReadData());
        }
    }

    private IEnumerator ReadData()
    {
        yield return new WaitForSeconds(0.5f);
        nickName = photonView.Owner.CustomProperties["NickName"].ToString();
        life = (float) photonView.Owner.CustomProperties["Life"];
        ServiceLocator.Instance.GetService<IDebug>().Log($"Data Read {nickName} {life} {photonView.IsMine}");
    }

    private void OnFire()
    {
        ServiceLocator.Instance.GetService<IDebug>().Log($"{nickName} OnFire {photonView.IsMine}");
        if (photonView.IsMine)
        {
            if (Physics.Raycast(pointShoot.transform.position, pointShoot.transform.forward, out var hit, 100f))
            {
                ServiceLocator.Instance.GetService<IDebug>().Log($"Hit {hit.transform.name}");
                if (hit.transform.CompareTag("Player"))
                {
                    var targetNick = hit.transform.GetComponent<Player>().nickName;
                    ServiceLocator.Instance.GetService<IDebug>().Log($"Player {hit.transform.name} and name of target {targetNick} and me is {nickName}");
                    var target = hit.transform.GetComponent<PhotonView>();
                    target.RPC(nameof(TakeDamage), RpcTarget.All, damage, targetNick);
                }
            }
        }
            
    }

    private void OnDestroy()
    {
        if (!photonView.IsMine) return;
        //Disconnect the player
        PhotonNetwork.Disconnect();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //Move forward or backward based on input with rigidbody
            rb.velocity = transform.forward * (inputCustom.Move * speed * Time.deltaTime);
            //transform.position += transform.forward * (inputCustom.Move * speed * Time.deltaTime);
            //Rotate based on input and speed
            transform.Rotate(Vector3.up, inputCustom.Rotate * speedRotation * Time.deltaTime);
            
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, string nickName)
    {
        var nickl = this.nickName;
        ServiceLocator.Instance.GetService<IDebug>().Log($"{nickName} take damage {damage} and me is {nickl}");
        if (nickl == nickName)
        {
            ServiceLocator.Instance.GetService<IDebug>().Log($" {this.nickName} = {nickName} take damage {damage} photonView.IsMine {photonView.IsMine}");
            life -= damage;
            OnPlayerTakeDamage?.Invoke();
            ServiceLocator.Instance.GetService<IDebug>().Log($" {this.nickName} take damage {damage} photonView.IsMine {photonView.IsMine}");
            if (life <= 0)
            {
                ServiceLocator.Instance.GetService<IDebug>().Log($" {this.nickName} is dead");
                photonView.RPC(nameof(Dead), RpcTarget.All);
            }
            photonView.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                {"Life", life}
            });
        }
        ServiceLocator.Instance.GetService<IPhotonRPC>().UpdateTable();
    }
    
    [PunRPC]
    public void Dead()
    {
        ServiceLocator.Instance.GetService<IDebug>().Log($" {this.nickName} Dead");
        if (photonView.IsMine)
        {
            ServiceLocator.Instance.GetService<IDebug>().Log($" {this.nickName} Dead and is mine");
            //ServiceLocator.Instance.GetService<IGameManager>().RemovePlayer(this);
            //PhotonNetwork.Destroy(gameObject);
        }
    }

    public void StopInput()
    {
        inputCustom.StopInput();
    }

    public void StartInput()
    {
        inputCustom.StartInput();
    }

    public float GetLife()
    {
        return life;
    }
}