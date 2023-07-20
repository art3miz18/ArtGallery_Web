using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCspawning : MonoBehaviour
{
    public int _NpcCount ;
    public GameObject[] npcPrefab; // assign your NPC prefab in the Inspector
    public float spawnRadius = 50; // define the radius within which the NPC can be spawned
    public List<Transform> waypoints;
    private void Start() {
        _NpcCount = npcPrefab.Length;
        for(int i=0 ; i<_NpcCount; i++){
            SpawnNPC(npcPrefab[i]);
        }
    }
    public void SpawnNPC(GameObject npc) {
        // Random point within a sphere
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;

        // Variables to hold the result
        NavMeshHit hit;

        // Try to sample the NavMesh
        if (NavMesh.SamplePosition(randomPoint, out hit, spawnRadius, NavMesh.AllAreas)) {
            // If a valid position on the NavMesh has been found, spawn the NPC
            Instantiate(npc, hit.position, Quaternion.identity,this.transform);
            
        }
    }

}
