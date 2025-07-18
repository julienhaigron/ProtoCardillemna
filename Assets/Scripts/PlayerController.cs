using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerController : MonoBehaviour
{
	[Title("Depedencies")]
	[SerializeField] private Camera m_camera;
	[SerializeField] private LayerMask m_boardLayer;

	[Title("Parameters")]
	[SerializeField] private float m_movementSpeed = 0.2f;
	[SerializeField] private float m_movementSmooth = 5f;
	[SerializeField] private Vector2 m_boundaries;

	private EntityCard currentHoveredEntity = null;
	private ActionCard currentHoveredCard = null;
	public Vector3 targetCamPos;

	private void Update ()
	{
		HandleHover();
		HandleClick();

		if (Input.GetMouseButton(1))
		{
			HandleSwipeMovement();
		}
	}

	private void HandleClick ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (GameManager.Instance.State == GameManager.GameState.Playing)
			{
				if (currentHoveredEntity != null && GameManager.Instance.CardManager.CardInTargetSelection != null
					&& GameManager.Instance.CardManager.CardInTargetSelection.Data.TargetPredicate(GameManager.Instance.TurnManager.Player, currentHoveredEntity))
				{
					GameManager.Instance.CardManager.AddCardTarget(currentHoveredEntity);
				}

				else if (currentHoveredCard != null && currentHoveredCard.CanUse())
				{
					if (currentHoveredCard.Data.TargetNeededAmount() > 0)
						GameManager.Instance.CardManager.SetActionCardInSelection(currentHoveredCard);
					else
					{
						currentHoveredCard.Use(GameManager.Instance.TurnManager.Player, null);
						GameManager.Instance.CardManager.UseCard(currentHoveredCard);
					}
				}

				else if (Physics.Raycast(m_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
				{
					EndTurnBtn endTurnBtn = hit.collider.transform.GetComponent<EndTurnBtn>();
					if (endTurnBtn != null)
						endTurnBtn.EndTurn();
				}
			}
			else if (GameManager.Instance.State == GameManager.GameState.Reward)
			{
				if (currentHoveredCard != null)
				{
					GameManager.Instance.CardManager.ChooseReward(currentHoveredCard);
				}
			}
		}
	}

	private void HandleSwipeMovement ()
	{
		if (Input.mousePositionDelta != Vector3.zero)
			targetCamPos = ((GameManager.Instance.MapManager.RoomParent.up * Input.mousePositionDelta.y + GameManager.Instance.MapManager.RoomParent.right * Input.mousePositionDelta.x) * m_movementSpeed) + GameManager.Instance.MapManager.RoomParent.position;

		GameManager.Instance.MapManager.RoomParent.position = Vector3.Lerp(GameManager.Instance.MapManager.RoomParent.position, targetCamPos, m_movementSmooth * Time.unscaledDeltaTime);
		GameManager.Instance.MapManager.RoomParent.localPosition = new Vector3(Mathf.Clamp(GameManager.Instance.MapManager.RoomParent.localPosition.x, -m_boundaries.x, m_boundaries.x), Mathf.Clamp(GameManager.Instance.MapManager.RoomParent.localPosition.y, -m_boundaries.y, m_boundaries.y), 0f);
	}

	private void HandleHover ()
	{
		Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			ActionCard hoveredCard = hit.collider.transform.parent.parent.GetComponent<ActionCard>();
			EntityCard hoveredEntity = hoveredCard == null ? hit.collider.transform.parent.parent.GetComponent<EntityCard>() : null;

			if (hoveredCard != null)
			{
				if (currentHoveredCard != hoveredCard)
				{
					if (currentHoveredCard != null)
						currentHoveredCard.OnHoverExit();

					currentHoveredCard = hoveredCard;
					currentHoveredCard.OnHoverEnter();
				}
			}
			else if (currentHoveredCard != null)
			{
				currentHoveredCard.OnHoverExit();
				currentHoveredCard = null;
			}

			if (hoveredEntity != null)
			{
				currentHoveredEntity = hoveredEntity;
			}

			return;
		}

		if (currentHoveredCard != null)
		{
			currentHoveredCard.OnHoverExit();
			currentHoveredCard = null;
		}
		if (currentHoveredCard != null)
		{
			currentHoveredCard = null;
		}
	}
}
