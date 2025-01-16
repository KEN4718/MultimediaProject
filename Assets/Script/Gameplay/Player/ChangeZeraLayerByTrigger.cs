using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeZeraLayerByTrigger : MonoBehaviour
{
	public string defaultLayer,targetLayer;

	public ZeraMovement zera;


	void Awake()
	{
		defaultLayer = LayerMask.LayerToName (zera.gameObject.layer);
	}

	void Start ()
	{
		
	}
	

	void Update ()
	{
		
	}

	void OnTriggerEnter(Collider other)
	{
		ZeraMovement zera = other.GetComponent<ZeraMovement> ();
		if (zera != null) 
		{
			int layerIndex = LayerMask.NameToLayer(targetLayer);
            if (layerIndex != -1) // Ensure the layer exists
            {
                zera.gameObject.layer = layerIndex;
            }
            else
            {
                Debug.LogError($"Layer '{targetLayer}' does not exist. Please check the layer name.");
            }
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(Color.yellow.r,Color.yellow.g,Color.yellow.b,0.5f);
		Gizmos.DrawSphere (transform.position,1);
	}

}
