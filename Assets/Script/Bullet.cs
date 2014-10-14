using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(10*Time.deltaTime, 0, 0, transform);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Enemy")
		{
			DestroyObject(this.gameObject);
			DestroyObject(other.gameObject);
		}
		else if (other.tag == "Wall")
		{
			DestroyObject(this.gameObject);
		}
	}
}
