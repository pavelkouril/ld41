using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardDatabase
{
    public static Dictionary<string, CardPrototype> Prototypes = new Dictionary<string, CardPrototype>();

    static CardDatabase()
    {
        RegisterCard(new UnitCard()
        {
            Name = "Goo",
            Attack = 5,
            Defense = 5,
        });

        RegisterCard(new SupportCard()
        {
            Name = "Smash!",
            CanUseEffect = (player) =>
            {
                var opp = PlayerManager.Instance.GetOpponentOf(player);
                return opp.ActiveUnit != null;
            },
            Effect = (player) =>
            {
                var opp = PlayerManager.Instance.GetOpponentOf(player);
                opp.DestroyMonster();
            }
        });
    }

    public static List<CardPrototype> GetDeck()
    {
        var cards = new List<CardPrototype>();
        cards.Add(Prototypes["Goo"]);
        cards.Add(Prototypes["Smash!"]);
        cards.Add(Prototypes["Goo"]);
        cards.Add(Prototypes["Smash!"]);
        cards.Add(Prototypes["Goo"]);
        cards.Add(Prototypes["Goo"]);
        cards.Add(Prototypes["Goo"]);
        cards.Shuffle();
        return cards;
    }

    public static void RegisterCard(CardPrototype card)
    {
        Prototypes.Add(card.Name, card);
    }

    public static void Shuffle<T>(this List<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
