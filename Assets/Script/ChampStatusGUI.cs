using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChampStatusGUI : MonoBehaviour {

	Champ	m_champ;

	ChampSettingGUI	m_champSettingGUI = null;

	GameObject	m_guageGUI;
	GameObject	m_accessoryGUI;
	GameObject	m_specialGUI;

	class GUIButton
	{
		Button	m_button;
		Text	m_text;
		System.Func<bool>	m_enableChecker;

		public GUIButton(GameObject obj, string name, System.Func<bool> enableChecker)
		{
			m_button = obj.transform.Find(name).GetComponent<Button>();
			m_text = obj.transform.Find(name + "/Text").GetComponent<Text>();
			m_enableChecker = enableChecker;
		}

		public void Update()
		{
			m_button.gameObject.SetActive( m_enableChecker() );
		}
	}

	class GUIGuage
	{
		Image	m_guage;
		Text	m_text;
		System.Func<float>	m_fillAmountGetter;
		System.Func<string>	m_lableGetter;

		public GUIGuage(GameObject obj, string name, System.Func<float>	fillAmountGetter, System.Func<string> lableGetter)
		{
			m_guage = obj.transform.Find(name).GetComponent<Image>();
			m_text = obj.transform.Find(name + "/Text").GetComponent<Text>();
			m_fillAmountGetter = fillAmountGetter;
			m_lableGetter = lableGetter;
		}

		public void Update()
		{
			m_guage.fillAmount = m_fillAmountGetter();
			m_text.text = m_lableGetter();
		}
	}

	GUIButton[]	m_specialButtons = new GUIButton[3];
	GUIGuage[] m_guages = new GUIGuage[3];

	void Start () {

		m_guageGUI = transform.Find("Guage").gameObject;
		m_accessoryGUI = transform.Find("Accessory").gameObject;
		m_specialGUI = transform.Find("Special").gameObject;

		m_specialButtons[0] = new GUIButton(gameObject, "Special/Button0", ()=>{
			return m_champ.RemainStatPoint > 0;
		});
		m_specialButtons[1] = new GUIButton(gameObject, "Special/Button1", ()=>{
			return m_champ.ComboSkillStack > 0;
		});
		m_specialButtons[2] = new GUIButton(gameObject, "Special/Button2", ()=>{
			return true;
		});

		m_guages[0] = new GUIGuage(gameObject, "Guage/HP", 
			()=>{return m_champ.m_creatureProperty.getHPRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxHP).ToString(); }
		);

		m_guages[1] = new GUIGuage(gameObject, "Guage/XP", 
		                           ()=>{return m_champ.m_creatureProperty.getExpRemainRatio();}, 
		()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxExp).ToString();}
		);

		m_guages[2] = new GUIGuage(gameObject, "Guage/SP", 
		                           ()=>{return 1f;}, 
		()=>{return "Weapon Enegy"; }
		);
	}

	public void OnClickLevelUp()
	{
		if (m_champ.RemainStatPoint == 0)
			return;

		ChampAbilityGUI abilityGUI = m_champ.transform.Find("ChampAbilityGUI").GetComponent<ChampAbilityGUI>();
		abilityGUI.gameObject.SetActive(true);
	}

	public void OnClickComboSkill()
	{
		if (m_champ.ComboSkillStack == 0)
			return;

		--m_champ.ComboSkillStack;
		m_champ.ApplyBuff(null, DamageDesc.BuffType.Combo100, 10f, null);
	}

	public void OnClickAccessory(int slot)
	{
		if (m_champSettingGUI.EquipedAccessories[slot] == null)
			return;

		m_champSettingGUI.EquipedAccessories[slot].Item.Use(m_champ);
	}

	public void OnClickDashSkill()
	{
		if (m_champ.MoveDir == Vector3.zero)
			return;

		DamageDesc desc  = new DamageDesc(0, DamageDesc.Type.Normal, DamageDesc.BuffType.Dash, Resources.Load<GameObject>("Pref/ef_dash"));
		desc.Dir = m_champ.MoveDir;
		m_champ.ApplyBuff(null, DamageDesc.BuffType.Dash, 0.5f, desc);
	}

	void SetActiveGUI(bool active)
	{
		m_guageGUI.SetActive(active);
		m_accessoryGUI.SetActive(active);
		m_specialGUI.SetActive(active);
	}

	void OnGUI()
	{		
		if (m_champ == null)
		{
			GameObject obj = GameObject.Find("Champ");
			if (obj == null)
			{
				SetActiveGUI(false);
				return;
			}

			m_champ = obj.GetComponent<Champ>();
			SetActiveGUI(true);
			return;
		}

		foreach(GUIButton button in m_specialButtons)
		{
			button.Update();
		}

		foreach(GUIGuage guage in m_guages)
		{
			guage.Update();
		}
	}

}
