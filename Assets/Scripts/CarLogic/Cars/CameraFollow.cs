using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{

	[SerializeField] private Transform carTransform;
    [Range(1, 10), SerializeField] private float followSpeed = 2;
    [SerializeField, Range(1, 10)] private float lookSpeed = 5;

	private Vector3 initialCameraPosition;
	private Vector3 initialCarPosition;
	private Vector3 absoluteInitCameraPosition;

	private void Start()
	{
		initialCameraPosition = gameObject.transform.position;
		initialCarPosition = carTransform.position;
		absoluteInitCameraPosition = initialCameraPosition - initialCarPosition;
	}

	private void FixedUpdate()
	{
		Vector3 _lookDirection = carTransform.position - transform.position;
		Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);

		Vector3 _targetPos = absoluteInitCameraPosition + carTransform.transform.position;
		transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);
	}

}
