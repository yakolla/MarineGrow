using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class Spawn : MonoBehaviour {



	[SerializeField]
	Champ		m_champ = null;

	[SerializeField]
	Transform[]		m_areas = null;

	[SerializeField]
	GameObject		m_prefSpawnEffect = null;
	
	GameObject[]	m_prefItemBoxSkins = new GameObject[(int)ItemData.Type.Count];
	GameObject		m_prefItemBox;

	float		m_creationTime = 0;

	float			m_effectBulletTime = 0f;

	FollowingCamera	m_followingCamera = null;

	int				m_mobsOfCheckOnDeath = 0;

	RefWorldMap		m_refWorldMap;
	Dungeon			m_dungeon;
	int				m_hive = 0;

	ComboGUIShake	m_comboGUIShake;
	GoldGUISmooth	m_goldGUIShake;

	DropShip		m_dropShip;

	BoxCollider		m_edgeRect;

	[SerializeField]
	int				m_wave = 0;
	// Use this for initialization
	void Start () {

		m_dropShip = transform.parent.Find("dropship").GetComponent<DropShip>();
		m_edgeRect = transform.Find("EdgeRect").GetComponent<BoxCollider>();

		m_followingCamera = Camera.main.GetComponent<FollowingCamera>();

		m_comboGUIShake = Camera.main.gameObject.transform.Find("KillCombo").gameObject.GetComponent<ComboGUIShake>();
		m_goldGUIShake = Camera.main.gameObject.transform.Find("Gold").gameObject.GetComponent<GoldGUISmooth>();


		m_dungeon = transform.parent.GetComponent<Dungeon>();
		int dungeonId = m_dungeon.DungeonId;
		m_refWorldMap = RefData.Instance.RefWorldMaps[dungeonId];

		m_prefItemBox = Resources.Load<GameObject>("Pref/ItemBox/ItemBox");
		string[] itemTypeNames = Enum.GetNames(typeof(ItemData.Type));
		for(int i = 0; i < itemTypeNames.Length-1; ++i)
		{
			m_prefItemBoxSkins[i] = Resources.Load<GameObject>("Pref/ItemBox/item_" + itemTypeNames[i] + "_skin");
			if (m_prefItemBoxSkins[i] == null)
				Debug.Log("Pref/ItemBox/item_" + itemTypeNames[i] + "_skin");
		}

		guiText.pixelOffset = new Vector2(Screen.width/2, -Screen.height/4);
		m_areas = transform.GetComponentsInChildren<Transform>();

	}

	IEnumerator EffectWaveText(string msg, float alpha)
	{
		guiText.text = msg;
		Color color = guiText.color;
		color.a = alpha;
		guiText.color = color;
		yield return new WaitForSeconds (0.2f);

		if (alpha > 0)
		{
			StartCoroutine(EffectWaveText(msg, alpha-0.1f));
		}

	}

	public RefWave	GetCurrentWave()
	{
		return m_refWorldMap.waves[m_hive];
	}

	public void StartWave(int wave, Champ champ)
	{
		if (m_wave == 0)
			m_wave = wave*GetCurrentWave().mobSpawns.Length;

		m_goldGUIShake.Gold = Warehouse.Instance.Gold.Item.Count;
		m_creationTime = Time.time;

		m_dropShip.SetChamp(champ);
		m_dropShip.GetComponent<Animator>().SetTrigger("Move");
		StartCoroutine(spawnMobPer());
	}

	bool checkBossAlive()
	{
		return m_mobsOfCheckOnDeath > 0;
	}

	public float CreationTime
	{
		get {return m_creationTime;}
	}

	Transform	getSpawnArea(bool champAreaExcept)
	{
		return m_areas[Random.Range(1,m_areas.Length)];
	}

	class SpawnMobDescResult
	{
		public List<RefMob>	spawnMobs = new List<RefMob>();
		public List<bool> 	spawnMobBoss = new List<bool>();
		public List<int> 	spawnMobCount = new List<int>();
		public List<bool> 	spawnMobMonitored = new List<bool>();
		public List<MobSpawnEffectType> spawnEffectType = new List<MobSpawnEffectType>();
	}

	void	buildSpawnMob(SpawnMobDescResult result, float progress, RefMobSpawnRatio.Desc spawnRatioDesc, RefMob[] mobs, bool monitoredDeath, bool boss)
	{
		if (spawnRatioDesc == null)
			return;

		if (spawnRatioDesc.chance < Random.Range(0f, 1f))
			return;

		int minIndex = 0;
		int maxIndex = Mathf.Min((int)progress, mobs.Length-1);
		Debug.Log("min:" + minIndex + ", max:" + maxIndex + ", progress:" + progress);
		minIndex = Mathf.Clamp(minIndex, 0, mobs.Length-1);

		if (boss == true && progress < 3f)
		{
			result.spawnMobs.Add(mobs[maxIndex]);
		}
		else
		{
			result.spawnMobs.Add(mobs[Random.Range(minIndex, maxIndex+1)]);
		}


		minIndex = (int)(spawnRatioDesc.count[0]);
		maxIndex = (int)(spawnRatioDesc.count[0] * (1f-progress*0.1f) + spawnRatioDesc.count[1] * progress*0.1f);
		result.spawnMobCount.Add(Random.Range(minIndex, maxIndex));
		result.spawnMobMonitored.Add(monitoredDeath);
		result.spawnMobBoss.Add(boss);
		result.spawnEffectType.Add(spawnRatioDesc.spawnEffectType);
	}

	public int GetStage(int wave)
	{
		return wave/GetCurrentWave().mobSpawns.Length + 1;
	}

	IEnumerator spawnMobPerCore(RefMobSpawn mobSpawn, float waveProgress)
	{
		int mobSpawnRepeatCount = (int)(mobSpawn.repeatCount[0] * (1f-waveProgress*0.1f) + mobSpawn.repeatCount[1] * waveProgress * 0.1f);
		for(int r = 0; r < mobSpawnRepeatCount; ++r)
		{
			yield return new WaitForSeconds (mobSpawn.interval);
			
			if (m_champ == null)
			{
				break;
			}
			
			SpawnMobDescResult spawnMobDescResult = new SpawnMobDescResult();
			
			buildSpawnMob(spawnMobDescResult, waveProgress, mobSpawn.refMobIds.melee, RefData.Instance.RefMeleeMobs, true, false);
			buildSpawnMob(spawnMobDescResult, waveProgress, mobSpawn.refMobIds.range, RefData.Instance.RefRangeMobs, true, false);
			buildSpawnMob(spawnMobDescResult, waveProgress, mobSpawn.refMobIds.boss, RefData.Instance.RefBossMobs, true, true);
			buildSpawnMob(spawnMobDescResult, waveProgress, mobSpawn.refMobIds.itemPandora, RefData.Instance.RefItemPandoraMobs, false, false);
			buildSpawnMob(spawnMobDescResult, waveProgress, mobSpawn.refMobIds.itemDummy, RefData.Instance.RefItemDummyMobs, false, false);
			buildSpawnMob(spawnMobDescResult, waveProgress, mobSpawn.refMobIds.miniBoss, RefData.Instance.RefMiniBossMobs, true, false);
			
			if (Random.Range(0, 2) == 0)
			{
				Transform area = getSpawnArea(true);
				Vector3 cp = m_champ.transform.position;
				Vector3 scale = area.localScale*0.5f;
				
				for(int ii = 0;  ii < spawnMobDescResult.spawnMobs.Count; ++ii)
				{
					for(int i = 0; i < spawnMobDescResult.spawnMobCount[ii]; ++i)
					{
						
						RefMob refMob = spawnMobDescResult.spawnMobs[ii];
						Vector3 enemyPos = cp;
						float angle = Random.Range(0f, 3.14f*2);
						enemyPos.x += Mathf.Cos(angle) * 5f;
						enemyPos.z += Mathf.Sin(angle) * 5f;							
						
						yield return new WaitForSeconds (0.02f);
						
						Creature cre = SpawnMob(refMob, enemyPos, spawnMobDescResult.spawnMobBoss[ii], spawnMobDescResult.spawnMobMonitored[ii]);
						cre.gameObject.SetActive(false);
						
						switch(spawnMobDescResult.spawnEffectType[ii])
						{
						case MobSpawnEffectType.Falling:
							StartCoroutine(  EffectSpawnMob1(enemyPos, cre) );
							break;
						default:
							StartCoroutine(  EffectSpawnMob(enemyPos, cre) );
							break;
						}
						
						
					}
				}
			}
			else
			{
				for(int ii = 0;  ii < spawnMobDescResult.spawnMobs.Count; ++ii)
				{
					for(int i = 0; i < spawnMobDescResult.spawnMobCount[ii]; ++i)
					{
						Transform area = getSpawnArea(true);
						Vector3 cp = area.position;
						Vector3 scale = area.localScale*0.5f;
						
						RefMob refMob = spawnMobDescResult.spawnMobs[ii];
						if (refMob.nearByChampOnSpawn == true)
						{
							if (m_champ)
							{
								cp = m_champ.transform.position;
							}
							
						}
						else
						{
							cp = area.position;
						}
						
						Vector3 enemyPos = cp;
						enemyPos.x += Random.Range(-scale.x,scale.x);
						enemyPos.z += Random.Range(-scale.z,scale.z);
						
						RefItemSpawn[] dropItems = null;
						if (GetCurrentWave().itemSpawn.mapMobItems.ContainsKey(refMob.id))
						{
							dropItems = GetCurrentWave().itemSpawn.mapMobItems[refMob.id].refDropItems;
						}
						else
						{
							dropItems = GetCurrentWave().itemSpawn.defaultItem;
						}
						
						yield return new WaitForSeconds (0.02f);
						
						Creature cre = SpawnMob(refMob, enemyPos, spawnMobDescResult.spawnMobBoss[ii], spawnMobDescResult.spawnMobMonitored[ii]);
						cre.gameObject.SetActive(false);
						
						switch(spawnMobDescResult.spawnEffectType[ii])
						{
						case MobSpawnEffectType.Falling:
							StartCoroutine(  EffectSpawnMob1(enemyPos, cre) );
							break;
						default:
							StartCoroutine(  EffectSpawnMob(enemyPos, cre) );
							break;
						}					
						
					}
				}
			}
			
			
		}
	}

	IEnumerator spawnMobPer()
	{
		while(true)
		{
			if (m_champ == null)
			{
				yield return null;
			}
			else
			{
				
				RefMobSpawn mobSpawn = GetCurrentWave().mobSpawns[m_wave%GetCurrentWave().mobSpawns.Length];

				yield return new WaitForSeconds(mobSpawn.interval);

				Warehouse.Instance.WaveIndex = m_wave;
				
				float waveProgress = ProgressStage();
				Debug.Log("waveProgress:" + waveProgress + "," + m_wave);

				if (mobSpawn.boss == true)
				{
					StartCoroutine(EffectWaveText("Boss", 3));
					m_champ.ShakeCamera(3f);
				}
				else
				{
					if (m_wave % GetCurrentWave().mobSpawns.Length == 0)
					{
						StartCoroutine(EffectWaveText("Stage " + GetStage(m_wave), 3));
					}
				}

				yield return StartCoroutine(spawnMobPerCore(mobSpawn, waveProgress));


				while(checkBossAlive())
				{
					yield return new WaitForSeconds(0.5f);
				}

				if (Application.platform == RuntimePlatform.Android)
				{
					GPlusPlatform.Instance.OpenGame(Warehouse.Instance.FileName, (SavedGameRequestStatus status, ISavedGameMetadata game)=>{
						if (status == SavedGameRequestStatus.Success) 
						{
							System.TimeSpan totalPlayingTime = game.TotalTimePlayed;
							totalPlayingTime += new System.TimeSpan(System.TimeSpan.TicksPerSecond*(long)(Time.time-CreationTime));					
							
							GPlusPlatform.Instance.SaveGame(game, Warehouse.Instance.Serialize(), totalPlayingTime, Const.getScreenshot(), (SavedGameRequestStatus a, ISavedGameMetadata b)=>{
								m_creationTime = Time.time;
							});
						} 
						else {
							// handle error
						}
					});
				}

				yield return StartCoroutine(spawnMobPerCore(GetCurrentWave().randomMobSpawns[m_wave%GetCurrentWave().randomMobSpawns.Length], waveProgress));

				m_wave++;
			}
		}
	}

	void bindItemOption(ItemData item, RefItemOptionSpawn[] descs)
	{
		foreach(RefItemOptionSpawn desc in descs)
		{
			float ratio = Random.Range(0f, 1f);
			if (ratio <= desc.ratio)
			{
				item.OptionDescs.Add(new ItemMagicOption(desc.type, Random.Range(desc.minValue, desc.maxValue)));
			}
		}
	}

	public int SpawnMobLevel()
	{
		return (int)(1 + ProgressStage());
	}

	public float ProgressStage()
	{
		return (float)(m_wave)/GetCurrentWave().mobSpawns.Length;
	}

	public void OnKillMob(Mob mob)
	{
		SpawnItemBox(mob.RefDropItems, mob.transform.position);

		if (mob.Boss)
		{
			SpawnItemBox(GetCurrentWave().itemSpawn.bossDefaultItem, mob.transform.position);
			TimeEffector.Instance.BulletTime(0.005f);
		}

		if (true == mob.CheckOnDeath)
		{
			m_mobsOfCheckOnDeath--;
		}

		if (m_champ)
		{
			++m_champ.ComboKills;
			if (m_champ.ComboKills % Const.ComboSkillStackOnCombo == 0)
			{
				++m_champ.ComboSkillStack;
				GPlusPlatform.Instance.ReportProgress(Const.ACH_COMBO_KILLS_100, 100, (bool success)=>{
				});
			}

			if (Warehouse.Instance.Stats.m_comboKills < m_champ.ComboKills)
			{
				Warehouse.Instance.Stats.m_comboKills = m_champ.ComboKills;
			}

			m_comboGUIShake.enabled = true;
			m_comboGUIShake.shake = 2f;

			switch(mob.RefMob.id)
			{
			case 50001:
				GPlusPlatform.Instance.ReportProgress(Const.ACH_KILL_THE_BOSS_OF_STAGE_1, 100, (bool success)=>{
				});
				break;
			case 50002:
				GPlusPlatform.Instance.ReportProgress(Const.ACH_KILL_THE_BOSS_OF_STAGE_2, 100, (bool success)=>{
				});
				break;
			case 50003:
				GPlusPlatform.Instance.ReportProgress(Const.ACH_KILL_THE_BOSS_OF_STAGE_3, 100, (bool success)=>{
				});
				break;
			case 50004:
				GPlusPlatform.Instance.ReportProgress(Const.ACH_KILL_THE_BOSS_OF_STAGE_4, 100, (bool success)=>{
				});
				break;
			case 50005:
				GPlusPlatform.Instance.ReportProgress(Const.ACH_KILL_THE_BOSS_OF_STAGE_5, 100, (bool success)=>{
				});
				break;
			}
		}
	}

	IEnumerator SpawnEffectDestroy(GameObject obj, float delay)
	{		
		yield return new WaitForSeconds (delay);
		
		GameObjectPool.Instance.Free(obj);		
	}


	IEnumerator EffectSpawnMob(Vector3 pos, Creature creature)
	{		

		Vector3 enemyPos = pos;
		enemyPos.y = m_prefSpawnEffect.transform.position.y;

		if (m_prefSpawnEffect != null)
		{
			GameObject spawnEffect = GameObjectPool.Instance.Alloc(m_prefSpawnEffect, enemyPos, m_prefSpawnEffect.transform.rotation) as GameObject;
			ParticleSystem particle = spawnEffect.GetComponentInChildren<ParticleSystem>();
			
			StartCoroutine(SpawnEffectDestroy(spawnEffect, particle.duration));
		}

		yield return new WaitForSeconds (1f);
		creature.gameObject.SetActive(true);


	}

	IEnumerator EffectSpawnMob1(Vector3 pos, Creature creature)
	{	
		Creature.Type oriType = creature.CreatureType;
		creature.CreatureType = Creature.Type.Npc;

		pos.y = Random.Range(10,15);
		creature.transform.position = pos;

		Parabola parabola = new Parabola(creature.gameObject, 15f, 0f, 90*Mathf.Deg2Rad, 3);
		creature.gameObject.SetActive(true);

		while(parabola.Update())
		{
			yield return null;
		}

		creature.CreatureType = oriType;
	}

	IEnumerator EffectSpawnItemBox(ItemBox itemBox, float time)
	{				
		yield return new WaitForSeconds (time);
		itemBox.gameObject.SetActive(true);
		
	}
	
	public Mob SpawnMob(RefMob refMob, Vector3 pos, bool boss, bool monitoredDeath)
	{
		RefItemSpawn[] refDropItems = null;
		if (GetCurrentWave().itemSpawn.mapMobItems.ContainsKey(refMob.id))
		{
			refDropItems = GetCurrentWave().itemSpawn.mapMobItems[refMob.id].refDropItems;
		}
		else
		{
			refDropItems = GetCurrentWave().itemSpawn.defaultItem;
		}

		int mobLevel = SpawnMobLevel();
		GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/mob");
		GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/mon_skin/" + refMob.prefBody);
		if (prefEnemyBody == null)
		{
			Debug.Log(refMob.prefBody);
			return null;
		}
		Vector3 enemyPos = pos;

		enemyPos.x = Mathf.Clamp(enemyPos.x, m_edgeRect.transform.position.x-m_edgeRect.size.x/2, m_edgeRect.transform.position.x+m_edgeRect.size.x/2);
		enemyPos.z = Mathf.Clamp(enemyPos.z, m_edgeRect.transform.position.z-m_edgeRect.size.z/2, m_edgeRect.transform.position.z+m_edgeRect.size.z/2);

		GameObject enemyObj = Instantiate(prefEnemy, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;
		GameObject enemyBody = GameObjectPool.Instance.Alloc(prefEnemyBody, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;
		enemyBody.name = "Body";
		enemyBody.transform.parent = enemyObj.transform;
		enemyBody.transform.localPosition = Vector3.zero;
		enemyBody.transform.localRotation = prefEnemyBody.transform.rotation;
		enemyObj.transform.localScale = new Vector3(refMob.scale, refMob.scale, refMob.scale);

		Mob enemy = enemyObj.GetComponent<Mob>();
		enemy.Init(refMob, mobLevel, this, refDropItems, boss);
		enemy.m_creatureProperty.AlphaMaxHP=(int)(enemy.m_creatureProperty.MaxHP*ProgressStage()*0.1f);
		enemy.m_creatureProperty.Heal(enemy.m_creatureProperty.MaxHP);

		if (m_champ != null)
			enemy.SetTarget(m_champ.gameObject);

		Debug.Log(refMob.prefBody + ", Lv : " + mobLevel + ", HP: " + enemy.m_creatureProperty.HP + ", PA:" + enemy.m_creatureProperty.PhysicalAttackDamage + ", PD:" + enemy.m_creatureProperty.PhysicalDefencePoint + ", scale:" + refMob.scale + " pos:" + enemyPos);

	
		if (monitoredDeath == true)
		{
			enemy.CheckOnDeath = true;
			m_mobsOfCheckOnDeath++;
		}

		switch(refMob.id)
		{
		case 50001:
			GPlusPlatform.Instance.ReportProgress(Const.ACH_DISCOVER_THE_BOSS_OF_STAGE_1, 100, (bool success)=>{
			});
			break;
		case 50002:
			GPlusPlatform.Instance.ReportProgress(Const.ACH_DISCOVER_THE_BOSS_OF_STAGE_2, 100, (bool success)=>{
			});
			break;
		case 50003:
			GPlusPlatform.Instance.ReportProgress(Const.ACH_DISCOVER_THE_BOSS_OF_STAGE_3, 100, (bool success)=>{
			});
			break;
		case 50004:
			GPlusPlatform.Instance.ReportProgress(Const.ACH_DISCOVER_THE_BOSS_OF_STAGE_4, 100, (bool success)=>{
			});
			break;
		case 50005:
			GPlusPlatform.Instance.ReportProgress(Const.ACH_DISCOVER_THE_BOSS_OF_STAGE_5, 100, (bool success)=>{
			});
			break;
		}

		return enemy;
	}
	
	public void SpawnItemBox(RefItemSpawn[] refDropItems, Vector3 pos)
	{		
		if (refDropItems == null)
		{
			refDropItems = GetCurrentWave().itemSpawn.defaultItem;
		}

		float goldAlpha = m_wave*0.1f;
		int spawnedItemCount = 0;
		foreach(RefItemSpawn desc in refDropItems)
		{
			for(int i = 0; i < desc.count; ++i)
			{
				float ratio = Random.Range(0f, 1f);
				if (ratio <= desc.ratio)
				{
					float scale = 1f;
					ItemData item = null;
					switch(desc.refItem.type)
					{
					case ItemData.Type.Gold:
						item = new ItemGoldData(Random.Range(desc.minValue, desc.maxValue));
						switch(item.Count/3)
						{
						case 0:
							scale = 0.3f;
							break;
						case 1:
							scale = 0.5f;
							break;
						case 2:
							scale = 0.7f;
							break;
						}
						item.Count += (int)(item.Count*goldAlpha);

						break;
					case ItemData.Type.HealPosion:
						item = new ItemHealPosionData(Random.Range(desc.minValue, desc.maxValue));
						switch(item.Count/300)
						{
						case 0:
							scale = 0.3f;
							break;
						case 1:
							scale = 0.5f;
							break;
						case 2:
							scale = 0.7f;
							break;
						}
						break;
					case ItemData.Type.Weapon:
						item = new ItemWeaponData(desc.refItem.id, null);
						break;
					case ItemData.Type.WeaponParts:
						item = new ItemWeaponPartsData(desc.refItemId, Random.Range(desc.minValue, desc.maxValue));					
						break;					
					case ItemData.Type.WeaponDNA:
						item = new ItemWeaponDNAData(Random.Range(desc.minValue, desc.maxValue));					
						break;
					case ItemData.Type.Accessory:
						item = new ItemAccessoryData(desc.refItem.id);					
						break;
					case ItemData.Type.GoldMedal:
						item = new ItemGoldMedalData(Random.Range(desc.minValue, desc.maxValue));					
						break;
					case ItemData.Type.SilverMedal:
						item = new ItemSilverMedalData(Random.Range(desc.minValue, desc.maxValue));					
						break;
					case ItemData.Type.MobEgg:
						break;					
					}
					
					if (item != null)
					{
						++spawnedItemCount;

						GameObject itemBoxObj = (GameObject)Instantiate(m_prefItemBox, pos, Quaternion.Euler(0f, 0f, 0f));
						GameObject itemSkinObj = (GameObject)Instantiate(m_prefItemBoxSkins[(int)desc.refItem.type], pos, Quaternion.Euler(0f, 0f, 0f));
						itemSkinObj.transform.parent = itemBoxObj.transform;
						itemSkinObj.transform.localPosition = Vector3.zero;
						itemSkinObj.transform.localRotation = m_prefItemBoxSkins[(int)desc.refItem.type].transform.rotation;
						itemBoxObj.transform.localScale = Vector3.one * scale;
						itemBoxObj.SetActive(false);
						
						ItemBox itemBox = itemBoxObj.GetComponent<ItemBox>();
						itemBox.Item = item;
						itemBox.PickupCallback = (Creature obj)=>{
							m_goldGUIShake.Gold = Warehouse.Instance.Gold.Item.Count;
						};

						StartCoroutine(EffectSpawnItemBox(itemBox, 0.15f*spawnedItemCount));
					}
					
				}
			}

		}
		
	}

	// Update is called once per frame
	void Update () {
		if (m_champ == null)
		{
			GameObject obj = GameObject.Find("Champ");
			if (obj != null)
			{
				m_champ = obj.GetComponent<Champ>();
			}
		}

		if (m_champ != null)
		{
			m_comboGUIShake.Text = "x" + m_champ.ComboKills;
		}

	}

}
