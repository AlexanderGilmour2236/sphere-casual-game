using tuesdayPizza;

namespace sphereGame
{
    public class SGServicesLoader : ServicesLoader
    {
        public override void loadServices()
        {
            base.loadServices();
            invokeServicesLoaded();
        }
    }
}