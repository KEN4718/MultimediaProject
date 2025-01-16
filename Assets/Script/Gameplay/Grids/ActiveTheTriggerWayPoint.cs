using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTheTriggerWayPoint : MonoBehaviour //此类用于机关触发旋转改变type
{
	public WayPoint[] myWayPointToActive;
	public float targetAngle;
	//public int targetType;

	private int defaultMyGridType;



	void Awake () 
	{
		foreach(WayPoint wayPoint in myWayPointToActive)
		{
			defaultMyGridType =  wayPoint.type;
		}

	}


	void Update()
	{
		if (Mathf.Abs (transform.eulerAngles.x - targetAngle) < 0.2f)
		{
			foreach (WayPoint wayPoint in myWayPointToActive)
			{
				//wayPoint.type = targetType;
				wayPoint.GetComponent<BoxCollider>().enabled = true;
			}
		} 
		else 
		{
			foreach (WayPoint wayPoint in myWayPointToActive)
			{
				//wayPoint.type = defaultMyGridType;
				wayPoint.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}
}
