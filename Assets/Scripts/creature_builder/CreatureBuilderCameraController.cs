using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBuilderCameraController : MonoBehaviour
{
    [SerializeField]
    private float m_cameraSpeed = 1f;

    private Camera m_camera;

    private void Start()
    {
        m_camera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 cameraForward = m_camera.transform.forward;
        Vector3 cameraRight = m_camera.transform.right;
        Vector3 cameraPosition = m_camera.transform.position;

        float speedPerSecond = m_cameraSpeed * Time.deltaTime;

        // Movement
        if(Input.GetKey(KeyCode.W))
        {
            cameraPosition += cameraForward * speedPerSecond;
        }
        
        if(Input.GetKey(KeyCode.S))
        {
            cameraPosition -= cameraForward * speedPerSecond;
        }
                
        if(Input.GetKey(KeyCode.D))
        {
            cameraPosition += cameraRight * speedPerSecond;
        }

        if (Input.GetKey(KeyCode.A))
        {
            cameraPosition -= cameraRight * speedPerSecond;
        }

        if (Input.GetKey(KeyCode.E))
        {
            cameraPosition += Vector3.up * speedPerSecond;
        }
        
        if(Input.GetKey(KeyCode.Q))
        {
            cameraPosition -= Vector3.up * speedPerSecond;
        }

        // Camera
        if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            float y = Input.GetAxis("Mouse X");
            float x = Input.GetAxis("Mouse Y");            

            Vector3 rotateValue = new Vector3(x, y * -1, 0);
            m_camera.transform.eulerAngles = m_camera.transform.eulerAngles - rotateValue;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        m_camera.transform.position = cameraPosition;
    }
}
