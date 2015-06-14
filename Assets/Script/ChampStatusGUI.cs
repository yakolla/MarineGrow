using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChampStatusGUI : MonoBehaviour {

	Champ	m_champ;



	int			m_oldMobKills;
	int			m_oldGold;

	YGUISystem.GUIButton[]	m_specialButtons = new YGUISystem.GUIButton[Const.SpecialButtons];
	YGUISystem.GUIChargeButton[]	m_accessoryButtons = new YGUISystem.GUIChargeButton[Const.AccessoriesSlots];
	YGUISystem.GUIGuage[] m_guages = new YGUISystem.GUIGuage[Const.Guages];
	ComboGUIShake	m_gold;
	ComboGUIShake	m_mobKills;

	void Start () {


		m_gold = transform.Find("Gold/RawImage/Text").gameObject.GetComponent<ComboGUIShake>();
		m_mobKills = transform.Find("Kills/RawImage/Text").gameObject.GetComponent<ComboGUIShake>();

		m_specialButtons[0] = new YGUISystem.GUIButton(transform.Find("Special/Button0").gameObject, ()=>{
			m_specialButtons[0].Lable.Text.text = m_champ.RemainStatPoint.ToString();
			return m_champ.RemainStatPoint > 0;
		});
		m_specialButtons[1] = new YGUISystem.GUIButton(transform.Find("Special/Button1").gameObject, ()=>{
			m_specialButtons[1].Lable.Text.text = m_champ.MachoSkillStack.ToString();
			return m_champ.MachoSkillStack > 0;
		});
		m_specialButtons[2] = new YGUISystem.GUIButton(transform.Find("Special/Button2").gameObject, ()=>{
			m_specialButtons[2].Lable.Text.text = m_champ.NuclearSkillStack.ToString();
			return m_champ.NuclearSkillStack > 0;
		});

		for(int i = 0; i < m_accessoryButtons.Length; ++i)
		{
			m_accessoryButtons[i] = new YGUISystem.GUIChargeButton(transform.Find("Accessory/Button"+i).gameObject, ()=>{
				return true;
			});
		}

		m_guages[0] = new YGUISystem.GUIGuage(transform.Find("Guage/HP").gameObject, 
			()=>{return m_champ.m_creatureProperty.getHPRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxHP).ToString(); 
			}
		);

		m_guages[1] = new YGUISystem.GUIGuage(transform.Find("Guage/XP").gameObject, 
			()=>{return m_champ.m_creatureProperty.getExpRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxExp).ToString();
			}
		);

		m_guages[2] = new YGUISystem.GUIGuage(transform.Find("Guage/SP").gameObject, 
		    ()=>{return m_champ.m_creatureProperty.getSPRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.SP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxSP).ToString();
			}
		);
	}

	public void OnClickLevelUp()
	{
		if (m_champ.RemainStatPoint == 0)
			return;

		GameObject.Find("HudGUI/AbilityGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	public void OnClickOption()
	{
		GameObject.Find("HudGUI/OptionGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	public void OnClickComboSkill()
	{
		if (m_champ.MachoSkillStack == 0)
			return;

		if (true == m_champ.ApplyBuff(null, DamageDesc.BuffType.Macho, 10f, null))
			--m_champ.MachoSkillStack;

	}

	public void OnClickAccessory(int slot)
	{
		if (m_champ.AccessoryItems[slot] == null)
			return;

		if (m_accessoryButtons[slot].ChargingPoint == 0)
			return;

		if (m_champ.AccessoryItems[slot].Item.Usable(m_champ) == false)
			return;

		--m_accessoryButtons[slot].ChargingPoint;
		m_champ.AccessoryItems[slot].Item.Use(m_champ);

	}

	public void OnClickDashSkill()
	{
		if (m_champ.NuclearSkillStack == 0)
			return;

		--m_champ.NuclearSkillStack;

		m_champ.WeaponHolder.ActiveWeaponSkillFire(m_champ.WeaponHolder.MainWeapon.WeaponStat.skillId, transform.eulerAngles.y);
	}

	void SetActiveGUI(bool active)
	{
		transform.Find("Guage").gameObject.SetActive(active);
		transform.Find("Accessory").gameObject.SetActive(active);
		transform.Find("Special").gameObject.SetActive(active);
		transform.Find("Option").gameObject.SetActive(active);
		transform.Find("Gold").gameObject.SetActive(active);
		transform.Find("Kills").gameObject.SetActive(active);

		if (active == true)
		{
			for(int i = 0; i < Const.AccessoriesSlots; ++i)
			{
				if (m_champ.AccessoryItems[i] == null)
					continue;
				
				m_accessoryButtons[i].Icon.Image = m_champ.AccessoryItems[i].ItemIcon;
				m_accessoryButtons[i].MaxChargingPoint = 2;
				m_accessoryButtons[i].ChargingPoint = 2;
				m_accessoryButtons[i].CoolDownTime = m_champ.AccessoryItems[i].Item.RefItem.weaponStat.coolTime;
			}
		}

	}

	void Update()
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
			m_oldMobKills = m_champ.MobKills;
			m_oldGold = Warehouse.Instance.Gold.Item.Count;
			SetActiveGUI(true);
			return;
		}

		if (m_oldMobKills != m_champ.MobKills)
		{
			m_oldMobKills = m_champ.MobKills;
			m_mobKills.enabled = true;
			m_mobKills.shake = 2f;
			m_mobKills.Text = m_champ.MobKills.ToString();
		}

		if (m_oldGold != Warehouse.Instance.Gold.Item.Count)
		{
			m_oldGold = Warehouse.Instance.Gold.Item.Count;
			m_gold.enabled = true;
			m_gold.shake = 2f;
			m_gold.Text = Warehouse.Instance.Gold.Item.Count.ToString();
		}


		foreach(YGUISystem.GUIButton button in m_specialButtons)
		{
			button.Update();
		}

		foreach(YGUISystem.GUIButton button in m_accessoryButtons)
		{
			button.Update();
		}

		foreach(YGUISystem.GUIGuage guage in m_guages)
		{
			guage.Update();
		}
	}

}
