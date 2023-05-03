using System.Collections;
using System.Collections.Generic;
using Castle;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera cam;
    [HideInInspector,SerializeField]
    private new Transform transform;
    public Transform Transform => transform ? transform : transform = base.transform;

    public float lag;
    public static float CameraShake;
    public float cameraShakeIntensity;
    public Vector2 currentPos;
    private Vector2 shakePos;
    // Start is called before the first frame update
    void Start()
    {
        currentPos = Transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPosition = GameManager.instance.player.Transform.position;
        var dir = targetPosition - currentPos;
        if (dir.sqrMagnitude > lag)
        {
            currentPos = targetPosition - dir.normalized * Mathf.Sqrt(lag);
        }
        else
        {
            currentPos = Vector2.Lerp(currentPos, targetPosition, Time.deltaTime * dir.sqrMagnitude);
        }

        if (CameraShake > 0)
        {
            var prev = Mathf.CeilToInt(CameraShake);
            CameraShake -= Time.unscaledDeltaTime * 10;
            if (Mathf.CeilToInt(CameraShake) < prev)
            {
                shakePos = Random.insideUnitCircle * cameraShakeIntensity;
            }
            Transform.Move2D(currentPos + shakePos);
            //Transform.position = new Vector3(currentPos.x,currentPos.y,Transform.position.z) + Random.insideUnitCircle;
        }
        else
        {
            Transform.Move2D(currentPos);
            CameraShake = 0;
        }
    }
}
