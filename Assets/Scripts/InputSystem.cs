using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{

    private InputSystem_Actions playerMovement;

    private void Awake()
    {
        playerMovement = new InputSystem_Actions();
        playerMovement.Player.Jump.performed += ctx => Debug.Log("Player jumped");
        playerMovement.Player.Move.performed += ctx => Debug.Log("Player moved");
        playerMovement.Player.Look.performed += ctx => Debug.Log("Player looked");
    }

    private void OnEnable()
    {
        playerMovement.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();   
    }
}