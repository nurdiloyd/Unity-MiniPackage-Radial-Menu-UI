using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        string oTag = other.transform.tag;
        if (oTag == "Player" || oTag == "Enemy") {
            other.transform.GetComponent<CharacterController>().KillMe();;
        }
    }
}
