using System;
using Photon.Pun;
using UnityEngine;

public class Armor : MonoBehaviourPunCallbacks
{
    [SerializeField] private string weaponId;

    private void OnTriggerEnter(Collider other)
    {
        //Send id to player to change weapon
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<Player>();
            other.gameObject.GetComponent<PhotonView>().RPC(nameof(player.ChangeWeapon), RpcTarget.All, weaponId);
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        //rotate object
        transform.Rotate(Vector3.up * (50 * Time.deltaTime));
    }
}