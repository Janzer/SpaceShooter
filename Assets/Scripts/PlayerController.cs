using UnityEngine;
using System.Collections;

[System.Serializable]

public class Boundary {
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {

    private Rigidbody rb;
    private AudioSource audioSource;

    public float speed;//移动速率
    public float tilt;//倾斜度
    public Boundary boundary;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public new Camera camera;

    private float nextFire;
    private Quaternion calibrationQuaternion;

    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        CalibrateAccellerometer();
    }

    void Update() {
        if (Input.GetButton("Fire1") && Time.time > nextFire) {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            audioSource.Play();
        }
    }

    void CalibrateAccellerometer() {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
    }

    Vector3 FixAccelleration(Vector3 acceleration) {
        Vector3 fixedAcceleration = calibrationQuaternion * acceleration;
        return fixedAcceleration;
    }

    void FixedUpdate() {
        float tiltFly = 0f;//机翼偏转
        Vector3 movement = rb.position;//移动位置

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical")) 
        {
            float horizontal = Input.GetAxis("Horizontal");//水平移动的距离
            float vertical = Input.GetAxis("Vertical");//垂直移动的距离
            
            movement = camera.ScreenToWorldPoint(new Vector3(horizontal * speed,
                vertical * speed) + camera.WorldToScreenPoint(rb.position));

            tiltFly = horizontal * speed * -tilt;
        } 
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            movement = camera.ScreenToWorldPoint(new Vector3(touchDeltaPosition.x, 
                touchDeltaPosition.y) + camera.WorldToScreenPoint(rb.position));

            tiltFly = touchDeltaPosition.x * speed * -tilt;
        }

        rb.position = new Vector3(
            Mathf.Clamp(movement.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(movement.z, boundary.zMin, boundary.zMax)
        );
        rb.rotation = Quaternion.Euler(0.0f, 0.0f, tiltFly);
    }

}
