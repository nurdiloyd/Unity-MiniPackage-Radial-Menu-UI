using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour {

    [SerializeField] protected ObjectMotion objectMotion;
    [SerializeField] protected Obstacle obstacle;


    private void OnTriggerEnter(Collider other) {
        string oTag = other.transform.tag;
        if (oTag == "Player" || oTag == "Enemy") {
            objectMotion.Play();
        }
    }
}
