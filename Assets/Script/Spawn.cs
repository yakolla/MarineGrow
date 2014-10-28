using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {

	public int 				m_mobCount = 10;
	public float 			m_interval = 30;
	public Transform		m_targetPos;
	public GameObject		m_prefEnemy;
	public GameObject		m_prefBossEnemy;
	GameObject				m_boss;

	// Use this for initialization
	void Start () {

		m_boss = Instantiate (m_prefBossEnemy, transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		m_boss.GetComponent<Enemy>().SetTargetPos(transform.position);
		StartCoroutine(spawnEnemyPer());
	}

	IEnumerator spawnEnemyPer()
	{
		float cx = transform.position.x;
		float cz = transform.position.z;
		for(int i = 0; i < m_mobCount; ++i)
		{
			Vector3 enemyPos = m_prefEnemy.transform.position;
			GameObject obj = Instantiate (m_prefEnemy, new Vector3(Random.Range(cx,cx+3f), enemyPos.y, Random.Range(cz,cz+3f)), Quaternion.Euler (0, 0, 0)) as GameObject;
			obj.GetComponent<Enemy>().SetTargetPos(m_targetPos.position);
		}

		
		yield return new WaitForSeconds (m_interval);

		if (m_boss != null)
		{
			StartCoroutine(spawnEnemyPer());
		}

	}

	// Update is called once per frame
	void Update () {

	}

}
