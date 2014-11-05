using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	public enum Type
	{
		Gold,
		HPPosion,
		Ring,
	}

	[SerializeField]
	Type m_itemType = Type.Gold;

	[SerializeField]
	int		m_itemValue = 0;

	void Start () {
		

	}

	void Update()
	{

	}

	public void Death()
	{
		DestroyObject(this.gameObject);
	}

	public Type ItemType
	{
		get { return m_itemType; }
	}

	public int ItemValue
	{
		get { return m_itemValue; }
	}
}
