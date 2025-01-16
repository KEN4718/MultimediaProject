using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
	public static PathFinding pathFinding;

	public ZeraMovement zera;
	[SerializeField]
	public WayPoint startPoint, endPoint;

	public Dictionary<Vector2, WayPoint> wayPointDict = new Dictionary<Vector2, WayPoint>(); // Use a dictionary to store all walkable grids in the game
	private Vector2[] directions =
	{
		Vector2.up,Vector2.down,Vector2.right,Vector2.left
	}; // Four directions

	public Queue<WayPoint> queue = new Queue<WayPoint>();
	[SerializeField]
	public bool isRunning = true;

	private WayPoint searchCenter;

	public List<WayPoint> path = new List<WayPoint>();


	public void Awake()
	{
		if (pathFinding != null)
		{
			Destroy(this.gameObject);
		}
		else
		{
			pathFinding = this;
		}
	}

	public List<WayPoint> GetPath()
	{

		LoadAllWayPoints(); // Recalculate the map information every time pathfinding is performed
		BFS();
		CreatePath();
		return path;
	}

	private void ExploreAround()
	{
		if (!isRunning) return;

		// Explore physical neighbors
		foreach (Vector2 direction in directions)
		{
			var exploreArounds = searchCenter.GetPosition() + direction;

			if (wayPointDict.TryGetValue(exploreArounds, out WayPoint neighbor))
			{
				if (!neighbor.isExplored && !queue.Contains(neighbor))
				{
					queue.Enqueue(neighbor);
					neighbor.exploredFrom = searchCenter;
				}
			}
		}

		// Explore logical neighbors
		foreach (var logicalNeighbor in searchCenter.logicalNeighbors)
		{
			if (logicalNeighbor != searchCenter.exploredFrom) 
			{
				if (!logicalNeighbor.isExplored && !queue.Contains(logicalNeighbor))
				{
					queue.Enqueue(logicalNeighbor);
					logicalNeighbor.exploredFrom = searchCenter;
				}
			}
		}
	}

	 // Store all walkable grids in the scene into the dictionary wayPointDict
	private void LoadAllWayPoints() 
	{
		wayPointDict.Clear();
		var wayPoints = FindObjectsByType<WayPoint>(FindObjectsSortMode.None);
		foreach (WayPoint wayPoint in wayPoints)
		{
			var tempWayPoint = wayPoint.GetPosition();
			if (wayPointDict.ContainsKey(tempWayPoint))
			{

			}
			else
			{
				// Only grids whose type matches the type of the large block where zera is located can participate in the BFS (Breadth-First Search) algorithm. 
				// Therefore, the map must be divided into multiple levels (type).
				if (wayPoint.type == zera.inType) 
				{
					wayPointDict.Add(tempWayPoint, wayPoint);
				}
			}
		}
	}

	private void BFS()
	{
		queue.Enqueue(startPoint);

		while (queue.Count > 0 && isRunning)
		{
			searchCenter = queue.Dequeue(); // First in, first out
			StopIfSearchEnd();
			ExploreAround();
			searchCenter.isExplored = true;
		}
	}

	private void StopIfSearchEnd()
	{
		if (searchCenter == endPoint)
		{
			isRunning = false;
		}
	}


	private void CreatePath()
	{
		if (queue.Count == 0)
		{
			if (searchCenter != endPoint)
			{
				Debug.Log("End of road");
				return;
			}
		}

		path.Add(endPoint);

		WayPoint prePoint = endPoint.exploredFrom;

		while (prePoint != startPoint)
		{
			path.Add(prePoint);
			prePoint = prePoint.exploredFrom;
		}

		path.Add(startPoint);
		path.Reverse();
	}
}
