using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] protected float defaultSpeed;
    protected LevelManager levelManager;

    protected Rigidbody _rb;
    private float _speed;
    private Animator _animator;
    protected Vector3 defaultPosition;


    private void Start() {
        levelManager = FindObjectOfType<LevelManager>();

        _rb = GetComponent<Rigidbody>();  
        defaultPosition = transform.position;
        
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _animator.SetFloat("IdleSelect", Random.Range(0, 3));

        AdjustSpeed(1);
    }

    private void Update() {
        if (levelManager.LevelStarted) {
            // Virtual Gravity
            _rb.AddForce(-Vector3.up * 10);
            
            if (_speed > 0) {
                // Movement
                //Vector3 vel = _rb.velocity;
                //vel.z = Mathf.Lerp(vel.z, _speed, 40f * Time.deltaTime);
                //_rb.velocity = vel;
                _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.forward * _speed, 40f * Time.deltaTime);
                //transform.position = Vector3.Lerp(transform.position, (transform.position + Vector3.forward), _speed * Time.deltaTime);
            }

            _animator.SetBool("isRunning", _speed > 0 ? true : false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        string oTag = other.transform.tag;
        if (oTag == "Obstacle") {
            if (tag == "Player") {
                TapticPlugin.TapticManager.Impact(TapticPlugin.ImpactFeedback.Medium);
            }

            StartCoroutine(SlowDownAWhile(1, 0.5f));
        }
        else if (oTag == "Elevator"){
            other.transform.parent.GetComponent<ElevatorController>().Run();
            StartCoroutine(SlowDownAWhile(1, 0));
        }
        else if (oTag == "Finish") {
            IWon();
            StartCoroutine(SlowDownAWhile(0, 0));
        }
    }

    public void AdjustSpeed(float speedMultiplier) {
        _speed = defaultSpeed * speedMultiplier;
        _animator.SetFloat("RunSelect", 1 - _speed / defaultSpeed);

    }

    private IEnumerator SlowDownAWhile(float time, float speedMultiplier) {
        AdjustSpeed(speedMultiplier);

        yield return new WaitForSeconds(time);

        if (time == 0) {
            speedMultiplier = 1;    
        }
        
        while (speedMultiplier != 1) {
            if (Mathf.Abs(speedMultiplier - 1) < 0.1f) {
                speedMultiplier = 1;
            }

            yield return new WaitForSeconds(Time.deltaTime);
            speedMultiplier = Mathf.Lerp(speedMultiplier, 1, 2.5f * Time.deltaTime);
            
            AdjustSpeed(speedMultiplier);
        }
    }

    public void Push(Vector3 pushVector) {
        AdjustSpeed(0);
        _rb.AddForce(pushVector);
        KillMe();
    }

    protected virtual void IWon() {
        _animator.SetBool("isRunning", false);
        _animator.SetFloat("WinSelect", Random.Range(0, 3));
        _animator.SetBool("isWon", true);
        StartCoroutine(Stop());
    }

    private IEnumerator Stop() {
        yield return new WaitForSeconds(0.4f);
        _rb.isKinematic = true;
    }
    
    public virtual void KillMe() {
    }
}
