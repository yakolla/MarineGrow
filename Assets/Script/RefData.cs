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

public class RefPrice
{
	public int				refItemId;
	public int				count;
}


public class RefPriceCondition
{
	public RefPrice[]		conds;
	public RefPrice[]		else_conds;
}


public class RefItem : RefBaseData
{
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	public 	ItemData.Type 		type;
	public 	string 				codeName;
	public	string				icon;
	public	RefPriceCondition	levelup;
	public	RefPriceCondition	evolution;
	public 	RefPriceCondition	unlock;
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

public class RefMobClass
{
	public RefMob[]				melee;
	public RefMob[]				range;
	public RefMob[]				boss;
	public RefMob[]				shuttle;
	public RefMob[]				egg;
	public RefMob[]				itemPandora;
}

public class RefMobSpawnRatio
{
	public class Desc
	{
		public float[]	ratio;
		public int[]	count;
	}
	public Desc	melee;
	public Desc	range;
	public Desc	boss;
	public Desc	shuttle;
}

public class RefMobSpawn
{
	public RefMobSpawnRatio		refMobIds;	
	public float 			interval;
	public int[]			repeatCount;
	public int[]			mobCount;
	public bool				boss = false;
	public RefItemSpawn[]	refDropItems;

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
	RefMobClass		m_refMobClass = new RefMobClass();
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
		DeserializeArray(m_refWorldMaps, "RefWorldMap");
		Deserialize(ref m_refMobClass, "RefMob");
		DeserializeArray(m_refItems, "RefItem");

		foreach(KeyValuePair<int, RefWorldMap> pair in m_refWorldMaps)
		{
			foreach(RefWave wave in pair.Value.waves)
			{
				foreach(RefMobSpawn mobSpawn in wave.mobSpawns)
				{

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
		RefMob[][] mobs= {m_refMobClass.melee, m_refMobClass.range, m_refMobClass.boss, m_refMobClass.egg, m_refMobClass.shuttle};

		foreach(RefMob[] refMobs in mobs)
		{
			foreach(RefMob refMob in refMobs)
			{
				if (refMob.eggMob != null)
				{
					foreach(RefMob egg in m_refMobClass.egg)
					{
						if (refMob.eggMob.refMobId==egg.id)
						{
							refMob.eggMob.refMob = egg;
							break;
						}

					}

				}
			}

		}

	}

	void DeserializeArray<T>(Dictionary<int, T> records, string fileName) where T : RefBaseData
	{ 
		TextAsset textDocument =  Resources.Load("RefData/" + fileName) as TextAsset;

		T[] datas = JsonConvert.DeserializeObject<T[]>(textDocument.text);				
		foreach(T data in datas)
		{
			records[data.id] = data;
		}

	}

	void Deserialize<T>(ref T records, string fileName)
	{ 
		TextAsset textDocument =  Resources.Load("RefData/" + fileName) as TextAsset;

		records = JsonConvert.DeserializeObject<T>(textDocument.text);			

		
	}

	public Dictionary<int, RefWorldMap> RefWorldMaps
	{
		get {return m_refWorldMaps;}
	}

	public Dictionary<int, RefItem> RefItems
	{
		get {return m_refItems;}
	}

	public RefMob[] RefMeleeMobs
	{
		get {return m_refMobClass.melee;}
	}

	public RefMob[] RefRangeMobs
	{
		get {return m_refMobClass.range;}
	}

	public RefMob[] RefBossMobs
	{
		get {return m_refMobClass.boss;}
	}

	public RefMob[] RefShuttleMobs
	{
		get {return m_refMobClass.shuttle;}
	}

	public RefMob[] RefItemPandoraMobs
	{
		get {return m_refMobClass.itemPandora;}
	}
}
