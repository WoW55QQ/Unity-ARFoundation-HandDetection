﻿/*
    created by Jiadong Chen

    email:chenjd1024@gmail.com
 */

using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFoundationHandDetectDemo : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events.")]
    ARCameraManager m_CameraManager;

    public ARCameraManager cameraManager
    {
        get => m_CameraManager;
        set => m_CameraManager = value;
    }

    [SerializeField]
    Camera m_Cam;

    [SerializeField]
    HandDetector m_HandDetector;

    [SerializeField]
    GameObject m_TargetGo;

    public GameObject targetGo 
    {
        get => m_TargetGo;
        set => m_TargetGo = value;
    }

    public GameObject m_Go;


    void OnEnable()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.frameReceived += OnCameraFrameReceived;
        }

        if(m_HandDetector != null)
        {
            m_HandDetector.OnHandDeteced += OnHandDetectorCompleted;
        }
    }

    void OnDisable()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.frameReceived -= OnCameraFrameReceived;
        }

        if(m_HandDetector != null)
        {
            m_HandDetector.OnHandDeteced -= OnHandDetectorCompleted;
        }
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        
        #if !UNITY_EDITOR && UNITY_IOS

        var cameraParams = new XRCameraParams
        {
            zNear = m_Cam.nearClipPlane,
            zFar = m_Cam.farClipPlane,
            screenWidth = Screen.width,
            screenHeight = Screen.height,
            screenOrientation = Screen.orientation
        };

        XRCameraFrame frame;
        if (cameraManager.subsystem.TryGetLatestFrame(cameraParams, out frame))
        {
            if (m_HandDetector.IsIdle)
            {
                m_HandDetector.StartDetect(frame.nativePtr);
            }
        }

        #endif
    }

    private void OnHandDetectorCompleted(object sender, Vector2 pos)
    {
       var handPos = new Vector3();
       handPos.x = pos.x;
       handPos.y = 1 - pos.y;
       handPos.z = 4;//m_Cam.nearClipPlane;
       var handWorldPos = m_Cam.ViewportToWorldPoint(handPos);

       if(m_Go == null)
       {
          m_Go = Instantiate(m_TargetGo, handWorldPos, Quaternion.identity);
       }

       m_Go.transform.position = handWorldPos;
       m_Go.transform.LookAt(m_Cam.transform);
    }
}
