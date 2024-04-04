using Stride.CommunityToolkit.Engine;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Input;
using Stride.Physics;
using Stride.Rendering.Sprites;
using System;

namespace Etsu
{
    public class CardAction : SyncScript
    {
        public Card Card { get; set; }
        public SpriteFromSheet SpriteSheet { get; set; }
        public CameraComponent Camera { get; set; }

        private float animationProgress = 0f;
        private bool animationBusy = false;
        private int animationCurrentSprite = 0;
        private CardAnimationDirection animationDirection;

        private float startAnimationProgress = 0f;
        private bool startAnimationBusy = false;

        private Simulation simulation;

        public override void Start()
        {
            simulation = this.GetSimulation();

            SpriteSheet.CurrentFrame = Card.BackSpriteStartIndex;
            Entity.Transform.Scale *= startAnimationProgress;
            startAnimationBusy = true;
        }

        public override void Update()
        {
            float deltaTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;
            Vector3 mouseDetectionStart = new(Input.MousePosition, 2f);
            Vector3 mouseDetectionEnd = new(Input.MousePosition, -2f);

            if (startAnimationBusy)
            {
                AnimateCardSpawn(deltaTime);
            }
            else if (!animationBusy && Input.IsMouseButtonPressed(MouseButton.Left))
            {
                //Texture backBuffer = GraphicsDevice.Presenter.BackBuffer;
                //Viewport viewport = new(0, 0, backBuffer.Width, backBuffer.Height);
                //Vector3 nearPosition = viewport.Unproject(new Vector3(Input.MousePosition, 0.0f), Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);
                //Vector3 farPosition = viewport.Unproject(new Vector3(Input.MousePosition, 1.0f), Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);

                //HitResult hitResult = simulation.Raycast(nearPosition, farPosition);
                HitResult hitResult = Entity.Scene.GetCamera().RaycastMouse(this);
                if (hitResult.Succeeded)
                {
                    if (hitResult.Collider.Entity == this.Entity)
                    {
                        animationCurrentSprite = Card.IsFrontFacing ? Card.FrontSpriteStartIndex : Card.BackSpriteStartIndex;
                        animationDirection = Card.IsFrontFacing ? CardAnimationDirection.FrontToBack : CardAnimationDirection.BackToFront;
                        animationBusy = true;
                    }
                }
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
            float animationDuration = 2f;
            float spriteSwitchTime = animationDuration / 2f;
            int totalFrames = 3;

            animationProgress += deltaTime;

            int fromSprite = animationDirection == CardAnimationDirection.FrontToBack ? Card.FrontSpriteStartIndex : Card.BackSpriteStartIndex;
            int toSprite = animationDirection == CardAnimationDirection.FrontToBack ? Card.BackSpriteStartIndex : Card.FrontSpriteStartIndex;

            if (animationProgress <= spriteSwitchTime)
            {
                int currentFrameIndex = (int)Math.Floor(animationProgress / spriteSwitchTime * totalFrames);
                animationCurrentSprite = fromSprite + currentFrameIndex;
            }
            else if (animationProgress < animationDuration)
            {
                float timeElapsedSecondHalf = animationProgress - spriteSwitchTime;
                int currentFrameIndex = (int)Math.Floor(timeElapsedSecondHalf / spriteSwitchTime * totalFrames);
                animationCurrentSprite = toSprite + (totalFrames - 1 - currentFrameIndex);
            }

            if (animationProgress >= animationDuration)
            {
                animationDirection = CardAnimationDirection.None;
                animationBusy = false;
            }

            this.SpriteSheet.CurrentFrame = animationCurrentSprite;
        }
    }
}