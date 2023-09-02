namespace tuesdayPizza
{
    public abstract class GameNavigator : Navigator
    {
        private readonly SceneAccessor _sceneAccessor;

        public GameNavigator(SceneAccessor sceneAccessor, Navigator parent) : base(parent)
        {
            _sceneAccessor = sceneAccessor;
        }

        public void kill()
        {
        }
    }
}