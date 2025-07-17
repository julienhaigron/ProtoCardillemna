using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	private List<EntityCard> m_enemies = new();
	private EntityCard m_player;
	public EntityCard Player => m_player;

	private void Awake ()
	{
		EntityCard.onSpawn += OnEntitySpawn;
		EntityCard.onDeath += OnEntityDeath;
		CardManager.onUseCardEnd += OnUseCardEnd;
	}

	private void OnEntitySpawn(EntityCard _entity )
	{
		if (_entity.IsPlayer)
			m_player = _entity;
		else
			m_enemies.Add(_entity);
	}

	private void OnEntityDeath(EntityCard _entity )
	{
		if (!_entity.IsPlayer)
			m_enemies.Remove(_entity);

		if (m_enemies.Count == 0)
			EndGame(true);
	}

	private void OnUseCardEnd (ActionCard _card)
	{
		m_player.SpendActionPoint(_card.Data.GetCardType());

		if (!m_player.HasActionPointsToSpend())
			EndPlayerTurn();
	}

    public void StartGame ()
	{
		NewTurn();
	}

	private void NewTurn ()
	{
		GameManager.Instance.CardManager.DrawNewHand();

		foreach(EntityCard entity in m_enemies)
		{
			entity.ResetActionPoints();
		}
		m_player.ResetActionPoints();
	}

	public void EndPlayerTurn ()
	{
		foreach(EntityCard card in m_enemies)
		{
			//TODO : put this in Coroutine to wait for action display
			card.PlayTurn();
		}

		if (m_player.IsAlive)
			NewTurn();
		else
			EndGame(false);
	}

	public void EndGame(bool _didPlayerWin )
	{
		//display who wins
		Debug.Log(_didPlayerWin ? "Player" : "Ennemi" + " wins");
	}

}
