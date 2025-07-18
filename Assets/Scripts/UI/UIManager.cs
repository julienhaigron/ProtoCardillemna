using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class UIManager : Singleton<UIManager>
{
	public static Action onFocusedWindowChanged;

	[SerializeField] private GameResultPopup m_gameResult;

    private static List<AUIPanel> m_panels;
    private static List<AUIPopup> m_popups;
	private Dictionary<Type, AUIPanel> panelsDictionary { get; set; }
	private List<Type> previousPanels { get; set; }
	private Dictionary<Type, AUIPopup> popupsDictionary { get; set; }
	public HashSet<AUIWindow> oppenedPanels = new();
	public AUIPopup activePopup { get; private set; }

	private AUIPopup m_currentPopup;
    public AUIPopup CurrentPopup => m_currentPopup;

	public override void Awake ()
	{
		base.Awake();

		m_panels = new List<AUIPanel>();
		m_popups = new List<AUIPopup>();

		AUIWindow[] windows = FindObjectsOfType<AUIWindow>(true);

		for (int i = 0; i < windows.Length; i++)
		{
			windows[i].WindowCanvas.enabled = false;
			windows[i].gameObject.SetActive(false);

			if (windows[i] is AUIPanel)
				m_panels.Add(windows[i] as AUIPanel);
			else if (windows[i] is AUIPopup)
				m_popups.Add(windows[i] as AUIPopup);
		}

		SetupPanels();
		SetupPopups();

		for (int i = 0; i < windows.Length; i++)
		{
			windows[i].OnLoad();
		}

	}

	public T OpenPopup<T> ( float _showDelay = 0f, bool _showInstant = false ) where T : AUIPopup
	{
		Type type = typeof(T);
		return (this.OpenPopup(type, _showDelay, _showInstant) as T);
	}

	public AUIPopup OpenPopup ( AUIPopup _popup, float _delay = 0f, bool _instant = false )
	{
		Type type = _popup.GetType();
		return (this.OpenPopup(type, _delay, _instant) as AUIPopup);
	}

	private AUIPopup OpenPopup ( Type _type, float _delay = 0f, bool _instant = false )
	{
		AUIPopup popup;

		if (this.popupsDictionary.ContainsKey(_type) == false)
			throw new Exception(this.GetType().Name + " - Do not have popup [" + _type.Name + "].");

		popup = this.popupsDictionary[_type];

		/*if (activePopup != null)
		{
			if (popup == activePopup && !popup.allowMultipleInSuccesivePopup)
			{
				Debug.LogError("Popup " + _type.Name + " already opened");
				return null;
			}
			else
			{
				if (!m_succesivePopups.Contains(popup) || popup.allowMultipleInSuccesivePopup)
				{
					m_succesivePopups.Enqueue(popup);
#if UNITY_EDITOR
					if (ApplicationManager.config.debug.showUIManagerLog)
						Debug.Log("AUIManager - Added in queue popup: " + _type.Name);
#endif
					return popup;
				}
				else
				{
					Debug.LogError("Popup " + _type.Name + "is already in queue");
					return null;
				}
			}
		}*/

		popup.SetCanvasEnable(true);
		popup.ShowWindow(_delay, _instant);
		SetActivePopup(popup, _instant);
		return (popup);
	}

	public void ClosePopup<T> ( float _delay = 0f, bool _instant = false ) where T : AUIPopup
	{
		Type type = typeof(T);
		this.ClosePopup(type, _delay, _instant);
	}

	private void ClosePopup ( Type _type, float _delay = 0f, bool _instant = false )
	{
		AUIPopup popup;

		if (this.popupsDictionary.ContainsKey(_type) == false)
			throw new Exception(this.GetType().Name + " - Do not have popup [" + _type.Name + "].");

		popup = this.popupsDictionary[_type];
		//if (popup.gameObject.activeSelf)
		popup.Close(_delay, _instant);
		//else
		//{
#if UNITY_EDITOR
		//if (m_showLog)
		//Debug.LogWarning("AUIManager - Popup [" + type.Name + "] is already closed.");
#endif
		//}

		if (this.activePopup != null && popup == this.activePopup)
			this.activePopup = null;
	}

	public T GetPopup<T> () where T : AUIPopup
	{
		Type type = typeof(T);

		if (this.popupsDictionary.ContainsKey(type) == false)
			throw new Exception(this.GetType().Name + " - Do not have popup [" + type.Name + "].");
		return (this.popupsDictionary[type] as T);
	}

	public void SetupPanels ()
	{
		this.panelsDictionary = new Dictionary<Type, AUIPanel>();
		this.previousPanels = new List<Type>();

		foreach (AUIPanel panel in m_panels)
		{
			if (panel != null)
			{
				this.panelsDictionary.Add(panel.GetType(), panel);
				panel.gameObject.SetActive(false);
			}
		}
	}
	
	public void SetupPopups ()
	{
		this.popupsDictionary = new Dictionary<Type, AUIPopup>();

		foreach (AUIPopup popup in m_popups)
		{
			if (popup != null)
			{
				this.popupsDictionary.Add(popup.GetType(), popup);
				popup.gameObject.SetActive(false);
			}
		}
	}

	private void SetActivePopup ( AUIPopup _popup, bool _instant )
	{
		if (!m_popups.Contains(_popup))
			return;

		if (this.activePopup == _popup)
			return;

		if (this.activePopup != null)
			this.activePopup.Close();

		this.activePopup = _popup;
		OnFocusedWindowChanged(_instant);
	}

	void OnFocusedWindowChanged ( bool _instant )
	{
		//Set right Currencies List
		//FocusedAUIWindow.topCurrencyVisibleList
		/*InputManager.inputArea.CancelInput();

		AdaptiveCurrencyListTopCanvasRef.ShowSpecificCurrency(FocusedAUIWindow.topCurrencyVisibleList, true, _instant: _instant);*/

		onFocusedWindowChanged?.Invoke();
	}
}
