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
		m_weaponID = refMob.refWeaponItem;
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
		follower.WeaponItemID = m_weaponID;
		follower.Init(obj, m_aiType, m_baseProperty);
		follower.m_creatureProperty.Level = obj.m_creatureProperty.Level;
		Debug.Log(obj.m_creatureProperty.Level);
	}

	override public void NoUse(Creature obj)
	{
	}
	
	override public string Description()
	{
		return m_followerName + "\n" +  base.Description();
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Weapon)
			return base.Compare(item);

		ItemWeaponData weapon = (ItemWeaponData)item;
		return FollowerName.CompareTo(weapon.WeaponName) == 0;
	}

}
