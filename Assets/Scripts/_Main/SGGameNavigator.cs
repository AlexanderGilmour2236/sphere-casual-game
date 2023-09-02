using tuesdayPizza;

namespace sphereGame
{
    public class SGGameNavigator : GameNavigator
    {
        private readonly SphereNavigator _sphereNavigator;

        public SGGameNavigator(SceneAccessor sceneAccessor, Navigator parent) : base(sceneAccessor, parent)
        {
            _sphereNavigator = new SphereNavigator((SGSceneAccessor) sceneAccessor, this);
        }

        public override void go()
        {
            base.go();
            _sphereNavigator.go();
        }
    }
}