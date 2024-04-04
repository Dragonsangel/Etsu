using Stride.CommunityToolkit.Engine;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.Physics;
using Stride.Rendering.Sprites;
using System;

namespace Etsu
{
    public class CardAction : SyncScript
    {
        public Card Card { get; set; }
        public Entity SpriteChild { get; set; }
        public SpriteFromSheet SpriteSheet { get; set; }
        public CameraComponent Camera { get; set; }

        private float animationProgress = 0f;
        private bool animationBusy = false;
        private int animationCurrentSprite = 0;
        private CardAnimationDirection animationDirection;

        private float startAnimationProgress = 0f;
        private bool startAnimationBusy = false;

        public override void Start()
        {
            SpriteSheet.CurrentFrame = Card.BackSpriteStartIndex;
            SpriteChild.Transform.Scale *= startAnimationProgress;
            startAnimationBusy = true;

            if (Card is null)
            {
                Card = new()
                {
                    BackSpriteStartIndex = 0,
                    FrontSpriteStartIndex = 6,
                    IsFrontFacing = false
                };
            }
        }

        public override void Update()
        {
            if (startAnimationBusy)
            {
                AnimateCardSpawn();
            }
            else if (!animationBusy && Input.IsMouseButtonPressed(MouseButton.Left))
            {
                HitResult hitResult = Camera.RaycastMouse(this);
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
                AnimateCard();
            }
        }

        private void AnimateCardSpawn()
        {
            float startAnimationDuration = 0.2f;

            startAnimationProgress += Game.DeltaTime();
            if (startAnimationProgress < startAnimationDuration)
            {
                SpriteChild.Transform.Scale = new Vector3(startAnimationProgress * (1f / startAnimationDuration));
            }
            else
            {
                startAnimationBusy = false;
                SpriteChild.Transform.Scale = new Vector3(1f);
            }
        }

        private void AnimateCard()
        {
            float animationDuration = 0.2f;
            float spriteSwitchTime = animationDuration / 2f;
            int totalFrames = 3;

            animationProgress += Game.DeltaTime();

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
                animationProgress = 0f;
                Card.IsFrontFacing = !Card.IsFrontFacing;
            }

            this.SpriteSheet.CurrentFrame = animationCurrentSprite;
        }
    }
}