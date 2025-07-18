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
	[SerializeField] private List<MapData> m_levels;
	[SerializeField] private PlayerConfig m_thisPlayerConfig;
	public PlayerConfig PlayerConfig => m_thisPlayerConfig;

	[Title("Assets")]
	public EntityCard entityCard;
	public ActionCard baseActionCardPrefab;

	private GameState m_state;
	public GameState State => m_state;
	public enum GameState { Playing, Reward}

	private int m_gameDoneCount = 0;

	private void Start ()
	{
		m_cardManager.Init(m_thisPlayerConfig);
		StartNextGame();
	}

	private void SetState(GameState _state )
	{
		m_state = _state;
	}

	[Button]
	public void StartNextGame ()
	{
		SetState(GameState.Playing);
		m_mapManager.GenerateMap(m_levels[m_gameDoneCount]);

		m_turnManager.StartGame();
	}

	public void EndGame ( bool _didPlayerWin )
	{
		//display who wins
		SetState(GameState.Reward);
		m_cardManager.EndGame();
		UIManager.Instance.OpenPopup<GameResultPopup>().Init(_didPlayerWin);

		if (_didPlayerWin)
		{
			m_mapManager.DiscardMap();
			m_cardManager.DisplayReward(m_levels[m_gameDoneCount++].reward);
		}
	}
}
