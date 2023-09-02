using System;

namespace tuesdayPizza
{
    public class ServicesLoader
    {
        public event Action servicesLoaded;

        public virtual void loadServices()
        {
            // init ads, IAPs and other
        }

        public virtual void invokeServicesLoaded()
        {
            servicesLoaded?.Invoke();
        }
    }
}