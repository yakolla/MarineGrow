using UnityEngine;
using System.Collections;

public class ItemObject {


	Texture			m_icon;
	Item			m_item;

	public ItemObject(Item item)
	{
		m_item = item;
		m_icon = Resources.Load<Texture>(item.ItemIcon);
	}

	public Item Item
	{
		get {return m_item;}
	}

	public Texture ItemIcon
	{
		get { return m_icon; }
	}

}
