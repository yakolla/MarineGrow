using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joypad : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler 
{
	bool		m_dragging = false;
	Vector2		m_position;

	public void OnPointerDown(PointerEventData eventData)
	{
		dragging(eventData);
	}

	public void OnDrag (PointerEventData eventData)
	{
		dragging(eventData);
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		m_dragging = false;
	}

	void dragging(PointerEventData eventData)
	{
		Vector2 center = transform.position;
		m_position = (eventData.position-center).normalized;

		m_dragging = true;
	}

	public bool Dragging
	{
		get {return m_dragging;}
	}

	public Vector2 Position
	{
		get {return m_position;}
	}
}
