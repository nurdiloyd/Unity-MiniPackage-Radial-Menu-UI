using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Obstacle : MonoBehaviour
{
    public bool Kills;
    [SerializeField] protected float pushPower;
    [SerializeField] protected Transform pushDirection;

    public HueColor Hue;
    public List<MeshRenderer> MeshRenderers;
    public List<Outline> Outlines;

    [HideInInspector] public bool Dissolved;

    private LevelManager _levelManager;


    private void Start() {
        _levelManager = transform.root.GetComponent<LevelManager>();
        _levelManager.Stage.DissolveObject += DissolveMe;
        
        if (MeshRenderers.Count == 0) {
            Outlines.Add(GetComponent<Outline>());
            MeshRenderers.Add(GetComponent<MeshRenderer>());
        }
    }

    public void DissolveMe(bool dissolve, HueColor hue) {
        if (Hue == hue || hue == HueColor.SELF) {
            StartCoroutine(Dissolve(dissolve));
        }
    } 

    private IEnumerator Dissolve(bool dissolve) {
        Dissolved = dissolve;
        
        if (!dissolve) {
            foreach (var outline in Outlines) {
                outline.eraseRenderer = true;
            }
            //transform.GetComponent<Collider>().enabled = true;
            gameObject.layer = 0;
        }

        float transition = dissolve ? 0 : 1;
        float transitionTo = dissolve ? 1 : 0;
        while (transition != transitionTo) {
            yield return new WaitForSeconds(Time.deltaTime);
            transition = Mathf.MoveTowards(transition, transitionTo, 7f * Time.deltaTime);
            foreach (var meshRenderer in MeshRenderers) {
                foreach (var material in meshRenderer.materials) {
                    material.SetFloat("_Transition", transition);
                }
            }
        }

        if (dissolve) {
            foreach (var outline in Outlines) {
                outline.eraseRenderer = false;
            }
            //transform.GetComponent<Collider>().enabled = false;
            gameObject.layer = 8;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Avoid(other.transform);
    }

    private void OnCollisionEnter(Collision other) {
        Avoid(other.transform);
    }

    private void Avoid(Transform other) {
        string oTag = other.tag;
        if (oTag == "Player" || oTag == "Enemy") {
            DissolveMe(true, Hue);
            
            if (Kills) {
                Vector3 pushVector = Vector3.zero;
                pushVector.x = Vector3.SignedAngle(pushDirection.forward, Vector3.left, transform.right);
                pushVector.y = Mathf.Abs(pushVector.x);
                pushVector.Normalize();

                other.GetComponent<CharacterController>().Push(pushVector * pushPower);
            }
        }
    }
}
