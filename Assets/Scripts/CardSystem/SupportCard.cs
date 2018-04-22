using System;

public class SupportCard : CardPrototype
{
    public override CardType Type => CardType.Support;

    public string Text { get; internal set; }

    public Func<Player, bool> CanUseEffect;

    public Action<Player> Effect;
}
