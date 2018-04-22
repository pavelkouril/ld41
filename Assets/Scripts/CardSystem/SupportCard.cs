using System;

public class SupportCard : CardPrototype
{
    public override CardType Type => CardType.Support;

    public Func<Player, bool> CanUseEffect;

    public Action<Player> Effect;
}
