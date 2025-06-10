using UnityEngine;
using UnityEngine.InputSystem;

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

        controls.Player.Ability.performed += ctx => PlaceWall();    

    }

    private void PlaceWall()
    {
        Debug.Log("Wall Ability Performed");
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;
            Quaternion playerRotation = player.transform.rotation;

            Vector3 spawnPosition = playerPosition + player.transform.forward * spawnDistance;

            GameObject spawnedWall = Instantiate(Wand, spawnPosition, playerRotation);

            string[] coneNames = { "Cone", "Cone.001", "Cone.002", "Cone.003", "Cone.004", "Cone.005" };

            foreach (string coneName in coneNames)
            {
                Transform coneTransform = spawnedWall.transform.Find(coneName);

                GameObject cone = coneTransform.gameObject;

                MeshCollider meshCol = cone.AddComponent<MeshCollider>();
                meshCol.convex = true; // Needed if using Rigidbody or for proper collision
                Rigidbody rb = cone.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Debug.Log("Position: " + playerPosition.ToString() + "Rotation: " + playerRotation);
        }
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
