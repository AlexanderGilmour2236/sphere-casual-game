using System;
using System.Collections.Generic;
using UnityEngine;

namespace mics.input
{
    public class InputSystem : MonoBehaviour
    {
        private bool _isTapDown;
        private List<IInputListener> _inputListeners = new List<IInputListener>();

        public void addInputListener(IInputListener inputListener)
        {
            _inputListeners.Add(inputListener);
        }
        
        public void removeInputListener(IInputListener inputListener)
        {
            _inputListeners.Remove(inputListener);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isTapDown = true;
                invokeTapDown();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isTapDown = false;
                invokeTapUp();
            }
        }

        private void invokeTapDown()
        {
            foreach (IInputListener inputListener in _inputListeners)
            {
                inputListener.onTapDown();
            }
        }
        
        private void invokeTapUp()
        {
            foreach (IInputListener inputListener in _inputListeners)
            {
                inputListener.onTapUp();
            }
        }

        public bool isTapDown
        {
            get { return _isTapDown; }
        }
    }
}