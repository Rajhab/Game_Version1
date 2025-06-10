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
        playerMovement.Player.Jump.performed += ctx => Debug.Log(playerMovement.Player.Jump.triggered);
        if (playerMovement.Player.Jump.triggered == true)
        {
            GameObject obj = new GameObject("Test Spawn Objekt!");
            obj.AddComponent<Light>();
            obj.transform.position = new Vector3(-2, 1, 0);
        }
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