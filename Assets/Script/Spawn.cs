using UnityEngine;
using System.Collections;

[System.Serializable]
class SpawnDesc
{
	[SerializeField]
	public int 			m_mobCount = 10;
	[SerializeField]
	public float 			m_interval = 30;
	[SerializeField]
	public Transform		m_targetPos;
	[SerializeField]
	public GameObject		m_prefEnemy;
}

public class Spawn : MonoBehaviour {
	[SerializeField]
	SpawnDesc[]	m_descs;

	[SerializeField]
	GameObject		m_prefBossEnemy;

	GameObject				m_boss;

	// Use this for initialization
	void Start () {

		m_boss = Instantiate (m_prefBossEnemy, transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		m_boss.GetComponent<Enemy>().SetTargetPos(transform.position);
		foreach(SpawnDesc desc in m_descs)
		{
			StartCoroutine(spawnEnemyPer(desc));
		}
	}

	IEnumerator spawnEnemyPer(SpawnDesc desc)
	{
		float cx = transform.position.x;
		float cz = transform.position.z;


		for(int i = 0; i < desc.m_mobCount; ++i)
		{
			Vector3 enemyPos = desc.m_prefEnemy.transform.position;
			GameObject obj = Instantiate (desc.m_prefEnemy, new Vector3(Random.Range(cx,cx+3f), enemyPos.y, Random.Range(cz,cz+3f)), Quaternion.Euler (0, 0, 0)) as GameObject;
			obj.GetComponent<Enemy>().SetTargetPos(desc.m_targetPos.position);
		}
				
		yield return new WaitForSeconds (desc.m_interval);

		if (m_boss != null)
		{
			StartCoroutine(spawnEnemyPer(desc));
		}

	}

	// Update is called once per frame
	void Update () {

	}

}
