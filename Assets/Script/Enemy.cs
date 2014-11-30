using UnityEngine;
using System.Collections;

public class Enemy : Creature {
	[SerializeField]
	GameObject			m_prefWeapon = null;

	GameObject			m_goalForNavigation;
	// Use this for initialization
	new void Start () {
		base.Start();
		//m_material = transform.Find("Body/mon_a").GetComponent<SkinnedMeshRenderer>().material;

		ChangeWeapon(m_prefWeapon);
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();

		if (AutoAttack() == false)
		{
			if (m_goalForNavigation)
			{
				m_navAgent.SetDestination(m_goalForNavigation.transform.position);
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
		m_goalForNavigation = obj;
	}

}
