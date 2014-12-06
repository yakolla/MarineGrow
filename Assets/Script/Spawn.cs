using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {

	[SerializeField]
	GameObject		m_prefBossEnemy = null;

	[SerializeField]
	GameObject		m_target = null;

	GameObject		m_boss = null;

	// Use this for initialization
	void Start () {
		int dungeonId = transform.parent.GetComponent<Dungeon>().DungeonId;

		m_boss = Instantiate (m_prefBossEnemy, transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		foreach(KeyValuePair<int, RefMobSpawn> pair in RefData.Instance.RefWorldMaps[dungeonId].refMobSpawns)
		{
			StartCoroutine(spawnEnemyPer(pair.Value));
		}
	}

	IEnumerator spawnEnemyPer(RefMobSpawn desc)
	{
		if (m_target != null)
		{
			float cx = transform.position.x;
			float cz = transform.position.z;

			GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + desc.prefEnemy);
			for(int i = 0; i < desc.mobCount; ++i)
			{
				Vector3 enemyPos = prefEnemy.transform.position;
				GameObject obj = Instantiate (prefEnemy, new Vector3(Random.Range(cx,cx+3f), enemyPos.y, Random.Range(cz,cz+3f)), Quaternion.Euler (0, 0, 0)) as GameObject;
				obj.GetComponent<Enemy>().SetTarget(m_target);
				obj.GetComponent<Enemy>().SetSpawnDesc(desc);
			}
		}

				
		yield return new WaitForSeconds (desc.interval);

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
	}

}
