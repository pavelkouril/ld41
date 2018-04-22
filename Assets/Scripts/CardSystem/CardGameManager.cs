using System.Collections;
using System.Linq;
using UnityEngine;

public enum TurnPhase
{
    Begin,
    Draw,
    Main,
    Battle,
    End,
}

public class CardGameManager : Singleton<CardGameManager>
{
    private const float kTurnLength = 15;

    private Player _currentPlayer;

    private TurnPhase _currentPhase;

    public Player CurrentPlayer { get => _currentPlayer; set => _currentPlayer = value; }

    private float _currentTurnStart;

    private bool _alreadyDrew;

    [SerializeField]
    private Card _supportCardPrefab;

    [SerializeField]
    private Card _unitCardPrefab;

    public bool IsFirstTurn;

    public float CurrentTurnTime
    {
        get
        {
            return Time.time - _currentTurnStart;
        }
    }

    public TurnPhase CurrentPhase { get => _currentPhase; }

    public bool IsGameRunning { get; set; }

    private void Start()
    {
        StartCoroutine(CountDownToStartGame(3));
    }

    private IEnumerator CountDownToStartGame(int v)
    {
        for (int i = 0; i < v; i++)
        {
            yield return new WaitForSeconds(1);
        }
        StartGame();
        yield return null;
    }

    public void StartGame()
    {
        _currentPlayer = UnityEngine.Random.Range(0, 2) > 0 ? PlayerManager.Instance.Player1 : PlayerManager.Instance.Player2;

        foreach (var i in CardDatabase.GetDeck())
        {
            PlayerManager.Instance.Player1.Deck.Push(CreateCardGo(i));
        }

        foreach (var i in CardDatabase.GetDeck())
        {
            PlayerManager.Instance.Player2.Deck.Push(CreateCardGo(i));
        }

        for (int i = 0; i < 2; i++)
        {
            var cP1 = PlayerManager.Instance.Player1.Deck.Pop();
            PlayerManager.Instance.Player1.AddToHand(cP1);

            var cP2 = PlayerManager.Instance.Player2.Deck.Pop();
            PlayerManager.Instance.Player2.AddToHand(cP2);
        }
        // we switch the first Begin phase when starting a new game
        _currentTurnStart = Time.time;
        _currentPhase = TurnPhase.Draw;
        PlayerManager.Instance.Player1.SelectCard(PlayerManager.Instance.Player1.Hand.First());
        IsFirstTurn = true;
        IsGameRunning = true;
    }

    public bool Input_NextPhase(Player player, out string err)
    {
        err = string.Empty;
        if (_currentPlayer != player)
        {
            return false;
        }
        if (CurrentPhase != TurnPhase.Main && CurrentPhase != TurnPhase.Battle)
        {
            err = "Can only move to next phase from MP or BP.";
            return false;
        }
        if (player.ActiveUnit == null)
        {
            err = "Cannot go to BP without a active unit card.";
            return false;
        }
        if (IsFirstTurn)
        {
            NextPhase();
        }
        NextPhase();
        return true;
    }

    internal void Input_DeclareAttack(Player player)
    {
        if (player.ActiveUnit != null && player.ActiveUnit.Position == UnitCardPosition.Attack)
        {
            var op = PlayerManager.Instance.GetOpponentOf(player);
            var playerUnit = (player.ActiveUnit.Prototype as UnitCard);
            if (op.ActiveUnit == null)
            {
                op.TakeDamage(playerUnit.Attack);
            }
            else if (op.ActiveUnit.Position == UnitCardPosition.Attack)
            {
                var opUnit = (op.ActiveUnit.Prototype as UnitCard);
                if (opUnit.Attack == playerUnit.Attack)
                {
                    DestroyMonster(op.ActiveUnit);
                    DestroyMonster(player.ActiveUnit);
                    op.ActiveUnit = null;
                    player.ActiveUnit = null;
                }
                else if (opUnit.Attack > playerUnit.Attack)
                {
                    player.TakeDamage(opUnit.Attack - playerUnit.Attack);
                }
                else if (opUnit.Attack < playerUnit.Attack)
                {
                    op.TakeDamage(playerUnit.Attack - opUnit.Attack);
                }
            }
            else if (op.ActiveUnit.Position == UnitCardPosition.Def)
            {
                var opUnit = (op.ActiveUnit.Prototype as UnitCard);
                if (opUnit.Defense < playerUnit.Attack)
                {
                    DestroyMonster(op.ActiveUnit);
                }
                else if (opUnit.Defense > playerUnit.Attack)
                {
                    player.TakeDamage(opUnit.Defense - playerUnit.Attack);
                }
            }

            NextPhase();
        }
    }

    public void Input_SwitchBattlePos(Player p)
    {
        if (p.ActiveUnit == null)
        {
            return;
        }
        if (p.ActiveUnit.Position == UnitCardPosition.Attack)
        {
            Input_SwitchBattlePos(p, UnitCardPosition.Def);
        }
        else
        {
            Input_SwitchBattlePos(p, UnitCardPosition.Attack);
        }
    }

    public void Input_SwitchBattlePos(Player p, UnitCardPosition pos)
    {
        if (p.ActiveUnit == null)
        {
            return;
        }
        p.ActiveUnit.Position = pos;
    }

    private void DestroyMonster(Card activeUnit)
    {
        Destroy(activeUnit.gameObject, 0.5f);
    }

    private Card CreateCardGo(CardPrototype proto)
    {
        var card = Instantiate(proto.Type == CardType.Support ? _supportCardPrefab : _unitCardPrefab);
        card.Prototype = proto;
        card.gameObject.SetActive(false);
        return card;
    }

    private void NextPhase()
    {
        _currentPhase = (TurnPhase)(((int)_currentPhase + 1) % 5);
    }

    private void Update()
    {
        if (!IsGameRunning)
        {
            return;
        }
        // evaluate the game loop during each phase
        switch (_currentPhase)
        {
            case TurnPhase.Begin:
                SwitchPlayers();
                _currentTurnStart = Time.time;
                _alreadyDrew = false;
                IsFirstTurn = false;
                NextPhase();
                break;
            case TurnPhase.Draw:
                if (!_alreadyDrew)
                {
                    StartCoroutine(DrawPhase(1f));
                    _alreadyDrew = true;
                }
                break;
            case TurnPhase.Main:
                CheckForTimeout();
                break;
            case TurnPhase.Battle:
                CheckForTimeout();
                break;
            case TurnPhase.End:
                EndTurn();
                break;
        }
    }

    private IEnumerator DrawPhase(float sec)
    {
        if (!DrawCard(_currentPlayer))
        {
            Lose(_currentPlayer);
        }
        yield return new WaitForSeconds(sec);
        NextPhase();
    }

    private bool CheckForTimeout()
    {
        if (Time.time > _currentTurnStart + kTurnLength)
        {
            EndTurn();
            return true;
        }
        return false;
    }

    private void SwitchPlayers()
    {
        _currentPlayer = PlayerManager.Instance.GetOpponentOf(_currentPlayer);
    }

    private void EndTurn()
    {
        _currentPhase = TurnPhase.Begin;
        // remove random cards if we have more than 4 in hand after end of our turn
        while (_currentPlayer.Hand.Count > 4)
        {
            var card = _currentPlayer.Hand[UnityEngine.Random.Range(0, _currentPlayer.Hand.Count)];
            _currentPlayer.MoveToGraveyard(card);
        }
    }

    public void Lose(Player p)
    {
        if (p == PlayerManager.Instance.Player1)
        {
            UiManager.Instance.SetWinLoseText("You lose");
        }
        else
        {
            UiManager.Instance.SetWinLoseText("You win");
        }
        IsGameRunning = false;
    }

    public bool CanPlayCards(Player p)
    {
        return p == CurrentPlayer && _currentPhase == TurnPhase.Main;
    }

    public bool CanAttack(Player p)
    {
        return p == CurrentPlayer && _currentPhase == TurnPhase.Battle;
    }

    public bool DrawCard(Player currentPlayer)
    {
        if (currentPlayer.Deck.Count == 0)
        {
            return false;
        }
        var card = currentPlayer.Deck.Pop();
        currentPlayer.AddToHand(card);
        return true;
    }
}
