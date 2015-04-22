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

		m_priceButton0 = new YGUISystem.GUIPriceButton(gameObject, "GUIPriceButton0", ()=>{return true;}, null, null);
		m_priceButton1 = new YGUISystem.GUIPriceButton(gameObject, "GUIPriceButton1", ()=>{return true;}, null, null);
	}

	public YGUISystem.GUIPriceButton PriceButton0
	{
		get{return m_priceButton0;}
	}

	public YGUISystem.GUIPriceButton PriceButton1
	{
		get{return m_priceButton1;}
	}
}


