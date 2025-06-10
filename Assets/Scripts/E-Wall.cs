using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class NewEmptyCSharpScript : MonoBehaviour
{
    private InputSystem_Actions controls;
    private GameObject player;
    public GameObject Wand;
    public float spawnDistance = 2f;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        player = GameObject.Find("Player") ?? null;

        controls.Player.Ability.performed += ctx =>
        {
            Debug.Log("Wall Ability Performed");
            if (player != null)
            {
                Vector3 playerposition = player.transform.position;
                Quaternion playerrotation = player.transform.rotation; 
                
                Vector3 spawnPosition = playerposition + player.transform.forward * spawnDistance;
               
                Instantiate(Wand, spawnPosition, playerrotation);


                Debug.Log("Position: " + playerposition.ToString() + "Rotation: " + playerrotation);
            }

        };


    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
