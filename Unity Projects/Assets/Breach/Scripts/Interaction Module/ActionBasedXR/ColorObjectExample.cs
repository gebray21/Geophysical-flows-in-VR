using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class ColorObjectExample : MonoBehaviour
{
   //reference to our action reference that is the toggle action
    public InputActionReference colorReference = null;
    private MeshRenderer meshRenderer = null;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        float value = colorReference.action.ReadValue<float>();
        UpdateColor(value);
    }

    private void UpdateColor(float value)
    {
        meshRenderer.material.color = new Color(value, value, value);
    }
}
