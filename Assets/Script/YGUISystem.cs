using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class YGUISystem {

	public class GUIButton
	{
		protected Button		m_button;

		protected GUIText		m_text;
		protected System.Func<bool>	m_enableChecker;
		
		public GUIButton(GameObject obj, string name, System.Func<bool> enableChecker)
		{
			m_button = obj.transform.Find(name).GetComponent<Button>();
			m_text = new GUIText(obj, name + "/Text");
			m_enableChecker = enableChecker;

		}
		
		public void Update()
		{
			m_button.gameObject.SetActive( m_enableChecker() );
		}

		public GUIText Text
		{
			get{return m_text;}
		}
	}

	public class GUIImageButton : GUIButton
	{
		List<GUIImage>	m_images = new List<GUIImage>();

		public GUIImageButton(GameObject obj, string name, System.Func<bool> enableChecker)
			: base(obj, name, enableChecker)
		{

		}

		public void AddGUIImage(Texture icon)
		{
			m_images.Add(new GUIImage(m_button.gameObject, icon));

			Vector3 pos = Vector3.zero;
			const int gap = 20;
			int startX = -gap*(m_images.Count-1);
			foreach(GUIImage image in m_images)
			{
				pos.Set(startX, -gap, 0);
				image.Position = pos;
				startX+=gap*2;
			}
		}

		public List<GUIImage> GUIImages
		{
			get{return m_images;}
		}
	}

	public class GUIPriceButton
	{
		GUIImageButton	m_button;
		RefPrice[]		m_normalPrices;
		RefPrice[]		m_specialPrices;
		float			m_normalWorth = 1f;
		float			m_specialWorth = 1f;

		public GUIPriceButton(GameObject obj, string name, System.Func<bool> enableChecker, RefPrice[] normalPrices, RefPrice[] specialPrices)
		{
			m_button = new GUIImageButton(obj, name, enableChecker);
			m_normalPrices = normalPrices;
			m_specialPrices = specialPrices;

			foreach(RefPrice price in m_normalPrices)
			{
				RefItem condRefItem = RefData.Instance.RefItems[price.refItemId];
				
				m_button.AddGUIImage(Resources.Load<Texture>(condRefItem.icon));				
			}
		}

		void displayPrice(GUIImageButton button, RefPrice[] prices, float itemWorth)
		{
			int priceIndex = 0;
			foreach(RefPrice price in prices)
			{
				RefItem condRefItem = RefData.Instance.RefItems[price.refItemId];
				
				string str = "<color=black>";
				int cost = (int)(price.count*itemWorth);
				
				ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId, null);
				int hasCount = 0;
				if (inventoryItemObj == null)
				{
					str = "<color=red>";
				}
				else if (inventoryItemObj != null)
				{
					if (inventoryItemObj.Item.Count < cost)
					{
						str = "<color=red>";
					}
					hasCount = inventoryItemObj.Item.Count;
				}
				str += hasCount;
				str += "/" + cost;
				str += "</color>";
				button.GUIImages[priceIndex].Text.Lable = str;
				
				++priceIndex;
			}
		}

		public float NormalWorth
		{
			set{m_normalWorth = value;}
		}

		public void Update()
		{
			m_button.Update();
			displayPrice(m_button, m_normalPrices, m_normalWorth);
		}

		public bool TryToNormalPay()
		{
			if (Const.CheckAvailableItem(m_normalPrices, m_normalWorth))
			{
				Const.PayPriceItem(m_normalPrices, m_normalWorth);
				return true;
			}

			return false;
		}
	}
	
	public class GUIGuage
	{
		Image	m_guage;
		GUIText	m_text;
		System.Func<float>	m_fillAmountGetter;
		System.Func<string>	m_lableGetter;
		
		public GUIGuage(GameObject obj, string name, System.Func<float>	fillAmountGetter, System.Func<string> lableGetter)
		{
			m_guage = obj.transform.Find(name).GetComponent<Image>();
			m_text = new GUIText(obj, name + "/Text");
			m_fillAmountGetter = fillAmountGetter;
			m_lableGetter = lableGetter;
		}
		
		public void Update()
		{
			m_guage.fillAmount = m_fillAmountGetter();
			m_text.Lable = m_lableGetter();
		}
	}

	public class GUIText
	{
		Text	m_text;

		public GUIText(GameObject obj, string name)
		{
			m_text = obj.transform.Find(name).GetComponent<Text>();
		}

		public string Lable
		{
			set{m_text.text = value;}
		}
	}

	public class GUIImage
	{
		RawImage	m_image;
		GUIText		m_text;

		public GUIImage(GameObject obj, Texture icon)
		{
			GameObject guiImageObj = GameObject.Instantiate(Resources.Load<GameObject>("Pref/GUIImage")) as GameObject;
			m_image = guiImageObj.GetComponent<RawImage>();
			m_image.texture = icon;

			m_text = new GUIText(guiImageObj, "Text");

			guiImageObj.transform.parent = obj.transform;
			guiImageObj.transform.localPosition = Vector3.zero;
		}
		
		public void Update()
		{

		}
		
		public GUIText Text
		{
			get{return m_text;}
		}

		public Vector3 Position
		{
			set{m_image.gameObject.transform.localPosition = value;}
		}
	}
}
