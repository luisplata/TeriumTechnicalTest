using System;
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
        ServiceLocator.Instance.GetService<IGameManager>().AddPlayer(this);
        cinemachineComponentBase.gameObject.SetActive(photonView.IsMine);
        
        if (photonView.IsMine)
        {
            inputCustom.OnFire += OnFire;
            nickName = ServiceLocator.Instance.GetService<ISaveData>().GetNickName();
        }
        else
        {
            nickName = photonView.Owner.CustomProperties["NickName"].ToString();
        }
        
        ServiceLocator.Instance.GetService<ISaveData>().SaveNickName(nickName, photonView);
        ServiceLocator.Instance.GetService<ISaveData>().SaveLife(life, photonView);
        ServiceLocator.Instance.GetService<IDebug>().Log($"Data Saved {nickName} {life} {photonView.IsMine}");
    }

    private void OnFire()
    {
        ServiceLocator.Instance.GetService<IDebug>().Log($"OnFire {photonView.IsMine}");
        if (photonView.IsMine)
        {
            if (Physics.Raycast(pointShoot.transform.position, pointShoot.transform.forward, out var hit, 100f))
            {
                ServiceLocator.Instance.GetService<IDebug>().Log($"Hit {hit.transform.name}");
                if (hit.transform.CompareTag("Player"))
                {
                    ServiceLocator.Instance.GetService<IDebug>().Log($"Player {hit.transform.name}");
                    var target = hit.transform.GetComponent<PhotonView>();
                    photonView.RPC(nameof(TakeDamage), target.Owner, damage, target.Owner.CustomProperties["NickName"]);
                }
            }
        }
            
    }

    private void OnDestroy()
    {
        if (!photonView.IsMine) return;
        ServiceLocator.Instance.GetService<IGameManager>().RemovePlayer(this);
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
        ServiceLocator.Instance.GetService<IDebug>().Log($" {this.nickName} = {nickName} take damage {damage} photonView.IsMine {photonView.IsMine}");
        life -= damage;
        OnPlayerTakeDamage?.Invoke();
        if (life <= 0)
        {
            ServiceLocator.Instance.GetService<IDebug>().Log($"{nickName} is dead");
        }
        ServiceLocator.Instance.GetService<ISaveData>().SaveLife(life, photonView);
        ServiceLocator.Instance.GetService<IPhotonRPC>().UpdateTable();
    }

    public void StopInput()
    {
        if(photonView.IsMine)
            inputCustom.StopInput();
    }

    public void StartInput()
    {
        if(photonView.IsMine)
            inputCustom.StartInput();
    }

    public float GetLife()
    {
        return life;
    }
}