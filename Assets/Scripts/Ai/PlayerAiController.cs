using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAiController : MonoBehaviour
{
    private bool _isOnCoolDown;

    [SerializeField]
    private Transform _useFromHandParent;

    private void Update()
    {
        if (!CardGameManager.Instance.IsGameRunning)
        {
            return;
        }

        if (CardGameManager.Instance.CurrentPlayer != PlayerManager.Instance.Player2)
        {
            return;
        }

        if (_isOnCoolDown)
        {
            return;
        }

        switch (CardGameManager.Instance.CurrentPhase)
        {
            case TurnPhase.Main:
                DoMainPhaseActions(PlayerManager.Instance.Player2);
                break;
            case TurnPhase.Battle:
                DoBattlePhaseActions(PlayerManager.Instance.Player2);
                break;
        }
    }

    private void DoMainPhaseActions(Player p)
    {
        Card maxAtkUnit = null;

        // scan options
        foreach (var c in p.Hand)
        {
            if (c.Prototype.Type == CardType.Unit)
            {
                var unit = c.Prototype as UnitCard;
                if (maxAtkUnit == null)
                {
                    maxAtkUnit = c;
                }
                else if (unit.Attack > (maxAtkUnit.Prototype as UnitCard).Attack)
                {
                    maxAtkUnit = c;
                }
            }
        }

        if (maxAtkUnit != null)
        {
            p.UseCard(maxAtkUnit);
            StartCoroutine(StartCoolDown(1.5f));
        }

        var usableSupports = GetUsableSupportCards(p);
        if (usableSupports.Count > 0)
        {
            var c = usableSupports.First();
            p.UseCard(usableSupports.First());
            c.transform.parent = _useFromHandParent;
            StartCoroutine(StartCoolDown(1.5f));
            c._animator.enabled = true;
            c._animator.Play("UseCard");
        }


        string str;
        CardGameManager.Instance.Input_NextPhase(p, out str);
        StartCoroutine(StartCoolDown(1));
    }

    private List<Card> GetUsableSupportCards(Player p)
    {
        var usableSupports = new List<Card>();
        foreach (var c in p.Hand)
        {
            if (c.Prototype.Type == CardType.Support)
            {
                var sup = c.Prototype as SupportCard;
                if (sup.CanUseEffect(p))
                {
                    usableSupports.Add(c);
                }
            }
        }
        return usableSupports;
    }

    private void DoBattlePhaseActions(Player p)
    {
        if (p.ActiveUnit == null || CardGameManager.Instance.IsFirstTurn)
        {
            string str;
            CardGameManager.Instance.Input_NextPhase(p, out str);
            return;
        }
        var op = PlayerManager.Instance.GetOpponentOf(p);

        if (op.ActiveUnit == null)
        {
            if (p.ActiveUnit.Position == UnitCardPosition.Def)
            {
                CardGameManager.Instance.Input_SwitchBattlePos(p, UnitCardPosition.Attack);
                StartCoroutine(StartCoolDown(1f));
            }
            else
            {
                CardGameManager.Instance.Input_DeclareAttack(p);
            }
        }
        else
        {
            if ((op.ActiveUnit.Prototype as UnitCard).Attack < (p.ActiveUnit.Prototype as UnitCard).Attack)
            {
                if (p.ActiveUnit.Position == UnitCardPosition.Def)
                {
                    CardGameManager.Instance.Input_SwitchBattlePos(p, UnitCardPosition.Attack);
                    StartCoroutine(StartCoolDown(1f));
                }
                else
                {
                    CardGameManager.Instance.Input_DeclareAttack(p);
                }
            }
            else if (p.ActiveUnit.Position == UnitCardPosition.Attack)
            {
                CardGameManager.Instance.Input_SwitchBattlePos(p, UnitCardPosition.Def);
                StartCoroutine(StartCoolDown(2f));
            }
            else
            {
                if (p.ActiveUnit.Position == op.ActiveUnit.Position)
                {
                    string str;
                    CardGameManager.Instance.Input_NextPhase(p, out str);
                }
            }
        }
    }

    private IEnumerator StartCoolDown(float secs)
    {
        _isOnCoolDown = true;
        yield return new WaitForSeconds(secs);
        _isOnCoolDown = false;
    }
}
