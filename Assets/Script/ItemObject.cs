using UnityEngine;
using System.Collections;

public class ItemObject {


	Texture			m_icon;
	ItemData			m_item;

	public ItemObject(ItemData item)
	{
		m_item = item;
		m_icon = Resources.Load<Texture>(item.RefItem.icon);
	}

	public ItemData Item
	{
		get {return m_item;}
	}

	public Texture ItemIcon
	{
		get { return m_icon; }
	}

}
