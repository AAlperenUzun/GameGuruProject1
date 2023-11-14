using System;
using Lean.Touch;
using UnityEngine;

public class InputController
{
    public event Action<Vector3> Clicked; 
    
    private readonly Camera _camera;
    private readonly Plane _inputPlane;
    private bool _inputEnabled;

    public InputController()
    {
        _camera = Camera.main;
        _inputPlane = new Plane(Vector3.back, Vector3.zero);
        _inputEnabled = true;
        
        RegisterListeners();
    }

    public void Dispose()
    {
        UnregisterListeners();
    }
    
    private void RegisterListeners()
    {
        LeanTouch.OnFingerDown += OnFingerDown;
    }
    
    private void UnregisterListeners()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
    }

    private void OnFingerDown(LeanFinger finger)
    {
        if (!_inputEnabled)
        {
            return;
        }
        
        Ray ray = _camera.ScreenPointToRay(finger.ScreenPosition);

        if (_inputPlane.Raycast(ray, out float enter))
        {
            Vector3 worldPoint = ray.GetPoint(enter);
            Clicked?.Invoke(worldPoint);
        }
    }
}