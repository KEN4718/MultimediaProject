using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ZeraMovement : MonoBehaviour
{
	public int inType = 1; // 在哪一个大块中，便是哪个type
	public LayerMask groundLayer;
	public bool canClick = true;
	public ParticleSystem indicatorEffect;
	public float walkingSpeed = 0.5f;

	public AudioSource footstepAudioSource; // Audio source for footstep sounds
	public AudioSource clickSuccessAudioSource; // Audio source for click success sound
	public AudioClip footstepClip;           // Footstep sound effect
	public AudioClip clickSuccessClip;      // Sound effect for successful click on walkable point
	public bool isInDialogue = false;


	private void Start()
	{
		CheckWhichGridIn();
		StartCoroutine(GetTheVelocity());
	}

	private void Update()
	{
		GetTheDir();
	}

	private void FixedUpdate()
	{
		CheckWhichGridIn();
		MoveZeraByClickTheDestination();
	}


	private void MoveZeraByClickTheDestination()
	{
		// Left-click logic
		if (Input.GetMouseButtonDown(0) && canClick&& !isInDialogue)
		{
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit mouseHit;

			if (Physics.Raycast(mouseRay, out mouseHit))
			{
				if (mouseHit.transform.GetComponent<WayPoint>() != null)
				{
					if (mouseHit.transform.GetComponent<WayPoint>().type == inType)
					{
						PathFinding.pathFinding.path.Clear();
						PathFinding.pathFinding.queue.Clear();
						foreach (WayPoint wayPoint in FindObjectsByType<WayPoint>(FindObjectsSortMode.None))
						{
							wayPoint.transform.parent = null;
							wayPoint.isExplored = false;
							wayPoint.exploredFrom = null;
						}
						PathFinding.pathFinding.isRunning = true;

						CheckWhichGridIn();
						PathFinding.pathFinding.endPoint = mouseHit.transform.GetComponent<WayPoint>();

						List<WayPoint> path = new List<WayPoint>();
						path.Clear();
						if (PathFinding.pathFinding.startPoint != PathFinding.pathFinding.endPoint)
						{
							path = PathFinding.pathFinding.GetPath();
						}
						StartCoroutine(FindWayPoint(path));

						clickSuccessAudioSource.PlayOneShot(clickSuccessClip);
						ShowClickEffect(mouseHit.point, Color.white, Color.black);
					}
					else
					{
						Debug.Log("WayPoint type: " + mouseHit.transform.GetComponent<WayPoint>().type + ", inType: " + inType);
						ShowClickEffect(mouseHit.point, Color.red, Color.clear);
					}
				}
			}
		}

		// Right-click logic for yellow indicator
		if (Input.GetMouseButtonDown(1)) // Detect right mouse button
		{
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit mouseHit;

			if (Physics.Raycast(mouseRay, out mouseHit))
			{
				// Display the yellow indicator effect at the clicked position
				ShowClickEffect(mouseHit.point, Color.yellow, Color.clear);
			}
		}
	}


	private void ShowClickEffect(Vector3 position, Color startColor, Color endColor)
	{
		// Set the indicator position
		indicatorEffect.transform.position = position;
		indicatorEffect.Play();

		// Get the material of the indicator
		Material indicatorMaterial = indicatorEffect.GetComponent<Renderer>().material;

		// Create a DOTween animation sequence
		Sequence s = DOTween.Sequence();
		s.AppendCallback(() => indicatorEffect.GetComponentInChildren<ParticleSystem>().Play()); // Play particle system
		s.Append(indicatorMaterial.DOColor(startColor, 0.1f)); // Start color transition
		s.Append(indicatorMaterial.DOColor(endColor, 0.3f).SetDelay(0.2f)); // End color transition
		s.Append(indicatorMaterial.DOColor(Color.clear, 0.3f)); // Fade out
	}

	public Animator anim;
	IEnumerator FindWayPoint(List<WayPoint> pathWayPoints)
	{
		// 如果正在对话，则不允许角色移动
    if (isInDialogue)
    {
        yield break;  // 结束协程，不继续执行后面的代码
    }
		Sequence s = DOTween.Sequence();
		canClick = false;  //当角色去往指定格子的过程中，游戏不接受玩家的点击或者任何才操作
		transform.parent = null;
		anim.SetBool("IsRunning", true);
		footstepAudioSource.PlayOneShot(footstepClip);
		foreach (WayPoint wayPoint in pathWayPoints)
		{
			s.Append(transform.DOMove(wayPoint.transform.position + transform.up * 1.5f, walkingSpeed).SetEase(Ease.Linear));
			yield return new WaitForSeconds(walkingSpeed);
		}
		anim.SetBool("IsRunning", false);
		footstepAudioSource.Stop();
		foreach (WayPoint wayPoint in FindObjectsByType<WayPoint>(FindObjectsSortMode.None))
		{
			wayPoint.transform.parent = wayPoint.father; //当角色到达指定的格子，所有的格子回到父对象中
		}
		canClick = true;
		transform.parent = hitGrid.transform;
	}

	RaycastHit hitGrid;
	public void CheckWhichGridIn()
	{
		if (Physics.Raycast(transform.position, -transform.up, out hitGrid, 1))
		{
			if (hitGrid.transform.GetComponent<WayPoint>() != null)
			{
				PathFinding.pathFinding.startPoint = hitGrid.collider.GetComponent<WayPoint>();
				inType = hitGrid.transform.GetComponent<WayPoint>().type; //zera的inType总是与其所在的格子type值同步,所以只会将与zera的inType相同的格子加入到BFS算法当中
																		  //this.gameObject.layer = hitGrid.collider.gameObject.layer;	//zera只能在与其相同type的格子中寻路，可通过传送点功能跨越type
			}
		}
	}

	private Vector3 prePos, currentPos;
	public Vector3 curretVelocity;
	public Transform zeraRender;
	private float currentAngle;

	public IEnumerator GetTheVelocity()
	{
		while (true)
		{
			if (this == null)
			{
				yield break;
			}
			prePos = transform.position;
			yield return new WaitForSeconds(0.01f);

			if (this == null)
			{
				yield break;
			}
			currentPos = transform.position;
			curretVelocity = currentPos - prePos;
			yield return new WaitForSeconds(0.01f);
		}
	}

	void GetTheDir()
	{
		float angle = Mathf.Atan2(curretVelocity.z, curretVelocity.x) * Mathf.Rad2Deg;
		if ((curretVelocity.x != 0 || curretVelocity.z != 0) && curretVelocity.magnitude < 0.3f)
		{
			zeraRender.transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(zeraRender.transform.eulerAngles.y, 90f - angle, ref currentAngle, 0.2f), 0); //rigidbody的freeze pos ,freeze Rotation需要勾选

		}
		zeraRender.transform.GetChild(0).GetChild(1).gameObject.layer = this.gameObject.layer;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position - Vector3.up);
	}
}
