using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChampStatusGUI : MonoBehaviour {

	Champ	m_champ;

	ChampSettingGUI	m_champSettingGUI = null;

	GameObject	m_guageGUI;
	GameObject	m_accessoryGUI;
	GameObject	m_specialGUI;


	YGUISystem.GUIButton[]	m_specialButtons = new YGUISystem.GUIButton[3];
	YGUISystem.GUIGuage[] m_guages = new YGUISystem.GUIGuage[3];

	void Start () {

		m_champSettingGUI = GameObject.Find("ChampSettingGUI").GetComponent<ChampSettingGUI>();

		m_guageGUI = transform.Find("Guage").gameObject;
		m_accessoryGUI = transform.Find("Accessory").gameObject;
		m_specialGUI = transform.Find("Special").gameObject;

		m_specialButtons[0] = new YGUISystem.GUIButton(gameObject, "Special/Button0", ()=>{
			return m_champ.RemainStatPoint > 0;
		});
		m_specialButtons[1] = new YGUISystem.GUIButton(gameObject, "Special/Button1", ()=>{
			return m_champ.ComboSkillStack > 0;
		});
		m_specialButtons[2] = new YGUISystem.GUIButton(gameObject, "Special/Button2", ()=>{
			return true;
		});

		m_guages[0] = new YGUISystem.GUIGuage(gameObject, "Guage/HP", 
			()=>{return m_champ.m_creatureProperty.getHPRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxHP).ToString(); }
		);

		m_guages[1] = new YGUISystem.GUIGuage(gameObject, "Guage/XP", 
			()=>{return m_champ.m_creatureProperty.getExpRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxExp).ToString();}
		);

		m_guages[2] = new YGUISystem.GUIGuage(gameObject, "Guage/SP", 
			()=>{return 1f;}, 
			()=>{return "Weapon Enegy"; }
		);
	}

	public void OnClickLevelUp()
	{
		if (m_champ.RemainStatPoint == 0)
			return;

		GameObject.Find("HudGUI/AbilityGUI").transform.Find("Panel").gameObject.SetActive(true);
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

		foreach(YGUISystem.GUIButton button in m_specialButtons)
		{
			button.Update();
		}

		foreach(YGUISystem.GUIGuage guage in m_guages)
		{
			guage.Update();
		}
	}

}
