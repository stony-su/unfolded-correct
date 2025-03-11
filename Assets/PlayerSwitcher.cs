using Unity.Cinemachine;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    private GameObject currentPlayer;
    private GameObject inactivePlayer;

    void Start()
    {
        InitializePlayers();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SwitchPlayers();
        }
    }

    void InitializePlayers()
    {
        if (player1.activeSelf)
        {
            currentPlayer = player1;
            inactivePlayer = player2;
        }
        else
        {
            currentPlayer = player2;
            inactivePlayer = player1;
        }
    }

    void SwitchPlayers()
    {
        Vector3 previousPosition = currentPlayer.transform.position;
        Quaternion previousRotation = currentPlayer.transform.rotation;

        if (currentPlayer == player1) 
        {
            previousPosition.y -= 7f; 
        }
        else if (currentPlayer == player2) 
        {
            previousPosition.y += 7f; 
        }

        currentPlayer.SetActive(false);
        inactivePlayer.SetActive(true);

        inactivePlayer.transform.SetPositionAndRotation(previousPosition, previousRotation);

        TransferVelocity(currentPlayer, inactivePlayer);

        GameObject temp = currentPlayer;
        currentPlayer = inactivePlayer;
        inactivePlayer = temp;

        //UpdateCinemachineTarget(currentPlayer.transform);
    }

    void TransferVelocity(GameObject from, GameObject to)
    {
        Rigidbody2D fromRb = from.GetComponent<Rigidbody2D>();
        Rigidbody2D toRb = to.GetComponent<Rigidbody2D>();

        if (fromRb != null && toRb != null)
        {
            toRb.linearVelocity = fromRb.linearVelocity;
            toRb.angularVelocity = fromRb.angularVelocity;
        }
    }

     /*
    void UpdateCinemachineTarget(Transform newTarget)
    {
        CinemachineVirtualCamera[] virtualCameras = FindObjectsOfType<CinemachineVirtualCamera>(true);
        foreach (var vcam in virtualCameras)
        {
            if (vcam.gameObject.activeInHierarchy)
            {
                vcam.Follow = newTarget;
                vcam.LookAt = newTarget; 
            }
        }
    }
    */
}