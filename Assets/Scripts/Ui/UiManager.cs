using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    [SerializeField]
    private Text _text_LeftCardsInDeckPlayer1;

    [SerializeField]
    private Text _text_LeftCardsInDeckPlayer2;

    [SerializeField]
    private Text _text_CardsInHandPlayer1;

    [SerializeField]
    private Text _text_CardsInHandPlayer2;

    [SerializeField]
    private Text _text_ActiveUnitPlayer1;

    [SerializeField]
    private Text _text_ActiveUnitPlayer2;

    [SerializeField]
    private Text _text_TurnTime;

    [SerializeField]
    private Text _text_CurrentPhase;

    [SerializeField]
    private Slider _slider_LpPlayer1;

    [SerializeField]
    private Slider _slider_LpPlayer2;

    [SerializeField]
    private Text _text_WinLose;

    [SerializeField]
    private Image _image_UnitP1;

    [SerializeField]
    private Image _image_UnitP2;

    private void Start()
    {
        _text_ActiveUnitPlayer1.text = string.Empty;
        _image_UnitP1.gameObject.SetActive(false);
        _image_UnitP2.gameObject.SetActive(false);
        _text_ActiveUnitPlayer2.text = string.Empty;
        _text_WinLose.text = string.Empty;
    }

    private void Update()
    {
        if (!CardGameManager.Instance.IsGameRunning)
        {
            return;
        }

        _text_LeftCardsInDeckPlayer1.text = PlayerManager.Instance.Player1.Deck.Count.ToString("00");
        _text_LeftCardsInDeckPlayer2.text = PlayerManager.Instance.Player2.Deck.Count.ToString("00");

        _text_CardsInHandPlayer1.text = PlayerManager.Instance.Player1.Hand.Count.ToString("00");
        _text_CardsInHandPlayer2.text = PlayerManager.Instance.Player2.Hand.Count.ToString("00");

        var p1Unit = PlayerManager.Instance.Player1.ActiveUnit;
        var p2Unit = PlayerManager.Instance.Player2.ActiveUnit;

        if (p1Unit == null)
        {
            _text_ActiveUnitPlayer1.text = string.Empty;
            _image_UnitP1.gameObject.SetActive(false);
        }
        else
        {
            _text_ActiveUnitPlayer1.text = p1Unit.Position == UnitCardPosition.Attack ? ((UnitCard)p1Unit.Prototype).Attack.ToString() : ((UnitCard)p1Unit.Prototype).Defense.ToString();
            _image_UnitP1.gameObject.SetActive(true);
            _image_UnitP1.gameObject.transform.rotation = Quaternion.Euler(0, 0, p1Unit.Position == UnitCardPosition.Attack ? 0 : -90);
        }
        if (p2Unit == null)
        {
            _text_ActiveUnitPlayer2.text = string.Empty;
            _image_UnitP2.gameObject.SetActive(false);
        }
        else
        {
            _text_ActiveUnitPlayer2.text = p2Unit.Position == UnitCardPosition.Attack ? ((UnitCard)p2Unit.Prototype).Attack.ToString() : ((UnitCard)p2Unit.Prototype).Defense.ToString();
            _image_UnitP2.gameObject.SetActive(true);
            _image_UnitP2.gameObject.transform.rotation = Quaternion.Euler(0, 0, p2Unit.Position == UnitCardPosition.Attack ? 0 : 90);
        }

        _text_TurnTime.text = CardGameManager.Instance.CurrentTurnTime.ToString("00");

        _text_CurrentPhase.text = (CardGameManager.Instance.CurrentPlayer == PlayerManager.Instance.Player1 ? "<" : "") + CardGameManager.Instance.CurrentPhase.ToString() + (CardGameManager.Instance.CurrentPlayer == PlayerManager.Instance.Player2 ? ">" : "");

        _slider_LpPlayer1.value = PlayerManager.Instance.Player1.LifePoints / 10f;
        _slider_LpPlayer2.value = PlayerManager.Instance.Player2.LifePoints / 10f;
    }

    public void SetWinLoseText(string text)
    {
        _text_WinLose.text = text;
    }
}
