using CityGen;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(AudioSource))]
public class Human : MonoBehaviour
{
    [SerializeField] private Transform _posRight;
    [SerializeField] private Transform _posLeft;

    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private CurrentHumanState _state;

    private Animator _animator;
    private AudioSource _audioSource;
    private Collider _collider;
    private NavMeshAgent _ai;

    private bool _isWalking;
    private bool _isDied;

    private void Start()
    {
        _ai = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();

        _ai.avoidancePriority = Random.Range(0, 100);
        _ai.stoppingDistance = Random.Range(0, 0.6f);
        _ai.speed = Random.Range(2.6f, 3.55f);
        _ai.angularSpeed = 240;
    }

    public Human SetPositions(Collider leftCollider, Collider rightCollider)
    {
        if (_posRight != null) return this;

        _posLeft = leftCollider.transform;
        _posRight = rightCollider.transform;

        return this;
    }

    public Human SetState(CurrentHumanState state)
    {
        _state = state;

        return this;
    }

    private void FixedUpdate()
    {
        if (_isDied) return;

        bool status = _ai.hasPath && _ai.velocity.sqrMagnitude >= 0.1f;

        if (_isWalking != status)
        {
            _isWalking = status;
            _animator.SetBool("isWalking", _isWalking);
        }
    }

    public IEnumerator ChangeState()
    {
        yield return new WaitForSeconds(Random.Range(0.01f, 1.4f));

        _state = _state == CurrentHumanState.Left ? CurrentHumanState.Right : CurrentHumanState.Left;
        _ai.SetDestination(_state == CurrentHumanState.Left ? _posLeft.position : _posRight.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out TrafficCar car) && car.GetSpeed() >= 5)
        {
            _isDied = true;
            transform.LookAt(collision.transform);
            _animator.Play("death");
            _audioSource.time = 1f;
            _audioSource.PlayOneShot(_hitClip);
            _ai.isStopped = true;
            Invoke(nameof(Death), 2f);
        }
    }

    private void Death()
    {
        _collider.enabled = false;
        enabled = false;
    }
}

public enum CurrentHumanState
{
    Left = 1,
    Right = 2
}