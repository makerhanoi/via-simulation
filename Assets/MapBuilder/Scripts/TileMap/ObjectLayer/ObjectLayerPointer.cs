﻿using System.Collections;
using System.Collections.Generic;
using Koi.Common;
using Koi.UI;
using UnityEngine;
using UnityEngine.UI;

public class ObjectLayerPointer : MonoBehaviour
{
    public enum State
    {
        S0_Idle,
        S1_OnMoveX,
        S2_OnMoveY,
        S3_OnMoveXY
    }

    [SerializeField] RectTransform root;
    [SerializeField] RectDrag axisX;
    [SerializeField] RectDrag axisY;
    [SerializeField] RectDrag center;
    [SerializeField] Slider rotationSlider;

    [Header("Debug")]
    [SerializeField] State m_state = State.S0_Idle;

    [SerializeField] TrafficSignObject m_SelectingObject;

    void Start()
    {
        InitListener();
    }

    void InitListener()
    {
        rotationSlider.onValueChanged.AddListener(OnRotChanged);

        center.AddBeginDragListener(OnBeginDrag);
        center.AddDraggingListener(OnDragging);
        center.AddFinishDragListener(OnFinishDrag);

        axisX.AddBeginDragListener(OnBeginDragX);
        axisX.AddDraggingListener(OnDraggingX);
        axisX.AddFinishDragListener(OnFinishDragX);

        axisY.AddBeginDragListener(OnBeginDragY);
        axisY.AddDraggingListener(OnDraggingY);
        axisY.AddFinishDragListener(OnFinishDragY);
    }

    void OnRotChanged(float pValue)
    {
        m_SelectingObject?.UpdateRot(pValue);
    }

    public void OnSelected(TrafficSignObject pObject)
    {
        Debug.LogWarning("OnSelected obj " + pObject.name);
        if (m_SelectingObject != null && pObject != m_SelectingObject)
        {
            m_SelectingObject.OnUnSelect();
        }

        Debug.LogWarning("Setup new obj with rot = " + pObject.Rotation);
        this.m_SelectingObject = pObject;
        this.rotationSlider.value = pObject.Rotation;
    }

    public void OnUnSelected()
    {
        if (m_SelectingObject != null)
        {
            m_SelectingObject.OnUnSelect();
            m_SelectingObject = null;
        }
    }

    void UpdateSelectingObjectPos()
    {
        if (m_SelectingObject != null)
        {
            m_SelectingObject.transform.position = root.position;
        }   
    }

    Vector2 savedLocalTouchPos;
    Vector2 savedLocalPos;

    void OnBeginDrag(int pointerId, Vector2 screenPos, int mouseId)
    {
        Debug.LogWarning("On Begin Drag " + screenPos);
        if (mouseId != 0)
        {
            return;
        }
        m_state = State.S3_OnMoveXY;
        
        savedLocalTouchPos = GetTouchLocalPos();
        savedLocalPos = root.localPosition;
    }

    void OnDragging(int pointerId, Vector2 screenPos, int mouseId)
    {
        if (mouseId != 0)
        {
            return;
        }

        Vector2 currentLocalTouchPos = GetTouchLocalPos();
        root.localPosition = savedLocalPos + currentLocalTouchPos - savedLocalTouchPos;

        UpdateSelectingObjectPos();
    }

    void OnFinishDrag(int pointerId)
    {
        m_state = State.S0_Idle;
    }

    void OnBeginDragX(int pointerId, Vector2 screenPos, int mouseId)
    {
        Debug.LogWarning("On Begin Drag " + screenPos);
        if (mouseId != 0)
        {
            return;
        }

        m_state = State.S1_OnMoveX;
        
        savedLocalTouchPos = GetTouchLocalPos();
        savedLocalPos = root.localPosition;
    }

    void OnDraggingX(int pointerId, Vector2 screenPos, int mouseId)
    {
        if (mouseId != 0)
        {
            return;
        }

        Vector2 currentLocalTouchPos = GetTouchLocalPos();
        var newPos = savedLocalPos + currentLocalTouchPos - savedLocalTouchPos;
        newPos.y = root.localPosition.y;
        root.localPosition = newPos;

        UpdateSelectingObjectPos();
    }

    void OnFinishDragX(int pointerId)
    {
        m_state = State.S0_Idle;
    }

    void OnBeginDragY(int pointerId, Vector2 screenPos, int mouseId)
    {
        Debug.LogWarning("On Begin Drag " + screenPos);
        if (mouseId != 0)
        {
            return;
        }

        m_state = State.S2_OnMoveY;

        savedLocalTouchPos = GetTouchLocalPos();
        savedLocalPos = root.localPosition;
    }

    void OnDraggingY(int pointerId, Vector2 screenPos, int mouseId)
    {
        if (mouseId != 0)
        {
            return;
        }

        Vector2 currentLocalTouchPos = GetTouchLocalPos();
        var newPos = savedLocalPos + currentLocalTouchPos - savedLocalTouchPos;
        newPos.x = root.localPosition.x;
        root.localPosition = newPos;

        UpdateSelectingObjectPos();
    }

    void OnFinishDragY(int pointerId)
    {
        m_state = State.S0_Idle;
    }


    Vector2 GetTouchLocalPos()
    {
        Vector2 currentLocalTouchPos;
        bool onMap = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent.GetComponent<RectTransform>(), Input.mousePosition,
            canvas.worldCamera, out currentLocalTouchPos);
        return currentLocalTouchPos;
    }


    Canvas m_canvas;
    Canvas canvas
    {
        get
        {
            if (m_canvas == null)
            {
                m_canvas = UMonoUtils.FindComponentFromChild<Canvas>(transform);
            }
            return m_canvas;
        }
    }

    public bool OnSelecting => m_SelectingObject != null;
    public TrafficSignObject SelectingObject => m_SelectingObject;
}
