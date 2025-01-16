using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransConOptimize : MonoBehaviour  //此类用于控制旋转本体四个方向的type控制，因为没有else语句//用于视觉欺骗的时候
{
	public ChangeAxis myAxis;
	public LayerMask playerLayer;
	public ZeraMovement player;

	public Transform myRotMap;//需要判断旋转的地图区块
	public float destinationAngle;//目标欧拉角度 
	public WayPoint[] myGrids; // 需要改变type的格子们
	public int transtype = 0; //如果道路接上，将吧myGrids的type变成transtype，transtype的值设置为与这个对象所在的道路格子type一致
	private int defaultMyGridType;

	public bool canTrans = false;

	void Awake()
	{
		foreach (WayPoint myGrid in myGrids)
		{
			defaultMyGridType = myGrid.type;
		}

	}


	void Update()
	{
		switch (myAxis)
		{
			case ChangeAxis.y:
				if (Mathf.Abs(myRotMap.eulerAngles.y - destinationAngle) < 0.2f) //判断目的地的角度是否与
				{
					canTrans = true;
				}
				else
				{
					canTrans = false;
				}
				break;
			case ChangeAxis.z:
				if (Mathf.Abs(myRotMap.eulerAngles.z - destinationAngle) < 0.2f) //判断目的地的角度是否与
				{
					canTrans = true;
				}
				else
				{
					canTrans = false;
				}
				break;
		}

		if (canTrans)
		{
			foreach (WayPoint myGrid in myGrids)
			{
				myGrid.type = transtype;
			}
		}
	}
}
