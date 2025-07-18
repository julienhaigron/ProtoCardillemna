using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using TMPro;

public class EntityCard : MonoBehaviour
{

	public static Action<EntityCard> onSpawn;
	public static Action<EntityCard> onDeath;

	[SerializeField] private InstancedRenderer m_instancedRenderer;
	[SerializeField] private TextMeshProUGUI m_hpTMP;
	[SerializeField] private TextMeshProUGUI m_actionsTMP;
	[SerializeField] private Vector3 m_entityPositionOffset;
	public Vector3 EntityPositionOffset => m_entityPositionOffset;

	private EntityData m_data;
	private int m_currentHP;
	public int CurrentHP => m_currentHP;
	public bool IsPlayer => m_data.isPlayer;

	private bool m_isAlive;
	public bool IsAlive => m_isAlive;
	private ARoom m_currentRoom;
	public ARoom CurrentRoom => m_currentRoom;

	private Dictionary<ActionCardData.ActionCardType, int> m_currentActionPoints = new();
	private Tween m_movementTween;

	private void OnDestroy ()
	{
		if (m_movementTween.IsActive())
			m_movementTween.Kill();
	}

	public void Spawn ( EntityData _data, ARoom _room )
	{
		m_data = _data;
		m_isAlive = true;
		m_currentRoom = _room;
		m_currentHP = _data.hp;

		m_currentActionPoints.Add(ActionCardData.ActionCardType.Attack, 0);
		m_currentActionPoints.Add(ActionCardData.ActionCardType.Movement, 0);
		m_currentActionPoints.Add(ActionCardData.ActionCardType.Special, 0);

		m_hpTMP.text = m_currentHP + "/" + _data.hp;
		m_actionsTMP.text = m_currentActionPoints[ActionCardData.ActionCardType.Attack] 
			+ "/" + m_currentActionPoints[ActionCardData.ActionCardType.Movement] 
			+ "/" + m_currentActionPoints[ActionCardData.ActionCardType.Special];
		m_instancedRenderer.SetColor("_Color", _data.isPlayer ? Color.blue : Color.red);

		onSpawn?.Invoke(this);
	}

	public void MoveTo ( ARoom _room )
	{
		if (m_movementTween.IsActive())
			m_movementTween.Kill();

		m_currentRoom.OnEnter(null);
		m_currentRoom = _room;
		m_movementTween = transform.DOMove(m_currentRoom.transform.position + m_entityPositionOffset, 1f).SetEase(Ease.Linear);
		m_currentRoom.OnEnter(this);
	}

	public void ResetActionPoints ()
	{
		m_currentActionPoints[ActionCardData.ActionCardType.Attack] = m_data.attackActionPoint;
		m_currentActionPoints[ActionCardData.ActionCardType.Movement] = m_data.movementActionPoint;
		m_currentActionPoints[ActionCardData.ActionCardType.Special] = m_data.specialActionPoint;

		m_actionsTMP.text = m_currentActionPoints[ActionCardData.ActionCardType.Attack]
			+ "/" + m_currentActionPoints[ActionCardData.ActionCardType.Movement]
			+ "/" + m_currentActionPoints[ActionCardData.ActionCardType.Special];
	}

	public void SpendActionPoint ( ActionCardData.ActionCardType _type )
	{
		m_currentActionPoints[_type]--;

		m_actionsTMP.text = m_currentActionPoints[ActionCardData.ActionCardType.Attack]
			+ "/" + m_currentActionPoints[ActionCardData.ActionCardType.Movement]
			+ "/" + m_currentActionPoints[ActionCardData.ActionCardType.Special];
	}

	public bool HasActionPointsToSpend ()
	{
		foreach (ActionCardData.ActionCardType type in m_currentActionPoints.Keys)
		{
			if (m_currentActionPoints[type] > 0)
				return true;
		}

		return false;
	}

	public void PlayTurn ()
	{
		if (IsPlayer)
			return;

		List<ActionCardData> tempCardList = new(m_data.deck);

		while (HasActionPointsToSpend())
		{
			int distanceToPlayer = GameManager.Instance.MapManager.GetDistance(GameManager.Instance.TurnManager.Player.CurrentRoom, CurrentRoom);
			if (distanceToPlayer > 1)
			{
				if (m_currentActionPoints[ActionCardData.ActionCardType.Movement] == 0)
					break;

				ActionCardData movementCard = null;
				foreach (ActionCardData card in tempCardList)
				{
					if (card.GetCardType() == ActionCardData.ActionCardType.Movement)
					{
						Vector2Int neighborPos = CurrentRoom.Position + (card as AMovementCardData).direction;
						ARoom neighbor = MapManager.Instance.GetRoom(neighborPos.x, neighborPos.y);

						if (neighbor != null && neighbor.EnterPredicate() && GameManager.Instance.MapManager.GetDistance(GameManager.Instance.TurnManager.Player.CurrentRoom, neighbor) < distanceToPlayer)
						{
							movementCard = card;
							MoveTo(neighbor);
							break;
						}
					}
				}

				if (movementCard != null)
				{
					tempCardList.Remove(movementCard);
					SpendActionPoint(ActionCardData.ActionCardType.Movement);
				}
				else
					break;
			}
			else
			{
				//attacks player if in range
				if (m_currentActionPoints[ActionCardData.ActionCardType.Attack] == 0)
					break;

				ActionCardData attackCard = null;
				foreach (ActionCardData card in tempCardList)
				{
					if (card.GetCardType() == ActionCardData.ActionCardType.Attack)
					{
						attackCard = card;
						break;
					}
				}

				if (attackCard != null)
				{
					GameManager.Instance.TurnManager.Player.TakeDamage(1);
					SpendActionPoint(ActionCardData.ActionCardType.Attack);
				}
				else
					break;
			}
		}
	}

	public void TakeDamage ( int _amount )
	{
		m_currentHP -= _amount;

		m_hpTMP.text = m_currentHP + "/" + m_data.hp;

		if (m_currentHP <= 0)
			Death();
	}

	public void Death ()
	{
		m_isAlive = false;

		onDeath?.Invoke(this);
		Destroy(gameObject);
	}
}
