using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Stack<Card> Deck = new Stack<Card>();

    public List<Card> Hand = new List<Card>();

    public Card ActiveUnit;

    public int LifePoints = 10;

    public Transform HandParent;

    public HoverMotor Vehicle { get; internal set; }

    private void Awake()
    {
        Vehicle = GetComponent<HoverMotor>();
    }

    public void DestroyMonster()
    {
        MoveToGraveyard(ActiveUnit);
        ActiveUnit = null;
    }

    public void MoveToGraveyard(Card card)
    {
        Hand.Remove(card);
        StartCoroutine(AnimateCardOut(card, -1, 0.5f));
        if (card.gameObject != null)
        {
            Destroy(card.gameObject, 2f);
        }
    }

    private IEnumerator AnimateCardOut(Card card, int dir, float dur)
    {
        if (PlayerManager.Instance.Player1 == this)
        {
            float time = 0;
            while (card != null && card.gameObject != null)
            {
                time += Time.deltaTime;
                if (time > dur)
                {
                    foreach (var r in card.GetComponentsInChildren<Renderer>())
                    {
                        r.enabled = false;
                    }
                    break;
                }
                var pos = card.transform.position;
                pos.y += dir * 8 * Time.deltaTime;
                card.transform.position = pos;
                yield return new WaitForEndOfFrame();
            }
            MoveAllToCorrectPositions();
            yield return null;
        }
    }

    private void Summon(Card card)
    {
        Hand.Remove(card);
        StartCoroutine(AnimateCardOut(card, 1, 0.5f));
        StartCoroutine(WaitForActiveCardSet(card, 0.5f));
    }

    private IEnumerator WaitForActiveCardSet(Card card, float v)
    {
        yield return new WaitForSeconds(v);
        ActiveUnit = card;
    }

    internal void UnSelectCard(Card c)
    {
        var pos = c.transform.localPosition;
        pos.y = 0;
        c.transform.localPosition = pos;
    }

    public void SelectCard(Card c)
    {
        var pos = c.transform.localPosition;
        pos.y += 0.5f;
        c.transform.localPosition = pos;
    }

    public void AddToHand(Card card)
    {
        card.transform.parent = HandParent;
        var pos = card.transform.localPosition;
        pos.x = -(Hand.Count * 1.65f) + 3f;
        pos.y = 0;
        pos.z = 0;
        card.transform.localPosition = pos;
        card.gameObject.SetActive(true);
        Hand.Add(card);
    }

    public void MoveAllToCorrectPositions()
    {
        for (var i = 0; i < Hand.Count; i++)
        {
            var card = Hand[i];
            var pos = card.transform.localPosition;
            pos.x = -(i * 1.65f) + 3f;
            pos.y = 0;
            pos.z = 0;
            card.transform.localPosition = pos;
        }
    }

    public bool UseCard(Card c)
    {
        if (CardGameManager.Instance.CurrentPlayer != this)
        {
            return false;
        }
        var x = c.Prototype as SupportCard;
        if (x != null)
        {
            if (x.CanUseEffect(this))
            {
                UnSelectCard(c);
                StartCoroutine(ActivateEffectAfter(x, 0.5f));
                MoveToGraveyard(c);
                return true;
            }
        }
        var y = c.Prototype as UnitCard;
        if (y != null)
        {
            if (ActiveUnit == null)
            {
                Summon(c);
                return true;
            }
        }
        return false;
    }

    private IEnumerator ActivateEffectAfter(SupportCard x, float v)
    {
        yield return new WaitForSeconds(v);
        x.Effect(this);
    }

    public void TakeDamage(int v)
    {
        LifePoints -= v;
        if (LifePoints <= 0)
        {
            LifePoints = 0;
            CardGameManager.Instance.Lose(this);
        }
    }
}
