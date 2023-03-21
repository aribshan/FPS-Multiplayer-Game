using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    public float intensityX;
    public float intensityY;
    public float smooth;
    private Quaternion originRotation;
    [SerializeField] MouseLook _mouselook;
    // Start is called before the first frame update
    void Start()
    {
        originRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSway();
    }

    void UpdateSway()
    {
        float t_x_mouse = Input.GetAxis("Mouse X") * _mouselook.mouseSens * Time.deltaTime;
        float t_y_mouse = Input.GetAxis("Mouse Y") * _mouselook.mouseSens * Time.deltaTime; 
        Quaternion t_x_adj = Quaternion.AngleAxis(-intensityX * t_x_mouse, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(intensityY * t_y_mouse, Vector3.right);
        Quaternion targetRotation = originRotation * t_x_adj * t_y_adj;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);

    }
}
