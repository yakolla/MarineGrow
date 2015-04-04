using UnityEngine;
using System.Collections;

public class ComboGUIShake : MonoBehaviour
{
	// How long the object should shake for.
	public float shake = 0f;

	[SerializeField]
	float decreaseFactor = 10.0f;
	
	Vector3 originalPos;

	TypogenicText	m_killComboGUI;

	void Start()
	{
		m_killComboGUI = GetComponent<TypogenicText>();
	}
	
	void OnEnable()
	{
		originalPos = transform.localPosition;
	}
	
	void Update()
	{
		if (shake > 1)
		{
			transform.localScale = Vector3.one*shake;
			
			shake -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shake = 0f;
			transform.localScale = Vector3.one;
			this.enabled = false;
		}
	}

	public string Text
	{
		get{return m_killComboGUI.Text;}
		set{m_killComboGUI.Text = value;}
	}
}