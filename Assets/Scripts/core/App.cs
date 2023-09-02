namespace tuesdayPizza
{
    public abstract class App 
    {
        // game's parent controller 
        protected GameNavigator _gameNavigator;

        protected ServicesLoader _servicesLoader;
        protected Navigator _activeNavigator;

        private static App _instance;

        public App(GameNavigator gameNavigator, ServicesLoader servicesLoader)
        {
            if (_instance != null)
            {
                _instance.kill();
            }
            
            _instance = this;
            _gameNavigator = gameNavigator;
            _servicesLoader = servicesLoader;
        }

        public virtual void start()
        {
            _servicesLoader.servicesLoaded += onServicesLoaded;
            _servicesLoader.loadServices();
        }

        private void appTick()
        {
            if (_activeNavigator != null)
            {
                _activeNavigator.tick();
            }
        }

        protected virtual void onServicesLoaded()
        {
            TickManager.instance.tick += appTick;
            _gameNavigator.go();
        }

        public void setActiveNavigator(Navigator navigator)
        {
            _activeNavigator = navigator;
        }

        public virtual void kill()
        {
            _gameNavigator.kill();
            _gameNavigator = null;
            
            _servicesLoader.servicesLoaded -= onServicesLoaded;
            _servicesLoader = null;
            
            TickManager.instance.tick -= appTick;
        }

        public Navigator activeNavigator
        {
            get { return _activeNavigator; }
        }

        public static App instance
        {
            get { return _instance; }
        }
    }
}