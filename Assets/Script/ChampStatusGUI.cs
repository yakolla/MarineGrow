using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChampStatusGUI : MonoBehaviour {

	Champ	m_champ;

	ChampSettingGUI	m_champSettingGUI = null;

	GameObject	m_guageGUI;
	GameObject	m_accessoryGUI;
	GameObject	m_specialGUI;


	YGUISystem.GUIButton[]	m_specialButtons = new YGUISystem.GUIButton[Const.SpecialButtons];
	YGUISystem.GUIButton[]	m_accessoryButtons = new YGUISystem.GUIButton[Const.AccessoriesSlots];
	YGUISystem.GUIGuage[] m_guages = new YGUISystem.GUIGuage[Const.Guages];

	void Start () {

		m_champSettingGUI = GameObject.Find("HudGUI/SettingGUI/Panel").GetComponent<ChampSettingGUI>();

		m_guageGUI = transform.Find("Guage").gameObject;
		m_accessoryGUI = transform.Find("Accessory").gameObject;
		m_specialGUI = transform.Find("Special").gameObject;

		m_specialButtons[0] = new YGUISystem.GUIButton(transform.Find("Special/Button0").gameObject, ()=>{
			m_specialButtons[0].Text.Lable = m_champ.RemainStatPoint.ToString();
			return m_champ.RemainStatPoint > 0;
		});
		m_specialButtons[1] = new YGUISystem.GUIButton(transform.Find("Special/Button1").gameObject, ()=>{
			m_specialButtons[1].Text.Lable = m_champ.ComboSkillStack.ToString();
			return m_champ.ComboSkillStack > 0;
		});
		m_specialButtons[2] = new YGUISystem.GUIButton(transform.Find("Special/Button2").gameObject, ()=>{
			m_specialButtons[2].Text.Lable = m_champ.DashSkillStack.ToString();
			return m_champ.DashSkillStack > 0;
		});

		m_accessoryButtons[0] = new YGUISystem.GUIButton(transform.Find("Accessory/Button0").gameObject, ()=>{
			return true;
		});
		m_accessoryButtons[1] = new YGUISystem.GUIButton(transform.Find("Accessory/Button1").gameObject, ()=>{
			return true;
		});
		m_accessoryButtons[2] = new YGUISystem.GUIButton(transform.Find("Accessory/Button2").gameObject, ()=>{
			return true;
		});
		m_accessoryButtons[3] = new YGUISystem.GUIButton(transform.Find("Accessory/Button3").gameObject, ()=>{
			return true;
		});

		m_guages[0] = new YGUISystem.GUIGuage(transform.Find("Guage/HP").gameObject, 
			()=>{return m_champ.m_creatureProperty.getHPRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxHP).ToString(); }
		);

		m_guages[1] = new YGUISystem.GUIGuage(transform.Find("Guage/XP").gameObject, 
			()=>{return m_champ.m_creatureProperty.getExpRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxExp).ToString();}
		);

		m_guages[2] = new YGUISystem.GUIGuage(transform.Find("Guage/SP").gameObject, 
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

		if (active == true)
		{
			for(int i = 0; i < Const.AccessoriesSlots; ++i)
			{
				if (m_champSettingGUI.EquipedAccessories[i] == null)
					continue;
				
				m_accessoryButtons[i].Icon.Image = m_champSettingGUI.EquipedAccessories[i].ItemIcon;
			}
		}

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
