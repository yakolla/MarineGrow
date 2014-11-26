using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Warehouse {

	List<ItemObject>	m_items = new List<ItemObject>();

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
}
