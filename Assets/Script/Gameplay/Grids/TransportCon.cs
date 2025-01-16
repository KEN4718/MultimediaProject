using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportCon : MonoBehaviour   
{
    // Two controllers cannot control the same block's type simultaneously; this will cause conflicts.
    // This script is used to control rotation by clicking to change the type.
    // Typically, this is used to control the type value of fixed blocks.
    // When used for visual deception:
    // If the rotating block and the connectable block are on the same plane or do not overlap in the XZ plane,
    // the BFS algorithm can directly determine whether the path is disconnected or connected (dead-end).

    public ChangeAxis myAxis;
    public LayerMask playerLayer;
    public ZeraMovement player;

    public Transform myRotMap;      // The map block that needs rotation.
    public float destinationAngle;  // Target Euler angle.
    public WayPoint[] myGrids;      // The grid cells that need to have their type changed.
    public int transtype = 0;       // If the path is connected, myGrids' type will be changed to transtype.
                                    // The value of transtype is set to match the type of the road grid where this object is located.
    private int defaultMyGridType;

    public bool canTrans = false;

    void Awake () 
    {
        foreach (WayPoint myGrid in myGrids)
        {
            defaultMyGridType = myGrid.type;
        }
    }
    
    void Update () 
    {
        switch (myAxis) 
        {
            case ChangeAxis.y:
                if (Mathf.Abs(myRotMap.eulerAngles.y - destinationAngle) < 0.2f) 
                {
                    // Check if the current angle matches the target angle.
                    canTrans = true;
                } 
                else 
                {
                    canTrans = false;
                }
                break;

            case ChangeAxis.z:
                if (Mathf.Abs(myRotMap.eulerAngles.z - destinationAngle) < 0.2f) 
                {
                    // Check if the current angle matches the target angle.
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
                myGrid.SetType(transtype, 1); // Change the type of the grid cells to transtype if the path is connected.
            }
        } 
        else 
        {
            foreach (WayPoint myGrid in myGrids) 
            {
                myGrid.SetType(defaultMyGridType, 1); // Restore the grid cells to their default type if the path is disconnected.
            }
        }
    }
}
