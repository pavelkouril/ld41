using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitCardPosition
{
    Attack,
    Def,
}

public class Card : MonoBehaviour
{
    public CardPrototype Prototype
    {
        get { return _proto; }
        set { _proto = value; Configure(); }
    }

    [SerializeField]
    private TextMesh _text_Name;

    [SerializeField]
    private TextMesh _text_Atk;

    [SerializeField]
    private TextMesh _text_Def;

    [SerializeField]
    private TextMesh _text_Eff;

    private CardPrototype _proto;

    public UnitCardPosition Position;

    public Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Configure()
    {
        _text_Name.text = _proto.Name;
        var unit = _proto as UnitCard;
        if (unit != null)
        {
            _text_Atk.text = unit.Attack.ToString();
            _text_Def.text = unit.Defense.ToString();
        }
        var sup = _proto as SupportCard;
        if (sup != null)
        {
            _text_Eff.text = sup.Text;
        }
    }
}
