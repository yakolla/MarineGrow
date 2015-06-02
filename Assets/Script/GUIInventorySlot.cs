using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIInventorySlot : MonoBehaviour {

	YGUISystem.GUIImageStatic	m_item;

	public class GUIPriceGemButton
	{
		public YGUISystem.GUIPriceButton	m_priceButton;
		public YGUISystem.GUIPriceButton	m_gemButton;
		public bool							m_enable;

		public GUIPriceGemButton(Transform transform, string buttonPath)
		{
			m_priceButton = new YGUISystem.GUIPriceButton(transform.Find(buttonPath).gameObject, Const.StartPosYOfPriceButtonImage, ()=>{return m_enable;});
			m_gemButton = new YGUISystem.GUIPriceButton(transform.Find(buttonPath + "/GemButton").gameObject, Const.StartPosYOfGemPriceButtonImage, ()=>{return m_enable;});
		}

		public void Update()
		{
			m_priceButton.Update();
			m_gemButton.Update();
		}

		public void RemoveAllListeners()
		{
			m_priceButton.GUIImageButton.Button.onClick.RemoveAllListeners();
			m_gemButton.GUIImageButton.Button.onClick.RemoveAllListeners();
		}

		public void AddListener(UnityEngine.Events.UnityAction callback, UnityEngine.Events.UnityAction gemCallback)
		{
			m_priceButton.GUIImageButton.Button.onClick.AddListener(callback);
			m_gemButton.GUIImageButton.Button.onClick.AddListener(gemCallback);
		}
		
		public void SetPrices(RefPrice[] normal, RefPrice[] gem)
		{
			m_priceButton.Prices = normal;
			m_gemButton.Prices = gem;
		}

		public void SetLable(string text)
		{
			m_priceButton.GUIImageButton.Lable.Text.text = text;
		}
	}

	GUIPriceGemButton	m_priceButton0;
	GUIPriceGemButton	m_priceButton1;

	public void Init(Texture icon, string desc)
	{
		m_item = new YGUISystem.GUIImageStatic(transform.Find("Icon").gameObject, icon);
		m_item.Lable.Text.text = desc;

		m_priceButton0 = new GUIPriceGemButton(transform, "GUIPriceButton0");
		m_priceButton1 = new GUIPriceGemButton(transform, "GUIPriceButton1");
	}

	public GUIPriceGemButton PriceButton0
	{
		get{return m_priceButton0;}
	}

	public string ItemDesc
	{
		set{m_item.Lable.Text.text = value;}
	}

	public GUIPriceGemButton PriceButton1
	{
		get{return m_priceButton1;}
	}

	void Update()
	{
		m_priceButton0.Update();
		m_priceButton1.Update();
	}


	
	public void StopSpinButton(int slot)
	{
		m_priceButton0.m_priceButton.GUIImageButton.Button.enabled = true;
		m_priceButton0.m_priceButton.GUIImageButton.Lable.Text.enabled = true;

		m_priceButton1.m_priceButton.GUIImageButton.Button.enabled = true;
		m_priceButton1.m_priceButton.GUIImageButton.Lable.Text.enabled = true;
	}
}



