using Handlers;
using UnityEngine;

public class NukeBomb : MonoBehaviour
{
    public GameObject explosion;
    public float timeBeforeLaunch = 1f;
    public float travelDuration = 0.5f;
    public Vector3 startPosition;
    public Transform target;
    public AnimationCurve trajectory;
    [HideInInspector] public ShakeTransform cameraShakeTransform;
    public ShakeTransformEventData nukeShake;
    public AudioSource flightEffect;
    

    private float startLauchTime;
    private float startAnimationTime;
    private bool launched;
    private bool exploded;
    private void Start()
    {
        startAnimationTime = Time.time;
    }

    private void Update()
    {
        if (!launched)
        {
            float elapsed = Time.time - startAnimationTime;
            if (elapsed >= timeBeforeLaunch)
            {
                Launch();
            }
        }
        else if(!exploded)
        {
            float elapsed = Time.time - startLauchTime;
            float percent = elapsed / travelDuration;
            float yPercent = trajectory.Evaluate(percent);
            float x = Mathf.Lerp(startPosition.x, target.position.x, percent);
            float y = Mathf.Lerp(startPosition.y, target.position.y, yPercent);
            float z = Mathf.Lerp(startPosition.z, target.position.z, percent);
            Vector3 oldPos = transform.position;
            Vector3 newPos = new Vector3(x, y, z);
            transform.position = newPos;
            transform.forward = (newPos - oldPos).normalized;
            if (percent >= 1f)
            {
                Explode();
            }
        }
    }

    private void Launch()
    {
        launched = true;
        startLauchTime = Time.time;
        transform.position = startPosition;
        flightEffect.Play();
    }

    private void Explode()
    {
        exploded = true;
        Instantiate(explosion, target.position, Quaternion.identity);
        //faire un spherecast
        GameManager.Instance.ShakeCameras(nukeShake);
        Destroy(gameObject);
    }
}
