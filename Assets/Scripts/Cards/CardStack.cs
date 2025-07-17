using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour
{
    public enum StackType { Discard, Deck, Hand}
    public StackType type;

    protected List<ActionCard> m_cards = new();

    [SerializeField] private bool m_isFacingDown = true;
    [SerializeField] private Transform m_parent;
    public Transform Parent => m_parent;
    [SerializeField] protected Vector3 m_cardOffset;
    [SerializeField] private Vector3 m_firstCardPosition;

    public int Count => m_cards.Count;

    public void AddCard ( ActionCard card )
    {
        card.SetStack(this);
        m_cards.Add(card);
    }

    public ActionCard RemoveCardOnTop ()
    {
        if (m_cards.Count == 0)
        {
            Debug.LogWarning("[CardStack] Tried to remove card from empty stack.");
            return null;
        }

        ActionCard card = m_cards[^1];
        m_cards.Remove(card);

        return card;
    }

    public List<ActionCard> ExtractAll ()
    {
        var cards = new List<ActionCard>(m_cards);
        m_cards.Clear();
        return cards;
    }

    public void Shuffle ( List<ActionCard> cards )
    {
        foreach (var card in cards)
        {
            m_cards.Add(card);
        }

        var count = m_cards.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = m_cards[i];
            m_cards[i] = m_cards[r];
            m_cards[r] = tmp;
        }

        UpdateVisuals();
    }

    public void UpdateVisuals ()
    {
        //update cards in stack position, first at bottom position
        int cardCount = 0;
        foreach(ActionCard card in m_cards)
		{
            card.GoToPosition(transform.position + m_firstCardPosition + (cardCount++ * m_cardOffset), Quaternion.identity, 1f);
        }
    }

}