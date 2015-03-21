using UnityEngine;
using System.Collections;


public class ItemFollowerData : ItemData{

	public string 	m_followerName;
	public int		m_weaponID;
	public RefCreatureBaseProperty	m_baseProperty;
	public MobAIType	m_aiType;

	public ItemFollowerData(RefMob refMob) : base(1001, 1)
	{
		m_followerName = refMob.prefBody;
		m_weaponID = refMob.refWeaponItems[0].refItemId;
		m_baseProperty = refMob.baseCreatureProperty;
		m_aiType = refMob.mobAI;
	}

	public ItemFollowerData() : base(1001, 1)
	{

	}

	public string FollowerName
	{
		get{return m_followerName;}
	}

	override public void Use(Creature obj)
	{
		Vector3 enemyPos = obj.transform.position;
		float angle = Random.Range(-3.14f, 3.14f);
		enemyPos.x += Mathf.Cos(angle) * 1f;
		enemyPos.z += Mathf.Sin(angle) * 1f;
		GameObject followerObj = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Pref/Follower"), enemyPos, obj.transform.rotation);
		GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/mon_skin/" + m_followerName);
		
		GameObject enemyBody = GameObject.Instantiate (prefEnemyBody, Vector3.zero, Quaternion.Euler (0, 0, 0)) as GameObject;
		enemyBody.name = "Body";
		enemyBody.transform.parent = followerObj.transform;
		enemyBody.transform.localPosition = prefEnemyBody.transform.localPosition;
		enemyBody.transform.localRotation = prefEnemyBody.transform.rotation;

		Follower follower = (Follower)followerObj.GetComponent<Follower>();

		ItemObject weapon = new ItemObject(new ItemWeaponData(m_weaponID, null));
		weapon.Item.Level = Level;
		weapon.Item.Evolution = Evolution;
		follower.WeaponItem = weapon;
		follower.Init(obj, m_aiType, m_baseProperty);
		follower.m_creatureProperty.Level = obj.m_creatureProperty.Level;
	
	}

	override public void NoUse(Creature obj)
	{
	}
	
	override public string Description()
	{
		return "<color=white>" + m_followerName + "</color>" + "\n" +  base.Description();
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Follower)
			return false;

		ItemFollowerData itemFollowerData = item as ItemFollowerData;
		return m_weaponID == itemFollowerData.m_weaponID;
	}

}
