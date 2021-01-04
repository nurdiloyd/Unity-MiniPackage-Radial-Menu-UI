using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxWall : MonoBehaviour
{
    private ObjectMotion _objectMotion;


    private void Start() {
        _objectMotion = GetComponent<ObjectMotion>();
        StartCoroutine(PlayDelay(Random.Range(0, 11) / 5f));    
    }

    private IEnumerator PlayDelay(float delay) {
        yield return new WaitForSeconds(delay);
        _objectMotion.Play();
    } 
}
