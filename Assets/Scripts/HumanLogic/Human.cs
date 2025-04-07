using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class Human : MonoBehaviour
{
    [SerializeField] private Transform[] _posRight;
    [SerializeField] private Transform[] _posLeft;
    [SerializeField] private Animator _animator;

    [SerializeField] private CurrentHumanState _state;

    private NavMeshAgent _ai;
    private bool _isWalking;

    private void Start()
    {
        _ai = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        StartCoroutine(RandomMove());
        StartCoroutine(CheckWalkingState());

        _ai.avoidancePriority = Random.Range(0, 100);
        _ai.speed = Random.Range(2.9f, 3.55f);
        _ai.angularSpeed = 240;
    }

    private IEnumerator CheckWalkingState()
    {
        WaitForSeconds wait = new(0.2f);

        while (true)
        {
            yield return wait;
            bool newIsWalking = _ai.hasPath && _ai.velocity.sqrMagnitude > 0.01f;
            if (newIsWalking != _isWalking)
            {
                _isWalking = newIsWalking;
                _animator.SetBool("isWalking", _isWalking);
            }
        }
    }

    private void Move()
    {
        if (Physics.OverlapSphere(transform.position, 25, LayerMask.GetMask("Car")).Length > 0) return;


        _state = _state == CurrentHumanState.Left ? CurrentHumanState.Right : CurrentHumanState.Left;
        _ai.SetDestination(_state == CurrentHumanState.Left ? _posLeft.PickRandomElement().position : _posRight.PickRandomElement().position);
    }

    private IEnumerator RandomMove()
    {
        var wait = new WaitForSeconds(Random.Range(8, 30));
        while (true)
        {
            yield return wait;
            try
            {
                Move();
            }
            catch { }
        }
    }
}

public enum CurrentHumanState
{
    Left = 1,
    Right = 2
}