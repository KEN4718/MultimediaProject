using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//用于根据父对象的旋转来设置WayPoint的伪坐标
public class SetPseudoPosByRot : MonoBehaviour 
{
	public Transform myRot;
	public TriggerActiveTheGrid myTrigger;
	public float[] targetRot;
	public Vector2[] targetPseudoPos;
	// Use this for initialization
	void Start ()
	{
		
	}

	// Update is called once per frame
	void Update () 
	{
		if(myTrigger.canUp)
		{
			for(int i = 0; i<targetRot.Length;i++)
			{
				if(Mathf.Abs(targetRot[i] - myRot.transform.eulerAngles.y) < 0.2f)
				{
					GetComponent<WayPoint> ().fakeX = targetPseudoPos [i].x;
					GetComponent<WayPoint> ().fakeY = targetPseudoPos [i].y;
				}
			}
		}

	}
}
