using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(0, 10)]
    public float smoothCam;
    public Vector3 minValue, maxValue;
    void FixedUpdate()
    {
        Fallow();
    }
    void Fallow()
    {
        Vector3 targetPosition = target.position + offset;
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minValue.x, maxValue.x),
            Mathf.Clamp(targetPosition.y, minValue.y, maxValue.y),
            Mathf.Clamp(targetPosition.z, minValue.z, maxValue.z)
            );
        Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothCam * Time.deltaTime);
        transform.position = smoothPosition;
    }

}
