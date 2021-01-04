using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterController 
{
    public override void KillMe() {
        if (!levelManager.LevelCompletedMe) {
            StartCoroutine(SpawnBehindPlayer());        
        }
    }

    private IEnumerator SpawnBehindPlayer() {
        yield return new WaitForSeconds(1f);
        _rb.velocity = Vector3.zero;
        Vector3 newSpawnPos = levelManager.Player.transform.position;
        newSpawnPos.x = defaultPosition.x;
        newSpawnPos.z -= Random.Range(4, 8);
        transform.position = newSpawnPos; 
    }
}
