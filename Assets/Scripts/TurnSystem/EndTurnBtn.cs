using UnityEngine;

public class EndTurnBtn : MonoBehaviour
{
    public void EndTurn ()
	{
		GameManager.Instance.TurnManager.EndPlayerTurn();
	}
}
