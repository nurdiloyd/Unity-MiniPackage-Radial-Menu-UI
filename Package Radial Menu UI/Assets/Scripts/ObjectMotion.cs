using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;

public class ObjectMotion : MonoBehaviour
{
    [SerializeField] protected bool playOnStart;
    
    [Header("Move Tween")]
    [SerializeField] protected LeanTweenType moveTweenType;
    [SerializeField] protected Vector3 moveDirection = Vector3.forward;
    [Min(0)] [SerializeField] protected float moveTweenTime;
    [SerializeField] protected float moveDistance;
    [SerializeField] protected bool movePingPong;
    [SerializeField] protected bool moveClamp;

    [Header("Rotation Tween")]
    [SerializeField] protected LeanTweenType rotationTweenType;
    [Min(0)] [SerializeField] protected float rotationTweenTime;
    [Range(0, 360)] [SerializeField] protected float rotationAngle;
    [SerializeField] protected bool rotationPingPong;
    [SerializeField] protected bool rotationClamp;

    [HideInInspector] public int MoveTurnCounter;
    [HideInInspector] public int RotationTurnCounter;


    private void Start() {
        if (playOnStart) {
            Play();
        }
    }
    
    public void Play() {
        // Move Tween
        LTDescr moveLT;
        moveLT = LeanTween.move(gameObject, transform.position + moveDirection.normalized * moveDistance, moveTweenTime);
        moveLT.setEase(moveTweenType);
        moveLT.setOnComplete(IncreaseMoveTurn);
        
        if (moveClamp) {
            moveLT.setLoopClamp();
        }
        
        if (movePingPong) {
            moveLT.setLoopPingPong();
        }

        // Rotation Tween
        LTDescr rotationLT;
        rotationLT = LeanTween.rotateAround(gameObject, transform.forward, -rotationAngle, rotationTweenTime);
        rotationLT.setEase(rotationTweenType);
        rotationLT.setOnComplete(IncreaseRotationTurn);
        
        if (rotationClamp) {
            rotationLT.setLoopClamp();
        }
        
        if (rotationPingPong) {
            rotationLT.setLoopPingPong();
        }
    }

    private void IncreaseMoveTurn() {
        MoveTurnCounter += 1;
    }

    private void IncreaseRotationTurn() {
        RotationTurnCounter += 1;
    }
}