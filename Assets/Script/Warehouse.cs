using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Warehouse {

	List<ItemObject>	m_items = new List<ItemObject>();
	int					m_gold = 0;

	static Warehouse m_ins = null;
	static public Warehouse Instance
	{
		get {
			if (m_ins == null)
			{
				m_ins = new Warehouse();
			}

			return m_ins;
		}
	}

	public void PushItem(ItemData item)
	{
		ItemObject itemObj = FindItemByItem(item);
		if (itemObj == null)
		{
			m_items.Add(new ItemObject(item));
		}
		else
		{
			itemObj.Item.Count += item.Count;
		}

	}

	public void RemoveItem(ItemObject obj)
	{
		m_items.Remove(obj);
	}

	public ItemObject FindItemByItemType(ItemData.Type type)
	{
		foreach(ItemObject obj in m_items)
		{
			if (obj.Item.RefItem.type == type)
			{
				return obj;
			}
		}

		return null;
	}

	public ItemObject FindItemByItem(ItemData item)
	{
		foreach(ItemObject obj in m_items)
		{
			if (obj.Item.Compare(item))
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
			m_items.Add(new ItemObject((ItemData)bf.Deserialize(file)));
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
