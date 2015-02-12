using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemFollowerData : ItemData{

	string 	m_followerName;
	int		m_weaponID;
	RefCreatureBaseProperty	m_baseProperty;
	MobAIType	m_aiType;

	public ItemFollowerData(RefMob refMob) : base(1001, 1)
	{
		m_followerName = refMob.prefBody;
		m_weaponID = refMob.refWeaponItems[0].refItemId;
		m_baseProperty = refMob.baseCreatureProperty;
		m_aiType = refMob.mobAI;
	}

	public string FollowerName
	{
		get{return m_followerName;}
	}

	override public void Use(Creature obj)
	{
		GameObject followerObj = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Pref/Follower"), obj.transform.position, obj.transform.rotation);
		GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/mon_skin/" + m_followerName);
		
		Vector3 enemyPos = obj.transform.position;
		
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
