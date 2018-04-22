using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Unit,
    Support,
}

public abstract class CardPrototype
{
    abstract public CardType Type { get; }

    public string Name;
}

