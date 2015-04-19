using UnityEngine;
using UnityEngine.UI;

public class YGUISystem {

	public class GUIButton
	{
		Button	m_button;
		GUIText	m_text;
		System.Func<bool>	m_enableChecker;
		
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
}
