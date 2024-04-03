using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;

namespace Etsu
{
    public class CardAction : SyncScript
    {
        public Card Card { get; set; }

        private float animationSpent = 0f;
        private bool animationBusy = false;

        private float startAnimationProgress = 0f;
        private bool startAnimationBusy = false;

        public override void Start()
        {
            Entity.Transform.Scale *= startAnimationProgress;
            startAnimationBusy = true;
        }

        public override void Update()
        {
            float deltaTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;

            if (startAnimationBusy)
            {
                AnimateCardSpawn(deltaTime);
            }
            else if (!animationBusy && Input.IsMouseButtonPressed(MouseButton.Left))
            {
                animationBusy = true;
            }
            else if (animationBusy)
            {
                AnimateCard(deltaTime);
            }
        }

        private void AnimateCardSpawn(float deltaTime)
        {
            float startAnimationDuration = 0.2f;

            startAnimationProgress += deltaTime;
            if (startAnimationProgress < startAnimationDuration)
            {
                Entity.Transform.Scale = new Vector3(startAnimationProgress * (1f / startAnimationDuration));
            }
            else
            {
                startAnimationBusy = false;
                Entity.Transform.Scale = new Vector3(1f);
            }
        }

        private void AnimateCard(float deltaTime)
        {
            float animationDuration = 0.2f;
        }
}
}