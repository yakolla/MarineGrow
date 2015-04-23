using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIInventorySlot : MonoBehaviour {

	YGUISystem.GUIImageStatic	m_item;

	YGUISystem.GUIPriceButton	m_priceButton0;
	YGUISystem.GUIPriceButton	m_priceButton1;

	public void Init(Texture icon, string desc)
	{
		m_item = new YGUISystem.GUIImageStatic(transform.Find("Icon").gameObject, icon);
		m_item.Text.Lable = desc;

		m_priceButton0 = new YGUISystem.GUIPriceButton(transform.Find("GUIPriceButton0").gameObject, ()=>{return true;});
		m_priceButton1 = new YGUISystem.GUIPriceButton(transform.Find("GUIPriceButton1").gameObject, ()=>{return true;});
	}

	public YGUISystem.GUIPriceButton PriceButton0
	{
		get{return m_priceButton0;}
	}

	public string ItemDesc
	{
		set{m_item.Text.Lable = value;}
	}

	public YGUISystem.GUIPriceButton PriceButton1
	{
		get{return m_priceButton1;}
	}

	void Update()
	{
		if (m_priceButton0 != null)
			m_priceButton0.Update();

		if (m_priceButton1 != null)
			m_priceButton1.Update();
	}
}


