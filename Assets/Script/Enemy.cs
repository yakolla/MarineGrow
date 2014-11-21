using UnityEngine;
using System.Collections;

public class Enemy : Creature {

	GameObject			m_target;
	// Use this for initialization
	new void Start () {
		base.Start();
		m_material = transform.Find("Body/mon_a").GetComponent<SkinnedMeshRenderer>().material;

	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();

		if (AutoAttack() == false)
		{
			if (m_target)
			{
				m_navAgent.SetDestination(m_target.transform.position);
			}
		}
		else
		{
			m_navAgent.Stop();

		}

	}

	override public string[] GetAutoTargetTags()
	{
		return new string[]{Creature.Type.Champ.ToString()};
	}

	public void SetTarget(GameObject obj )
	{
		m_target = obj;
	}

}
