using System.Collections.Generic;
using UnityEngine;
using System;

public class CardManager : MonoBehaviour
{
    public static Action<ActionCard> onUseCardEnd;
    
    [SerializeField] private CardHand m_hand;
    [SerializeField] private CardStack m_drawPile;
    [SerializeField] private CardStack m_discardPile;
    [SerializeField] private Transform m_cardInTargetSelectionPosition;
    [SerializeField] private CardStack m_rewardStack;

    [SerializeField] private int m_maxHandSize = 5;

    private ActionCard m_cardInTargetSelection;
    public ActionCard CardInTargetSelection => m_cardInTargetSelection;

    private List<EntityCard> m_cardTargets = new();

    public void Init ( PlayerConfig config )
    {
        // Crée la pioche initiale à partir du deck défini dans le PlayerConfig
        List<ActionCardData> initialDeck = new(config.playerData.deck); // méthode supposée
        List<ActionCard> cardDeck = new();
        for(int i = 0; i < config.playerData.deck.Count; i++)
		{
            ActionCard newCard = Instantiate(GameManager.Instance.baseActionCardPrefab, m_drawPile.transform.position, Quaternion.identity, m_hand.Parent);
            cardDeck.Add(newCard);
            newCard.Init(initialDeck[i]);
        }
        m_drawPile.Shuffle(cardDeck);

        //DrawNewHand();
    }

    public void DrawCard ()
    {
        if (m_hand.Count >= m_maxHandSize)
            return;

        if (m_drawPile.Count == 0)
        {
            ShuffleDiscardInDrawStack();
        }

        if (m_drawPile.Count == 0)
        {
            Debug.LogWarning("[CardManager] No cards left to draw.");
            return;
        }

        ActionCard card = m_drawPile.RemoveCardOnTop();
        if (card != null)
            m_hand.AddCard(card);
    }

    public void ShuffleDiscardInDrawStack ()
    {
        List<ActionCard> discarded = m_discardPile.ExtractAll();
        m_drawPile.Shuffle(discarded);
    }

    public void UseCard ( ActionCard card )
    {
        if (m_hand.RemoveCard(card))
        {
            m_discardPile.AddCard(card);
            m_discardPile.UpdateVisuals();

            if (m_hand.Count == 0)
            {
                DrawNewHand();
            }
        }

        m_cardTargets.Clear();
        m_cardInTargetSelection = null;
        onUseCardEnd?.Invoke(card);
    }

    public void DrawNewHand ()
    {
        List<ActionCard> discarded = m_hand.ExtractAll();
        m_discardPile.Shuffle(discarded);
        for (int i = 0; i < m_maxHandSize; i++)
        {
            DrawCard();
        }

        m_hand.UpdateVisuals();
    }

    public void EndGame ()
    {
        List<ActionCard> discarded = m_hand.ExtractAll();
        m_drawPile.Shuffle(discarded);
        m_drawPile.UpdateVisuals();
    }

    public void SetActionCardInSelection(ActionCard _card )
	{
        m_cardInTargetSelection = _card;
        _card.GoToPosition(m_cardInTargetSelectionPosition.position, m_cardInTargetSelectionPosition.rotation, 1f);
    }

    public void AddCardTarget(EntityCard _target )
	{
        m_cardTargets.Add(_target);

        if (m_cardInTargetSelection.Data.TargetNeededAmount() == m_cardTargets.Count)
		{
            m_cardInTargetSelection.Use(GameManager.Instance.TurnManager.Player, m_cardTargets);
            UseCard(m_cardInTargetSelection);
        }

    }

	#region BetweenGame

    public void DisplayReward (LevelRewardData _data)
	{
        foreach(ActionCardData cardData in _data.cards)
		{
            ActionCard card = Instantiate(GameManager.Instance.baseActionCardPrefab, m_rewardStack.transform.position, Quaternion.identity, m_hand.transform);
            card.Init(cardData);
            m_rewardStack.AddCard(card);
		}

        m_rewardStack.UpdateVisuals();
    }

    public void ChooseReward(ActionCard _reward )
	{
        m_rewardStack.RemoveCard(_reward);
        foreach(ActionCard card in m_rewardStack.ExtractAll())
		{
            Destroy(card.gameObject);
		}

        m_drawPile.AddCard(_reward);
        m_drawPile.UpdateVisuals();
        UIManager.Instance.ClosePopup<GameResultPopup>();
        GameManager.Instance.StartNextGame();
	}

	#endregion
}
