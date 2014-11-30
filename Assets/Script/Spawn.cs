using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemOptionSpawnDesc
{
	public Item.Option  m_optionType;
	public int 			m_minItemValue = 1;
	public int 			m_maxItemValue = 1;
	[Range(0F, 1F)]
	public float		m_ratio = 0f;
	
}

[System.Serializable]
public class ItemSpawnDesc
{
	public Item.Type m_itemType = Item.Type.Gold;
	public string 		m_ItemCodeName;
	public int 			m_minItemValue = 1;
	public int 			m_maxItemValue = 1;
	[Range(0F, 1F)]
	public float		m_ratio = 0f;

	public ItemOptionSpawnDesc[] m_itemOptionSpawnDesc = null;

}

[System.Serializable]
public class SpawnDesc
{
	public int 			m_mobCount = 10;
	public float 		m_interval = 30;

	public GameObject	m_prefEnemy = null;

	public ItemSpawnDesc[]		m_itemSpawnDesc = null;
}

public class Spawn : MonoBehaviour {
	[SerializeField]
	SpawnDesc[]		m_descs = null;

	[SerializeField]
	GameObject		m_prefBossEnemy = null;

	[SerializeField]
	GameObject		m_target = null;

	GameObject		m_boss = null;

	// Use this for initialization
	void Start () {

		m_boss = Instantiate (m_prefBossEnemy, transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		foreach(SpawnDesc desc in m_descs)
		{
			StartCoroutine(spawnEnemyPer(desc));
		}
	}

	IEnumerator spawnEnemyPer(SpawnDesc desc)
	{
		if (m_target != null)
		{
			float cx = transform.position.x;
			float cz = transform.position.z;
			
			for(int i = 0; i < desc.m_mobCount; ++i)
			{
				Vector3 enemyPos = desc.m_prefEnemy.transform.position;
				GameObject obj = Instantiate (desc.m_prefEnemy, new Vector3(Random.Range(cx,cx+3f), enemyPos.y, Random.Range(cz,cz+3f)), Quaternion.Euler (0, 0, 0)) as GameObject;
				obj.GetComponent<Enemy>().SetTarget(m_target);
				obj.GetComponent<Enemy>().SetSpawnDesc(desc);
			}
		}

				
		yield return new WaitForSeconds (desc.m_interval);

		if (m_boss != null)
		{
			StartCoroutine(spawnEnemyPer(desc));
		}

	}

	// Update is called once per frame
	void Update () {
		if (m_target == null)
		{
			m_target = GameObject.Find("Champ(Clone)");
		}
		if (m_boss == null)
		{
			Transform tran = transform.FindChild("Spawn");
			if (tran != null)
			{
				GameObject spawn = tran.gameObject;
				spawn.transform.parent = null;
				spawn.SetActive(true);
			}
			
			DestroyObject(this.gameObject);
		}
	}

}
