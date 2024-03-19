using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputCustom : MonoBehaviour
{
    public float Move => move; 
    public float Rotate => rotate;
    private float rotate;
    private float move;
    public Action OnFire;
    [SerializeField] private bool canReadInput = false;
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if(!canReadInput) return;
        Vector2 move = context.ReadValue<Vector2>();
        this.move = move.y;
        rotate = move.x;
    }
    
    public void Fire(InputAction.CallbackContext context)
    {
        if(!canReadInput) return;
        if (context.performed)
        {
            OnFire?.Invoke();
        }
    }

    public void StopInput()
    {
        canReadInput = false;
    }

    public void StartInput()
    {
        canReadInput = true;
    }
}