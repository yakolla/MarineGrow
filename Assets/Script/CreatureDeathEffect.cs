using UnityEngine;
using System.Collections;

public class CreatureDeathEffect : MonoBehaviour {


	void Start () {
		StartCoroutine(DoEffect());

	}
	IEnumerator DoEffect()
	{
		yield return new WaitForSeconds(1f);
		DestroyObject(this.gameObject);

	}

}
