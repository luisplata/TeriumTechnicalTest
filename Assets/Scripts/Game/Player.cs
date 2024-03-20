using System;
using System.Collections;
using Cinemachine;
using Photon.Pun;
using SL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField] private CinemachineVirtualCamera cinemachineComponentBase;
    [SerializeField] private InputCustom inputCustom;
    [SerializeField] private GameObject pointShoot;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float speedRotation = 50f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private string defaulWeaponId;
    [SerializeField] private Image blindImage; 
    [SerializeField] private GameObject canvasToInstaKill;
    [SerializeField] private TextMeshProUGUI countDownToDie;
    [SerializeField] private AudioClip takeDamageSound;

    private float life = 100f;
    private string nickName;
    
    public Action OnPlayerTakeDamage;
    private WeaponBase _weapon;

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
                {"Ready", true},
                {"Weapon", defaulWeaponId}
            });
            ServiceLocator.Instance.GetService<IGameManager>().AddPlayer(this);
            _weapon = ServiceLocator.Instance.GetService<IWeaponsFactory>().Create(defaulWeaponId);
            _weapon.transform.SetParent(weaponHolder.transform);
            _weapon.transform.localPosition = Vector3.zero;
        }
        else
        {
            StartCoroutine(ReadData());
        }
        canvasToInstaKill.SetActive(false);
    }

    private IEnumerator ReadData()
    {
        yield return new WaitForSeconds(0.5f);
        nickName = photonView.Owner.CustomProperties["NickName"].ToString();
        life = (float) photonView.Owner.CustomProperties["Life"];
        _weapon = ServiceLocator.Instance.GetService<IWeaponsFactory>().Create(photonView.Owner.CustomProperties["Weapon"].ToString());
        _weapon.transform.SetParent(weaponHolder.transform);
        _weapon.transform.localPosition = Vector3.zero;
        ServiceLocator.Instance.GetService<IDebug>().Log($"Data Read {nickName} {life} {photonView.IsMine}");
    }

    private void OnFire()
    {
        ServiceLocator.Instance.GetService<IDebug>().Log($"{nickName} OnFire {photonView.IsMine}");
        if (photonView.IsMine && _weapon.CanShoot())
        {
            _weapon.Shoot();
            if (Physics.Raycast(pointShoot.transform.position, pointShoot.transform.forward, out var hit, 100f))
            {
                ServiceLocator.Instance.GetService<IDebug>().Log($"Hit {hit.transform.name}");
                if (hit.transform.CompareTag("Player"))
                {
                    var targetNick = hit.transform.GetComponent<Player>().nickName;
                    ServiceLocator.Instance.GetService<IDebug>().Log($"Player {hit.transform.name} and name of target {targetNick} and me is {nickName}");
                    var target = hit.transform.GetComponent<PhotonView>();
                    target.RPC(nameof(TakeDamage), RpcTarget.All, damage + _weapon.GetDamage(), targetNick, _weapon.Id);
                    ServiceLocator.Instance.GetService<ISoundAndMusic>().PlaySfx(_weapon.GetShootSound());
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
    public void TakeDamage(float damage, string nickName, string weaponBaseId)
    {
        var nickl = this.nickName;
        ServiceLocator.Instance.GetService<IDebug>().Log($"{nickName} take damage {damage} and me is {nickl}");
        if (nickl == nickName)
        {
            var weaponBase = ServiceLocator.Instance.GetService<IWeaponsFactory>().Create(weaponBaseId);
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
            weaponBase.GetComponent<WeaponBase>().ApplyDamageEffect(this);
            Destroy(weaponBase.gameObject);
            ServiceLocator.Instance.GetService<ISoundAndMusic>().PlaySfx(takeDamageSound);
        }
        ServiceLocator.Instance.GetService<IPhotonRPC>().UpdateTable();
    }
    
    [PunRPC]
    public void ChangeWeapon(string weaponId)
    {
        ServiceLocator.Instance.GetService<IDebug>().Log($" {this.nickName} ChangeWeapon {_weapon.Id} to {weaponId}");
        if (_weapon != null)
        {
            Destroy(_weapon.gameObject);
        }
        _weapon = ServiceLocator.Instance.GetService<IWeaponsFactory>().Create(weaponId);
        _weapon.transform.SetParent(weaponHolder.transform);
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localRotation = Quaternion.identity;
        ServiceLocator.Instance.GetService<ISoundAndMusic>().PlaySfx(_weapon.GetGrabSound());
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

    public void Blind(float timeToBlind)
    {
        if(!photonView.IsMine) return;
        StartCoroutine(BlindC(timeToBlind));
    }

    private IEnumerator BlindC(float timeToBlind)
    {
        blindImage.gameObject.SetActive(true);
        blindImage.color = Color.white;
        //create transition
        var transition = 0f;
        while (transition < 1)
        {
            transition += Time.deltaTime / timeToBlind;
            blindImage.color = Color.Lerp(Color.white, Color.clear, transition);
            yield return null;
        }
        blindImage.gameObject.SetActive(false);
    }

    public void Stun(float timeToStun)
    {
        if (!photonView.IsMine) return;
        StartCoroutine(StunC(timeToStun));
    }

    private IEnumerator StunC(float timeToStun)
    {
        StopInput();
        yield return new WaitForSeconds(timeToStun);
        StartInput();
    }

    public void Kill(float timeToKill)
    {
        if (!photonView.IsMine) return;
        StartCoroutine(KillC(timeToKill));
    }

    private IEnumerator KillC(float timeToKill)
    {
        StopInput();
        canvasToInstaKill.SetActive(true);
        var time = timeToKill;
        while (time > 0)
        {
            countDownToDie.text = $"You will die in {time}";
            yield return new WaitForSeconds(1);
            time--;
        }
        life = 0;
        ServiceLocator.Instance.GetService<IDebug>().Log($" {this.nickName} is dead");
        photonView.RPC(nameof(Dead), RpcTarget.All);
        photonView.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            {"Life", life}
        });
    }
}