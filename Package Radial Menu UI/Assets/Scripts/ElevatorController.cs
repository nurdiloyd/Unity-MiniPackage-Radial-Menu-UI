using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] public bool IsEnabled;


    public void Run() {
        StartCoroutine(StartToMove());
    }

    private IEnumerator StartToMove() {
        IsEnabled = true;
        yield return new WaitForSeconds(0.5f);
        GetComponent<ObjectMotion>().Play();
    }
}