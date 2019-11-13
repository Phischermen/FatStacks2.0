﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    Vector2 mouse_look;
    public float baseSensitivity = 5.0f;
    private float sensitivity = 5.0f;
    Player character;

    void Start()
    {
        GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat("FOV");
        SetSensitivity(PlayerPrefs.GetFloat("Sensitivity"));
        character = GetComponentInParent<Player>();
        if (Player.firstSpawnInScene)
            Look(new Vector3(transform.localEulerAngles.x, character.transform.localEulerAngles.y, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 mouse_delta = Vector2.one;
            mouse_delta.x *= Input.GetAxis("Mouse X") * /*Time.deltaTime*/ sensitivity;
            mouse_delta.y *= Input.GetAxis("Mouse Y") * /*Time.deltaTime*/ sensitivity;

            mouse_look += mouse_delta;
            //Clamp y between 90 and -90 degrees
            mouse_look.y = Mathf.Clamp(mouse_look.y, -90, 90);
            //Rotate character and camera
            transform.localRotation = Quaternion.AngleAxis(-mouse_look.y, Vector3.right);
            character.transform.localRotation = Quaternion.AngleAxis(mouse_look.x, character.transform.up);
        }
    }
    public void Look(Vector3 dir)
    {
        //Orient character based on camera rotation set in editor
        mouse_look.x = dir.y;
        if (dir.x > 270f)
        {
            mouse_look.y = 360 - dir.x;
        }
        else
        {
            mouse_look.y = -dir.x;
        }
    }
    public void SetSensitivity(float value)
    {
        sensitivity = baseSensitivity * value;
    }
}
