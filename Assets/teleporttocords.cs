using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporttocords : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 3f;
    public string playerTag = "Player";

    [Header("Teleport Coordinates")]
    public Vector3 teleportPosition = new Vector3(0f, 0f, 0f);

    [Header("Options")]
    public bool teleportOnce = true;

    private bool hasTeleported = false;

    void Update()
    {
        if (teleportOnce && hasTeleported)
            return;

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= detectionRadius)
        {
            TeleportPlayer(player);
        }
    }

    void TeleportPlayer(GameObject player)
    {
        player.transform.position = teleportPosition;
        hasTeleported = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}