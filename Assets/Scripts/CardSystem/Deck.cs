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
            Attack = 1,
            Defense = 0,
        });

        RegisterCard(new UnitCard()
        {
            Name = "Pusher",
            Attack = 4,
            Defense = 1,
        });

        RegisterCard(new UnitCard()
        {
            Name = "Guardian",
            Attack = 1,
            Defense = 4,
        });

        RegisterCard(new UnitCard()
        {
            Name = "Nightmare",
            Attack = 2,
            Defense = 0,
        });

        RegisterCard(new UnitCard()
        {
            Name = "Critter",
            Attack = 3,
            Defense = 1,
        });

        RegisterCard(new SupportCard()
        {
            Name = "Smash!",
            Text = "Destroy\n opponent's\n unit.",
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

        RegisterCard(new SupportCard()
        {
            Name = "Ready! Draw!",
            Text = "Draw 1 card.",
            CanUseEffect = (player) =>
            {
                return player.Deck.Count > 1;
            },
            Effect = (player) =>
            {
                CardGameManager.Instance.DrawCard(player);
            }
        });

        RegisterCard(new SupportCard()
        {
            Name = "Heal",
            Text = "Regain 3 HP.",
            CanUseEffect = (player) =>
            {
                return player.LifePoints < 10;
            },
            Effect = (player) =>
            {
                player.LifePoints += 3;
                if (player.LifePoints > 10)
                {
                    player.LifePoints = 10;
                }
            }
        });

        RegisterCard(new SupportCard()
        {
            Name = "Earthquake",
            Text = "Destroy both\n player's units.",
            CanUseEffect = (player) =>
            {
                var opp = PlayerManager.Instance.GetOpponentOf(player);
                return opp.ActiveUnit != null || player.ActiveUnit != null;
            },
            Effect = (player) =>
            {
                var opp = PlayerManager.Instance.GetOpponentOf(player);
                opp.DestroyMonster();
                player.DestroyMonster();
            }
        });
    }

    public static List<CardPrototype> GetDeck()
    {
        var cards = new List<CardPrototype>();

        cards.Add(Prototypes["Goo"]);
        cards.Add(Prototypes["Goo"]);
        cards.Add(Prototypes["Goo"]);

        cards.Add(Prototypes["Nightmare"]);
        cards.Add(Prototypes["Nightmare"]);

        cards.Add(Prototypes["Guardian"]);
        cards.Add(Prototypes["Guardian"]);

        cards.Add(Prototypes["Pusher"]);
        cards.Add(Prototypes["Pusher"]);

        cards.Add(Prototypes["Critter"]);

        cards.Add(Prototypes["Smash!"]);
        cards.Add(Prototypes["Smash!"]);
        cards.Add(Prototypes["Smash!"]);

        cards.Add(Prototypes["Earthquake"]);
        cards.Add(Prototypes["Earthquake"]);

        cards.Add(Prototypes["Heal"]);
        cards.Add(Prototypes["Heal"]);

        cards.Add(Prototypes["Ready! Draw!"]);
        cards.Add(Prototypes["Ready! Draw!"]);
        cards.Add(Prototypes["Ready! Draw!"]);

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
