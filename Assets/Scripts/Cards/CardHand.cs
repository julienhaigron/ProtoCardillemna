using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHand : CardStack
{

    private ActionCard m_hoveredCard;
    
    public bool RemoveCard(ActionCard _card )
	{
        return m_cards.Remove(_card);
	}

}
