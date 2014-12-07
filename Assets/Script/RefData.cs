using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class RefBaseData
{
	public int 				id;
}

public class RefItem : RefBaseData
{
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	public 	ItemData.Type 	type;
	public 	string 			codeName;
	public	string			icon;

}

public class RefItemOptionSpawn
{	
	[JsonConverter(typeof(StringEnumConverter))]
	public ItemData.Option  type;
	public int 				minValue;
	public int 				maxValue;
	[Range(0F, 1F)]
	public float			ratio;
	
}

public class RefItemSpawn
{
	public int					refItemId;
	public int 					minValue;
	public int 					maxValue;
	[Range(0F, 1F)]
	public float				ratio;
	public RefItemOptionSpawn[] itemOptionSpawns;

	[JsonIgnore]
	public RefItem				refItem;
}

public class RefMobSpawn : RefBaseData
{
	public int 					mobCount;
	public float 				interval;	
	public string				prefEnemy;
	public RefItemSpawn[]		refItemSpawns;
}

public class RefWorldMap : RefBaseData
{
	public string			name;
	public int[]			refMobSpawnIds;

	[JsonIgnore]
	public Dictionary<int, RefMobSpawn>		refMobSpawns = new Dictionary<int, RefMobSpawn>();
}

public class RefData {


	Dictionary<int, RefWorldMap>	m_refWorldMaps = new Dictionary<int, RefWorldMap>();
	Dictionary<int, RefMobSpawn>	m_refMobSpawns = new Dictionary<int, RefMobSpawn>();
	Dictionary<int, RefItemSpawn>	m_refItemSpawns = new Dictionary<int, RefItemSpawn>();
	Dictionary<int, RefItem>		m_refItems = new Dictionary<int, RefItem>();

	static RefData m_ins = null;
	static public RefData Instance
	{
		get {
		
			if (m_ins == null)
			{
				m_ins = new RefData();
				m_ins.Load();
			}
			
			return m_ins;
		}

	}

	void Load()
	{
		Deserialize(m_refWorldMaps, "RefWorldMap.txt");
		Deserialize(m_refMobSpawns, "RefMobSpawn.txt");
		Deserialize(m_refItems, "RefItem.txt");

		foreach(KeyValuePair<int, RefWorldMap> pair in m_refWorldMaps)
		{
			foreach(int id in pair.Value.refMobSpawnIds)
			{
				pair.Value.refMobSpawns[id] = m_refMobSpawns[id];

				foreach(RefItemSpawn itemSpawn in m_refMobSpawns[id].refItemSpawns)
				{
					itemSpawn.refItem = m_refItems[itemSpawn.refItemId];
				}
			}
		}
	}

	void Deserialize<T>(Dictionary<int, T> records, string fileName) where T : RefBaseData
	{ 
		string filePath = Application.dataPath + "/RefData/" + fileName;
		if(File.Exists(filePath)) {
			T[] datas = JsonConvert.DeserializeObject<T[]>(File.ReadAllText(filePath));				
			foreach(T data in datas)
			{
				records[data.id] = data;
			}
		}
	}

	public Dictionary<int, RefWorldMap> RefWorldMaps
	{
		get {return m_refWorldMaps;}
	}

	public Dictionary<int, RefItem> RefItems
	{
		get {return m_refItems;}
	}

}
