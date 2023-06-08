using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class Crosshair : MonoBehaviour
{

    public Vector3 screenPosition;
    public Vector3 worldPosition;

    [SerializeField] float crosshairSpeed;
    Rigidbody2D myRigidbody;
    public GameObject player;
    public GameObject hotdog;
    SpriteRenderer spriteRenderer;
    public Camera cam;
    public GameObject sendBirds;
    public bool ivehitsomething;
    public Transform thingivehit;

    public enum CrosshairState { isactive, isdisabled }
    public CrosshairState crosshairstate;
    // Start is called before the first frame update
    void Start()
    {
        InitializeCrosshair();
    }
    public void InitializeCrosshair()
    {
        crosshairstate = CrosshairState.isdisabled;
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("PJohns");
        hotdog = GameObject.Find("Hotdog");
        cam = Camera.main;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        CrossHairState();
    }
    public void CrossHairState()
    {
        if (crosshairstate == CrosshairState.isactive)
        {
            spriteRenderer.enabled = true;
            MoveCrosshair();
            Targetting();
        }
        if (crosshairstate == CrosshairState.isdisabled)
        {
            spriteRenderer.enabled = false;
        }
    }
    private void MoveCrosshair()
    {
        // Mouse Controls
        screenPosition = Input.mousePosition;
        screenPosition.z = Camera.main.nearClipPlane + 2f;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = worldPosition;
    }
    private void Targetting()
    {
        RaycastHit hit;
        LayerMask layerMask = 1 << 8;
        Vector3 direction = (transform.position - cam.transform.position).normalized;
        Ray ray = cam.ScreenPointToRay(direction);
        Debug.DrawRay(cam.transform.position, direction * 15f, Color.green);
        Physics.Raycast(cam.transform.position, direction, out hit, 15f, layerMask);
        thingivehit = hit.transform;
        if (hit.collider != null)
        {
            transform.localScale += transform.localScale * Time.deltaTime * 1.5f;
            if (transform.localScale.x >= 1.5f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            ivehitsomething = true;
            player.GetComponent<PJohns>().target = hit.collider.gameObject;
            if (Input.GetKeyUp(KeyCode.Q))
            {
                crosshairstate = CrosshairState.isdisabled;
                thingivehit = null;
            }
        }
        else if (hit.collider == null)
        {
            player.GetComponent<PJohns>().target = null;
            thingivehit = null;
            ivehitsomething = false;
        }
    }
}
