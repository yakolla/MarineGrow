using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Warehouse {

	List<ItemObject>	m_items = new List<ItemObject>();

	ItemObject			m_gold = new ItemObject(new ItemGoldData(0));
	ItemObject			m_gem	= new ItemObject(new ItemGemData(0));

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
		ItemObject itemObj = FindItem(item.RefItem.id);
		if (itemObj == null)
		{
			m_items.Add(new ItemObject(item));
		}
		else
		{
			switch(itemObj.Item.RefItem.type)
			{
			case ItemData.Type.Follower:
			case ItemData.Type.Weapon:
			case ItemData.Type.Gold:
			case ItemData.Type.Gem:
				return;
			}
			itemObj.Item.Count += item.Count;
			itemObj.Item.Count = Mathf.Min(itemObj.Item.Count, 999);
		}
	}

	public void PullItem(int refItemId, int count)
	{
		ItemObject itemObj = FindItem(refItemId);
		if (itemObj == null)
		{
			return;
		}

		if (count <= itemObj.Item.Count)
		{
			itemObj.Item.Count -= count;
			if (itemObj.Item.Count == 0)
			{
				switch(itemObj.Item.RefItem.type)
				{
				case ItemData.Type.Gold:
				case ItemData.Type.Gem:
					return;
				}
				RemoveItem(itemObj);
			}
		}

	}

	public void RemoveItem(ItemObject obj)
	{
		m_items.Remove(obj);
	}

	public ItemObject FindItem(int refItemId)
	{
		switch(refItemId)
		{
		case 1:
			return m_gold;
		case 8:
			return m_gem;
		}

		foreach(ItemObject obj in m_items)
		{
			if (obj.Item.RefItem.id == refItemId)
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

	public ItemObject Gold
	{
		get { return m_gold; }
	}

	public ItemObject Gem
	{
		get { return m_gem; }
	}
}
