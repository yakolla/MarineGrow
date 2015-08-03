using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GameJoystickOptionGUI : MonoBehaviour {

	ADMob					m_admob;
	Toggle					m_autoMoveToggle;
	Toggle					m_autoAttackToggle;

	System.Action			m_callback;

	void Start () {

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();

		m_autoMoveToggle = transform.Find("AutoMoveToggle").gameObject.GetComponent<Toggle>();
		m_autoAttackToggle = transform.Find("AutoAttackToggle").gameObject.GetComponent<Toggle>();
	}

	public void Init(System.Action callback)
	{
		m_callback = callback;
		gameObject.SetActive(true);
	}

	void OnEnable() {
		if (m_admob != null)
			m_admob.ShowBanner(true);

		TimeEffector.Instance.StopTime();
	}

	void OnDisable() {
		m_admob.ShowBanner(false);
		TimeEffector.Instance.StartTime();
	}

	public void OnToggleAutoAttack()
	{
		Warehouse.Instance.AutoAttack = m_autoAttackToggle.isOn;
	}

	public void OnToggleAutoMove()
	{
		Warehouse.Instance.AutoMove = m_autoMoveToggle.isOn;
	}

	public void OnClickOk()
	{
		m_callback.Invoke();
		gameObject.SetActive(false);
	}
}
