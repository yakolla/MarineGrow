using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		this.transform.rigidbody.AddRelativeForce(new Vector3(-3f, 0f, 0f));
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Enemy")
		{
			DestroyObject(this.gameObject);
			DestroyObject(other.gameObject);
		}

	}
}
