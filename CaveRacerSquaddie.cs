using UnityEngine;
using System.Collections;

namespace Cloud race v1
{	
	public class CaveRacerSquaddie : MonoBehaviour 
	{
		public float FollowSpeed = 2f;
		public Vector3 HidingPlace;

		protected CaveRacer _target;
		protected Vector3 _newPosition;
		protected Vector3 _initialPosition;
		protected float yDifference;
		protected float initialDifference;
		protected float followSpeedDifference;

		protected bool _patrolling=true;

		protected virtual void Start () 
		{	
			_initialPosition = transform.position;
		}
	
		protected virtual void Update () 
		{
			if (_target==null)
			{
				return;
			}

			Patrol();

		}

		protected virtual void Patrol()
		{
			if (_patrolling)
			{
				_newPosition = transform.position;
				//_newPosition = new Vector3(transform.position.x,_target.transform.position.y + yDifference,transform.position.z);
				_newPosition = _newPosition + Vector3.up * Mathf.Sin(Time.time*1f * initialDifference )*1f ;
				_newPosition = _newPosition + Vector3.right * Mathf.Sin(Time.time*1f)*FollowSpeed/10;
				transform.position = Vector3.Lerp(transform.position,_newPosition,Time.deltaTime * FollowSpeed );
			}
		}

		protected virtual IEnumerator SquadEvac(float delay)
		{
			yield return new WaitForSeconds(delay);
			while (Vector3.Distance(transform.position,HidingPlace)>0.1f)
			{
				transform.position = Vector3.Lerp(transform.position,HidingPlace,Time.deltaTime * 0.2f );
				yield return null;
			}
			yield return new WaitForSeconds(45f);
			//StartCoroutine(SquadCome(10f));
		}

		protected virtual IEnumerator SquadCome(float stayDuration)
		{
			_patrolling=true;
			while (Vector3.Distance(transform.position,HidingPlace)>0.1f)
			{
				transform.position = Vector3.Lerp(transform.position,_initialPosition,Time.deltaTime * 1f );
				yield return null;
			}
			yield return new WaitForSeconds(stayDuration);
			StartCoroutine(SquadEvac(1f));
		}

		protected virtual void Initialize()
		{
			StartCoroutine(SetupSquaddies());
		}

		protected virtual IEnumerator SetupSquaddies()
		{
			followSpeedDifference = UnityEngine.Random.Range(0.1f,1f);
			yield return new WaitForSeconds(followSpeedDifference);
			_target = LevelManager.Instance.CurrentPlayableCharacters[0].GetComponent<CaveRacer>();
			yDifference = transform.position.y - _target.transform.position.y;
			initialDifference = transform.position.z;
		}

		protected virtual void GameStart()
		{
			StartCoroutine(SquadEvac(5f));
		}

		protected virtual void OnEnable()
		{
			EventManager.StartListening("PlayableCharactersInstantiated",Initialize);
			EventManager.StartListening("GameStart",GameStart);
		}

		protected virtual void OnDisable()
		{
			EventManager.StopListening("PlayableCharactersInstantiated",Initialize);
			EventManager.StopListening("GameStart",GameStart);
		}
	}
}
