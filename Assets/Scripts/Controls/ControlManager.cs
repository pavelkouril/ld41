using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : Singleton<ControlManager>
{
    private int _selectedCardIndex;

    private void Update()
    {
        if (!CardGameManager.Instance.IsGameRunning)
        {
            return;
        }

        var player = PlayerManager.Instance.Player1;

        if (player.Hand.Count == 0)
        {
            return;
        }

        if (_selectedCardIndex > player.Hand.Count - 1)
        {
            _selectedCardIndex = player.Hand.Count - 1;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (CardGameManager.Instance.CurrentPhase != TurnPhase.Battle)
            {
                player.UnSelectCard(player.Hand[_selectedCardIndex]);
                var newIndex = _selectedCardIndex - 1;
                if (newIndex < 0)
                {
                    newIndex = player.Hand.Count - 1;
                }
                _selectedCardIndex = newIndex;
                player.SelectCard(player.Hand[_selectedCardIndex]);
            }
            else
            {
                CardGameManager.Instance.Input_SwitchBattlePos(player);
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {

            if (CardGameManager.Instance.CurrentPhase != TurnPhase.Battle)
            {
                player.UnSelectCard(player.Hand[_selectedCardIndex]);
                _selectedCardIndex = ++_selectedCardIndex % player.Hand.Count;
                player.SelectCard(player.Hand[_selectedCardIndex]);
            }
            else
            {
                CardGameManager.Instance.Input_SwitchBattlePos(player);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (CardGameManager.Instance.CurrentPhase == TurnPhase.Main)
            {
                UseSelectedCard(player.Hand[_selectedCardIndex]);
            }
            if (CardGameManager.Instance.CurrentPhase == TurnPhase.Battle)
            {
                CardGameManager.Instance.Input_DeclareAttack(player);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            string e;
            CardGameManager.Instance.Input_NextPhase(player, out e);
        }
    }

    private void UseSelectedCard(Card c)
    {
        var player = PlayerManager.Instance.Player1;

        var newIndex = _selectedCardIndex - 1;
        if (newIndex < 0)
        {
            newIndex = player.Hand.Count - 1;
        }

        Card newSel = player.Hand[newIndex];

        if (player.UseCard(c))
        {
            _selectedCardIndex = newIndex;
            player.SelectCard(newSel);
        }
    }
}
