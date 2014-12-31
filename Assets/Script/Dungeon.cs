using UnityEngine;
using System.Collections;
using Enum = System.Enum;

public class Dungeon : MonoBehaviour {

	[SerializeField]
	int				m_dungeonId;

	GameObject[]	m_prefItemBoxes = new GameObject[(int)ItemData.Type.Count];

	// Use this for initialization
	void Start () {		

		string[] itemTypeNames = Enum.GetNames(typeof(ItemData.Type));
		for(int i = 0; i < itemTypeNames.Length-1; ++i)
		{
			m_prefItemBoxes[i] = Resources.Load<GameObject>("Pref/Item" + itemTypeNames[i] + "Box");
		}
	}

	// Update is called once per frame
	void Update () {

	}

	IEnumerator delayLoadLevel(float delay)
	{
		yield return new WaitForSeconds(delay);
		Application.LoadLevel("Worldmap");
	}
	
	public void DelayLoadLevel(float delay)
	{
		StartCoroutine(delayLoadLevel(delay));
	}

	void bindItemOption(ItemData item, RefItemOptionSpawn[] descs)
	{
		foreach(RefItemOptionSpawn desc in descs)
		{
			float ratio = Random.Range(0f, 1f);
			if (ratio <= desc.ratio)
			{
				item.OptionDescs.Add(new ItemOptionDesc(desc.type, Random.Range(desc.minValue, desc.maxValue)));
			}
		}
	}

	IEnumerator EffectBulletTime(float t)
	{
		yield return new WaitForSeconds (0.002f);
		
		if (t > 0)
		{
			Time.timeScale = 1.1f-t;
			StartCoroutine(EffectBulletTime(t-0.002f));
		}
		else
		{

		}
		
	}

	public void OnKillMob(Mob mob)
	{
		SpawnItemBox(mob.RefMob, mob.transform.position);

		if (mob.Boss == true)
		{
			StartCoroutine(EffectBulletTime(1));
		}
	}

	void SpawnItemBox(RefMob spawnDesc, Vector3 pos)
	{
		if (spawnDesc == null)
			return;


		foreach(RefItemSpawn desc in spawnDesc.refDropItems)
		{
			float ratio = Random.Range(0f, 1f);
			if (ratio <= desc.ratio)
			{
				GameObject itemBoxObj = (GameObject)Instantiate(m_prefItemBoxes[(int)desc.refItem.type], pos, Quaternion.Euler(0f, 0f, 0f));
				ItemData item = null;
				switch(desc.refItem.type)
				{
				case ItemData.Type.Gold:
					item = new ItemGoldData(Random.Range(desc.minValue, desc.maxValue));
					break;
				case ItemData.Type.HealPosion:
	                item = new ItemHealPosionData(Random.Range(desc.minValue, desc.maxValue));
                    break;
				case ItemData.Type.Weapon:
					item = new ItemWeaponData(desc.refItem.id);
					bindItemOption(item, desc.itemOptionSpawns);

					break;
				case ItemData.Type.WeaponUpgradeFragment:
					item = new ItemWeaponUpgradeFragmentData();					
					break;
				case ItemData.Type.Follower:
					item = new ItemFollowerData(desc.refItemId);					
					break;
				}

				if (item != null)
				{
					ItemBox itemBox = itemBoxObj.GetComponent<ItemBox>();
					itemBox.Item = item;
				}

			}
		}

	}

	public int DungeonId
	{
		get{return m_dungeonId;}
	}
}
