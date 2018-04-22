using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField]
    private Player _player1;

    [SerializeField]
    private Player _player2;

    public Player Player1
    {
        get
        {
            return _player1;
        }
    }

    public Player Player2
    {
        get
        {
            return _player2;
        }
    }

    public Player GetOpponentOf(Player currentPlayer)
    {
        if (currentPlayer == null)
        {
            throw new Exception("Cannot get opponent for null player.");
        }
        if (currentPlayer == _player1)
        {
            return _player2;
        }
        return _player1;
    }
}
