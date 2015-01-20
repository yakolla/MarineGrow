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

[System.Serializable]
public class RefCreatureBaseProperty
{
	public float	maxHP;
	public float	physicalDamage;

	[Range (0, 100)]
	public float	physicalDefence;
	public float	evolutionPerLevel;
	public int		exp;
	public float 	phyDamagePerLevel;
	public float 	phyDefencePerLevel;
	public float 	hpPerLevel;
	[Range (0, 100)]
	public float	moveSpeed;
	public float	navMeshBaseOffset;
}

public class RefProgressUpItem
{
	public int				refItemId;
	public int				count;
}

public class RefEvolutionFiring
{
	public float			angle;
	public float			delay;
}

public class RefWeapon
{
	public float			coolTime;
	public float			range;
}

public class RefItem : RefBaseData
{
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	public 	ItemData.Type 		type;
	public 	string 				codeName;
	public	string				icon;
	public	List<RefProgressUpItem>	levelUpItems = new List<RefProgressUpItem>();
	public	List<RefProgressUpItem>	evolutionItems = new List<RefProgressUpItem>();
	public 	RefEvolutionFiring	evolutionFiring;
	public  RefWeapon			weapon;
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

public class RefEggMob
{
	public int					refMobId;
	public int					count;

	[JsonIgnore]
	public RefMob				refMob;
}

public class RefFollowerMob
{
	public int					refMobId;
	public int					count;
	
	[JsonIgnore]
	public RefMob				refMob;
}

public enum MobAIType
{
	Normal,
	Rotation,
	Dash,
	Revolution,
	ItemShuttle,
}

public class RefMob : RefBaseData
{
	public string				prefBody;
	public int					refWeaponItem;
	public bool					nearByChampOnSpawn;
	public RefEggMob			eggMob;
	public RefFollowerMob		followerMob;
	public RefCreatureBaseProperty		baseCreatureProperty;

	[JsonConverter(typeof(StringEnumConverter))]
	public MobAIType			mobAI;
}

public class RefMobSpawn
{
	public int[]			refMobIds;	
	public float 			interval;
	public int[]			repeatCount;
	public int[]			mobCount;
	public bool				boss = false;
	public RefItemSpawn[]	refDropItems;

	[JsonIgnore]
	public Dictionary<int, RefMob>		refMobs = new Dictionary<int, RefMob>();
}

public class RefWave
{
	public RefMobSpawn[]	mobSpawns;
	public RefItemSpawn[]	refDropItems;
}

public class RefWorldMap : RefBaseData
{
	public string			name;
	public RefWave[]		waves;

}

public class RefData {


	Dictionary<int, RefWorldMap>	m_refWorldMaps = new Dictionary<int, RefWorldMap>();
	Dictionary<int, RefMob>			m_refMobs = new Dictionary<int, RefMob>();
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
		Deserialize(m_refMobs, "RefMob.txt");
		Deserialize(m_refItems, "RefItem.txt");

		foreach(KeyValuePair<int, RefWorldMap> pair in m_refWorldMaps)
		{
			foreach(RefWave wave in pair.Value.waves)
			{
				foreach(RefMobSpawn mobSpawn in wave.mobSpawns)
				{
					foreach(int id in mobSpawn.refMobIds)
					{
						mobSpawn.refMobs[id] = m_refMobs[id];
					}

					if (mobSpawn.refDropItems != null)
					{
						foreach(RefItemSpawn itemSpawn in mobSpawn.refDropItems)
						{
							itemSpawn.refItem = m_refItems[itemSpawn.refItemId];
						}
					}

				}

				if (wave.refDropItems != null)
				{
					foreach(RefItemSpawn itemSpawn in wave.refDropItems)
					{
						itemSpawn.refItem = m_refItems[itemSpawn.refItemId];
					}
				}
			}
		}

		foreach(KeyValuePair<int, RefMob> pair in m_refMobs)
		{
			if (pair.Value.eggMob != null)
			{
				pair.Value.eggMob.refMob = m_refMobs[pair.Value.eggMob.refMobId];
			}
		}

		foreach(KeyValuePair<int, RefMob> pair in m_refMobs)
		{
			if (pair.Value.followerMob != null)
			{
				pair.Value.followerMob.refMob = m_refMobs[pair.Value.followerMob.refMobId];
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

	public Dictionary<int, RefMob> RefMobs
	{
		get {return m_refMobs;}
	}
}
