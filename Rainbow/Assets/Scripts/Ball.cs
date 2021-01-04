using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        string oTag = other.tag;
        if (oTag == "Player" || oTag == "Enemy") {
            transform.parent.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
