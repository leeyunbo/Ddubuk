using UnityEngine;

public class ARLocationDevCameraController : MonoBehaviour
{
    /// <summary>
    /// The mouse look/rotation sensitivity.
    /// </summary>
    public float MouseSensitivity = 1.0f;

    /// <summary>
    /// The walking speed
    /// </summary>
    public float speed = 1.0f;

    // Current orientation parameters
    float rotationY = 0.0f;
    float rotationX = 0.0f;

    // The initial location
    Location firstLocation;

    // The accumulated lat/lng displacement
    private Vector3 accumDelta;

    // Use this for initialization
    void Start()
    {
        // If we are not running on a device, make this the main 
        // camera, else, self-destruct.
        if (!Utils.IsARDevice())
        {
            var arCamera = GameObject.Find("AR Camera");

            if (arCamera != null)
            {
                arCamera.tag = "Untagged";
                arCamera.SetActive(false);
            }
            
            GetComponent<Camera>().gameObject.SetActive(true);

            gameObject.AddComponent<AudioListener>();
            gameObject.tag = "MainCamera";
        } else
        {
            Destroy(gameObject);
        }

        rotationX = transform.rotation.eulerAngles.x;
        rotationY = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        var forward = Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0));

        var initialPosition = transform.position;

        if (Input.GetKey("w"))
        {
            transform.Translate(
                forward * speed, Space.World
            );
        }

        if (Input.GetKey("s"))
        {
            transform.Translate(
                -forward * speed, Space.World
            );
        }

        if (Input.GetKey("d"))
        {
            transform.Translate(
                transform.right * speed, Space.World
            );
        }

        if (Input.GetKey("a"))
        {
            transform.Translate(
                -transform.right * speed, Space.World
            );
        }

        if (Input.GetKey("up"))
        {
            transform.Translate(
                transform.up * speed, Space.World
            );
        }

        var finalPosition = transform.position;
        var delta = finalPosition - initialPosition;

        var locMngr = ARLocationProvider.Instance;

        if (firstLocation == null)
        {
            firstLocation = locMngr.currentLocation.ToLocation();
        }

        accumDelta += delta * 0.00001f;

        //locMngr.UpdateMockLocation(new Location(
        //    firstLocation.latitude + accumDelta.z,
        //    firstLocation.longitude + accumDelta.x,
        //    0
        //));

        rotationY += Input.GetAxis("Mouse X") * MouseSensitivity;
        rotationX -= Input.GetAxis("Mouse Y") * MouseSensitivity;

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
    }
}
