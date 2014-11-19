using UnityEngine;
using System.Collections;

public class background : MonoBehaviour {

	GameObject[]	m_prefItemBoxes = new GameObject[(int)ItemBox.Type.Count];

	// Use this for initialization
	void Start () {
		m_prefItemBoxes[(int)ItemBox.Type.Gold] = Resources.Load<GameObject>("Pref/ItemGoldBox");
		m_prefItemBoxes[(int)ItemBox.Type.HealPosion] = Resources.Load<GameObject>("Pref/ItemHealPosionBox");
	}

	// Update is called once per frame
	void Update () {

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
				ItemBox itemBox = itemBoxObj.GetComponent<ItemBox>();
				itemBox.ItemValue = Random.Range(desc.m_minItemValue, desc.m_maxItemValue);
				itemBoxObj.transform.localScale += itemBoxObj.transform.localScale*(itemBox.ItemValue/1000f);
			}
		}




	}
}
