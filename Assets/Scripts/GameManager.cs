using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
	[Title("Depedencies")]
    [SerializeField] private CardManager m_cardManager;
	public CardManager CardManager => m_cardManager;
	[SerializeField] private MapManager m_mapManager;
	public MapManager MapManager => m_mapManager;
	[SerializeField] private TurnManager m_turnManager;
	public TurnManager TurnManager => m_turnManager;

	[Title("Parameters")]
	[SerializeField] private MapData m_thisLevel;
	[SerializeField] private PlayerConfig m_thisPlayerConfig;
	public PlayerConfig PlayerConfig => m_thisPlayerConfig;

	[Title("Assets")]
	public EntityCard entityCard;
	public ActionCard baseActionCardPrefab;


	private void Start ()
	{
		StartGame();
	}

	[Button]
    public void StartGame ()
	{
		m_mapManager.GenerateMap(m_thisLevel);
		m_cardManager.Init(m_thisPlayerConfig);

		m_turnManager.StartGame();
	}
}
