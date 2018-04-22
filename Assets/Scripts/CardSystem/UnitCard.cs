using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCard : CardPrototype
{
    public int Attack;

    public int Defense;

    public override CardType Type => CardType.Unit;
}