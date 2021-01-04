using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishController : MonoBehaviour
{
    [SerializeField] protected GameObject confettis;


    private void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Player") {
            confettis.SetActive(true);
        } 
    }
}
