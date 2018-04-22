using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingAiController : Singleton<DrivingAiController>
{
    private Player _player;

    [SerializeField]
    private Transform _wpParent;

    private DrivingWp[] _waypoints;

    [SerializeField]
    private int _currentWp;

    private void Awake()
    {
        _waypoints = _wpParent.GetComponentsInChildren<DrivingWp>();
    }

    private void Start()
    {
        _player = PlayerManager.Instance.Player2;
        StartCoroutine(AccellEveryInterval(0.08f));
    }

    private IEnumerator AccellEveryInterval(float seconds)
    {
        while (true)
        {
            if (CardGameManager.Instance.IsGameRunning)
            {
                _player.Vehicle.Input_SteerAccelerate();
                TurnTowardsNextWp();

            }
            yield return new WaitForSeconds(seconds);
        }
    }

    private void TurnTowardsNextWp()
    {
        var nextWP = _waypoints[(_currentWp + 1) % _waypoints.Length];
        var isLeft = IsLeft(_player.transform.position, _player.transform.position + _player.transform.forward, nextWP.transform.position);
        Debug.Log(isLeft);
        if (isLeft)
        {
            _player.Vehicle.Input_SteerLeft();
        }
        else
        {
            _player.Vehicle.Input_SteerRight();
        }

        if (Vector3.Distance(_player.transform.position, nextWP.transform.position) < 4f)
        {
            _currentWp++;
        }
    }

    private bool IsLeft(Vector3 a, Vector3 b, Vector3 c)
    {
        return ((b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x)) > 0;
    }
}
