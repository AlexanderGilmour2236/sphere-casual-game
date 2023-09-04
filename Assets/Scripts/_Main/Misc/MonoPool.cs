using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace misc
{
    public class MonoPool<T> where T : Component, IPoolObject
    {
        private readonly List<T> _used = new List<T>();
        private readonly List<T> _free = new List<T>();
            
        private readonly Transform _parent;
        private readonly Func<T> _getMethod;

        public MonoPool(Transform parent, Func<T> getMethod = null)
        {
            this._parent = parent;
            if (getMethod != null) _getMethod = getMethod;
        }

        public T GetObject()
        {
            T component;
            if (_free.Count == 0)
            {
                if (_getMethod == null)
                {
                    component = new GameObject().AddComponent<T>();
                    component.transform.SetParent(_parent, false);

                    _used.Add(component);
                    component.onSpawn();
                    return component;
                }
                else
                {
                    GameObject newObject = _getMethod().gameObject;
                    newObject.transform.SetParent(_parent);
                    component = newObject.GetComponent<T>();
                    _used.Add(component);
                    component.onSpawn();
                    return component;
                }
            }

            component = _free[0];
            _free.Remove(component);
            component.gameObject.SetActive(true);
            _used.Add(component);
            component.onSpawn();
            return component;
        }


        public void ReleaseObject(T obj)
        {
            _used.Remove(obj);
            obj.onRelease();
            obj.gameObject.SetActive(false);
            _free.Add(obj);
        }

        public void Dispose()
        {
            foreach (T component in _free)
            {
                Object.Destroy(component.gameObject);
            }
            foreach (T component in _used)
            {
                Object.Destroy(component.gameObject);
            }
            _free.Clear();
            _used.Clear();
        }

        public bool ContainsItem(T item)
        {
            return _used.Contains(item) || _free.Contains(item);
        }
    }
}
