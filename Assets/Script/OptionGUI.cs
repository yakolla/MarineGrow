using UnityEngine;
using UnityEngine.UI;

public class OptionGUI : MonoBehaviour {

	ADMob					m_admob;
	Slider					m_sfxVolume;
	Slider					m_bgmVolume;
	Toggle					m_autoTarget;

	void Start () {

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();

		m_admob.ShowBanner(true);

		m_sfxVolume = transform.Find("SFX/Slider").gameObject.GetComponent<Slider>();
		m_bgmVolume = transform.Find("BGM/Slider").gameObject.GetComponent<Slider>();
		m_autoTarget = transform.Find("Auto Target").gameObject.GetComponent<Toggle>();

		m_sfxVolume.value = Warehouse.Instance.GameOptions.m_sfxVolume;
		m_bgmVolume.value = Warehouse.Instance.GameOptions.m_bgmVolume;
		m_autoTarget.isOn = Warehouse.Instance.GameOptions.m_autoTarget;


	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	public void OnSliderBGM()
	{
		GameObject champObj = GameObject.Find("Champ");
		if (champObj != null)
		{
			champObj.audio.volume = m_bgmVolume.value;
		}
		Warehouse.Instance.GameOptions.m_bgmVolume = m_bgmVolume.value;
	}

	public void OnSliderSFX()
	{
		m_sfxVolume.audio.Play();
		AudioListener.volume = m_sfxVolume.value;
		Warehouse.Instance.GameOptions.m_sfxVolume = m_sfxVolume.value;
	}

	public void OnToggleAutoTarget()
	{
		Warehouse.Instance.GameOptions.m_autoTarget = m_autoTarget.isOn;
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Options", "Autotarget"+m_autoTarget.isOn, 0);
	}

	public void OnClickOk()
	{
		GameObject champObj = GameObject.Find("Champ");
		if (champObj != null)
		{
			champObj.GetComponent<Champ>().ApplyGameOptions();
		}

		m_admob.ShowBanner(false);
		gameObject.SetActive(false);

	}

	public void OnClickTitle()
	{
		m_admob.ShowBanner(false);
		Application.LoadLevel("Worldmap");
	}

	public void OnClickRate()
	{
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Options", "Rate", 0);
		Application.OpenURL ("market://details?id=com.banegole.marinegrowing");
	}

}
