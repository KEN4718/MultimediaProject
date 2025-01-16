using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTheQuadRender : MonoBehaviour 
{
	public Transform myRotTrans;
	public string defaultLayer,topLayer;
	public float toplayerAngle;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Mathf.Abs (myRotTrans.eulerAngles.y - toplayerAngle) < 10f)
		{
			this.gameObject.layer = LayerMask.NameToLayer (topLayer);
		} 
		else 
		{
			this.gameObject.layer = LayerMask.NameToLayer (defaultLayer);
		}
	}
}
