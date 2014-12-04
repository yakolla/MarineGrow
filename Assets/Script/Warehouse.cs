using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Warehouse {

	List<ItemObject>	m_items = new List<ItemObject>();
	int					m_gold = 0;

	static Warehouse m_wareHouse = null;
	static public Warehouse Instance()
	{
		if (m_wareHouse == null)
		{
			m_wareHouse = new Warehouse();
		}

		return m_wareHouse;
	}

	public void PushItem(Item item)
	{
		m_items.Add(new ItemObject(item));
	}

	public void RemoveItem(ItemObject obj)
	{
		m_items.Remove(obj);
	}

	public ItemObject FindItemByItemType(Item.Type type)
	{
		foreach(ItemObject obj in m_items)
		{
			if (obj.Item.ItemType == type)
			{
				return obj;
			}
		}

		return null;
	}

	public void Save(BinaryFormatter bf, FileStream file)
	{
		bf.Serialize(file, m_items.Count);
		foreach(ItemObject obj in m_items)
		{
			bf.Serialize(file, obj.Item);
		}
	}

	public void Load(BinaryFormatter bf, FileStream file)
	{
		int length = (int)bf.Deserialize(file);
		for(int i = 0; i < length;  ++i)
		{
			m_items.Add(new ItemObject((Item)bf.Deserialize(file)));
		}
	}

	public List<ItemObject> Items
	{
		get {return m_items;}
	}

	public int Gold
	{
		get { return m_gold; }
		set { m_gold = value; }
	}
}
