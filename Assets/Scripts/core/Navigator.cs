using UnityEngine;

namespace tuesdayPizza
{
    public abstract class Navigator
    {
        private Navigator _parent;

        public Navigator(Navigator parent)
        {
            _parent = parent;
        }
        
        public virtual void go()
        {
            App.instance.setActiveNavigator(this);
        }

        public virtual void exit()
        {
            if (_parent != null)
            {
                _parent.go();
            }
            else
            {
                Application.Quit();
            }
        }

        public virtual void tick()
        {
        }
    }

}
