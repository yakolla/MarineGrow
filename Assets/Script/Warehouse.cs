using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;



public class Warehouse {

	class WarehouseData
	{
		static public MemoryStream Serialize(Warehouse obj)
		{

			MemoryStream stream = new MemoryStream();

			StreamWriter writer = new StreamWriter(stream);
			writer.WriteLine(JsonConvert.SerializeObject(obj.m_version));
			writer.WriteLine(JsonConvert.SerializeObject(obj.m_items.Count));
			foreach(ItemObject itemObj in obj.m_items)
			{
				writer.WriteLine(JsonConvert.SerializeObject(itemObj.Item.RefItem.type));
				//Debug.Log(JsonConvert.SerializeObject(itemObj.Item));
				writer.WriteLine(JsonConvert.SerializeObject(itemObj.Item));
			}

			writer.WriteLine(JsonConvert.SerializeObject(obj.m_waveIndex));
			writer.WriteLine(JsonConvert.SerializeObject(obj.m_gold.Item));
			writer.WriteLine(JsonConvert.SerializeObject(obj.m_gem.Item));

			writer.WriteLine(JsonConvert.SerializeObject(obj.m_gameBestStats));
			writer.WriteLine(JsonConvert.SerializeObject(obj.m_options));

			writer.Close();

			return stream;
		}

		static public void Deserialize(Warehouse obj, byte[] data)
		{
			obj.m_items.Clear();

			MemoryStream stream = new MemoryStream(data);
			
			StreamReader reader = new StreamReader(stream);

			int version = JsonConvert.DeserializeObject<int>(reader.ReadLine());
			int count = JsonConvert.DeserializeObject<int>(reader.ReadLine());

			for(int i = 0; i < count; ++i)
			{
				ItemData.Type type = JsonConvert.DeserializeObject<ItemData.Type>(reader.ReadLine());

				switch(type)
				{
				case ItemData.Type.Weapon:
					ItemWeaponData weaponData = JsonConvert.DeserializeObject<ItemWeaponData>(reader.ReadLine());
					obj.m_items.Add(new ItemObject(weaponData));
					break;
				case ItemData.Type.GoldMedal:
					ItemGoldMedalData goldMedalData = JsonConvert.DeserializeObject<ItemGoldMedalData>(reader.ReadLine());
					obj.m_items.Add(new ItemObject(goldMedalData));
					break;
				case ItemData.Type.WeaponDNA:
					ItemWeaponDNAData weaponDNAData = JsonConvert.DeserializeObject<ItemWeaponDNAData>(reader.ReadLine());
					obj.m_items.Add(new ItemObject(weaponDNAData));
					break;
				case ItemData.Type.WeaponParts:
					ItemWeaponPartsData weaponPartsData = JsonConvert.DeserializeObject<ItemWeaponPartsData>(reader.ReadLine());
					obj.m_items.Add(new ItemObject(weaponPartsData));
					break;
				case ItemData.Type.Follower:
					ItemFollowerData followerData = JsonConvert.DeserializeObject<ItemFollowerData>(reader.ReadLine());
					obj.m_items.Add(new ItemObject(followerData));
					break;
				case ItemData.Type.Accessory:
					ItemAccessoryData accessoryData = JsonConvert.DeserializeObject<ItemAccessoryData>(reader.ReadLine());
					obj.m_items.Add(new ItemObject(accessoryData));
					break;
				case ItemData.Type.Skill:
					ItemSkillData skillData = JsonConvert.DeserializeObject<ItemSkillData>(reader.ReadLine());
					obj.m_items.Add(new ItemObject(skillData));
					break;
				case ItemData.Type.Cheat:
					ItemCheatData cheatData = JsonConvert.DeserializeObject<ItemCheatData>(reader.ReadLine());
					obj.m_items.Add(new ItemObject(cheatData));
					break;

				default:
					Debug.Log(type);
					reader.ReadLine();
					break;
				}
			}

			int waveIndex = JsonConvert.DeserializeObject<int>(reader.ReadLine());
			obj.m_waveIndex = waveIndex;

			ItemGoldData goldData = JsonConvert.DeserializeObject<ItemGoldData>(reader.ReadLine());
			obj.m_gold = new ItemObject(goldData);

			ItemGemData gemData = JsonConvert.DeserializeObject<ItemGemData>(reader.ReadLine());
			obj.m_gem = new ItemObject(gemData);

			obj.m_gameBestStats = JsonConvert.DeserializeObject<GameStatistics>(reader.ReadLine());
			obj.m_options = JsonConvert.DeserializeObject<Options>(reader.ReadLine());
			
			reader.Close();
		}

	}

	int					m_version = 1;
	string				m_fileName;
	List<ItemObject>	m_items = new List<ItemObject>();

	ItemObject			m_gold = new ItemObject(new ItemGoldData(0));
	ItemObject			m_gem	= new ItemObject(new ItemGemData(0));
	int					m_waveIndex = 0;

	public class 	GameStatistics
	{
		SecuredType.XFloat		m_survivalTime = 0f;
		SecuredType.XInt		m_gainedGold = 0;
		SecuredType.XInt		m_gainedXP = 0;
		SecuredType.XInt		m_killedMobs = 0;

		public void SetBestStats(GameStatistics newStats)
		{
			if (SurvivalTime < newStats.SurvivalTime)
				SurvivalTime = newStats.SurvivalTime;

			GainedGold = Mathf.Max(GainedGold, newStats.GainedGold);
			GainedXP = Mathf.Max(GainedXP, newStats.GainedXP);
			KilledMobs = Mathf.Max(KilledMobs, newStats.KilledMobs);

		}

		public float SurvivalTime
		{
			set{m_survivalTime.Value = value;}
			get{return m_survivalTime.Value;}
		}

		public int GainedGold
		{
			set{m_gainedGold.Value = value;}
			get{return m_gainedGold.Value;}
		}

		public int GainedXP
		{
			set{m_gainedXP.Value = value;}
			get{return m_gainedXP.Value;}
		}

		public int KilledMobs
		{
			set{m_killedMobs.Value = value;}
			get{return m_killedMobs.Value;}
		}
	}

	public class Options
	{
		public float	m_sfxVolume = 1f;	
		public float	m_bgmVolume = 0.5f;
		public bool		m_autoTarget = false;
	}

	GameStatistics			m_gameBestStats = new GameStatistics();
	GameStatistics			m_newGameStats = new GameStatistics();
	Options					m_options = new Options();

	float					m_playTime = 0f;
	float					m_saveTime = 0f;
	public float PlayTime
	{
		get { 
			if (m_playTime == 0f)
				return 0f;

			return Time.time-m_playTime; 
		}
		set { m_playTime = value; }
	}

	public float SaveTime
	{
		get { 
			if (m_saveTime == 0f)
				return 0f;
			
			return Time.time-m_saveTime; 
		}
		set { m_saveTime = value; }
	}

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

	public void Reset()
	{
		m_items = new List<ItemObject>();
		
		m_gold = new ItemObject(new ItemGoldData(0));
		m_gem	= new ItemObject(new ItemGemData(0));
		m_waveIndex = 0;

		m_gameBestStats = new GameStatistics();
	}

	public void ResetNewGameStats()
	{	
		m_gameBestStats.SetBestStats(m_newGameStats);
		m_newGameStats = new GameStatistics();	
		Warehouse.Instance.PlayTime = Time.time;
		Warehouse.Instance.SaveTime = Time.time;
	}

	public void PushItem(ItemData item)
	{
		ItemObject itemObj = FindItem(item.RefItem.id, item);
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

	public void PullItem(ItemObject itemObj, int count)
	{
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

	public ItemObject FindItem(int refItemId, ItemData item)
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
			if (refItemId == 1001)
			{
				if (obj.Item.Compare(item))
					return obj;
			}
			else
			{
				if (obj.Item.RefItem.id == refItemId)
				{
					return obj;
				}
			}
		}

		return null;
	}


	public byte[] Serialize()
	{
		MemoryStream stream = WarehouseData.Serialize(this);

		return stream.ToArray();
	}

	public void Deserialize(byte[] data)
	{
		WarehouseData.Deserialize(this, data);
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

	public int CheatLevel
	{
		get {
			ItemObject cheatItem = FindItem(Const.CheatRefItemId, null);
			if (cheatItem.Item.Lock == true)
				return 0;

			return cheatItem.Item.Level;
		}
	}

	public int WaveIndex
	{
		get {return m_waveIndex;}
		set {m_waveIndex = value;}
	}

	public string FileName
	{
		get {return m_fileName;}
		set {m_fileName = value;}
	}

	public GameStatistics GameBestStats
	{
		get {return m_gameBestStats;}
	}

	public GameStatistics NewGameStats
	{
		get {return m_newGameStats;}
	}

	public Options GameOptions
	{
		get {return m_options;}
	}
}
