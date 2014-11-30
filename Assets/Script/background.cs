using UnityEngine;
using System.Collections;

public class background : MonoBehaviour {

	GameObject[]	m_prefItemBoxes = new GameObject[(int)Item.Type.Count];

	// Use this for initialization
	void Start () {
		m_prefItemBoxes[(int)Item.Type.Gold] = Resources.Load<GameObject>("Pref/ItemGoldBox");
		m_prefItemBoxes[(int)Item.Type.HealPosion] = Resources.Load<GameObject>("Pref/ItemHealPosionBox");
		m_prefItemBoxes[(int)Item.Type.Weapon] = Resources.Load<GameObject>("Pref/ItemWeaponBox");
	}

	// Update is called once per frame
	void Update () {

	}

	IEnumerator delayLoadLevel(float delay)
	{
		yield return new WaitForSeconds(delay);
		Application.LoadLevel("main");
	}
	
	public void DelayLoadLevel(float delay)
	{
		StartCoroutine(delayLoadLevel(delay));
	}

	public void SpawnItemBox(SpawnDesc spawnDesc, Vector3 pos)
	{
		if (spawnDesc == null)
			return;


		foreach(ItemSpawnDesc desc in spawnDesc.m_itemSpawnDesc)
		{
			float ratio = Random.Range(0f, 1f);
			if (ratio <= desc.m_ratio)
			{
				GameObject itemBoxObj = (GameObject)Instantiate(m_prefItemBoxes[(int)desc.m_itemType], pos, Quaternion.Euler(0f, 0f, 0f));
				Item item = null;
				switch(desc.m_itemType)
				{
				case Item.Type.Gold:
					item = new ItemGold(Random.Range(desc.m_minItemValue, desc.m_maxItemValue));
					break;
				case Item.Type.HealPosion:
					item = new ItemHealPosion(Random.Range(desc.m_minItemValue, desc.m_maxItemValue));
					break;
				case Item.Type.Weapon:
					item = new ItemWeapon("Pref/" + desc.m_ItemCodeName);
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
}
