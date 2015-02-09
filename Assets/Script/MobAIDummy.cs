using UnityEngine;
using System.Collections;

public class MobAIDummy : MobAI {


	// Update is called once per frame
	override public void Update () {
		if (TimeEffector.Instance.IsStop() == true)
			return;

		m_navAgent.updatePosition = false;
	}


}
