using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Input;
using Stride.Physics;
using Stride.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etsu;

public class CardBehaviour : SyncScript
{
    public SpriteSheet CardBorderSprite;

    private CameraComponent camera;

    public override void Start()
    {
        camera = Entity.Get<CameraComponent>();
        CardStorage.Cards = new();
    }

    public override void Update()
    {
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            ClearAllCards();

            Texture backBuffer = GraphicsDevice.Presenter.BackBuffer;
            Viewport viewport = new(0, 0, backBuffer.Width, backBuffer.Height);

            float spacingX = viewport.Width / 5f;
            float spacingY = viewport.Height / 2f;
            float paddingX = spacingX / 2f;
            float paddingY = spacingY / 2f;
            List<(int index, string matchCriterium)> spriteIndexes = GenerateShuffledSpriteIndexes();

            CardStorage.Cards = new()
            {
                CreateCard(CalculatePosition(0, 0), spriteIndexes[0]),
                CreateCard(CalculatePosition(1, 0), spriteIndexes[1]),
                CreateCard(CalculatePosition(2, 0), spriteIndexes[2]),
                CreateCard(CalculatePosition(3, 0), spriteIndexes[3]),
                CreateCard(CalculatePosition(4, 0), spriteIndexes[4]),

                CreateCard(CalculatePosition(0, 1), spriteIndexes[5]),
                CreateCard(CalculatePosition(1, 1), spriteIndexes[6]),
                CreateCard(CalculatePosition(2, 1), spriteIndexes[7]),
                CreateCard(CalculatePosition(3, 1), spriteIndexes[8]),
                CreateCard(CalculatePosition(4, 1), spriteIndexes[9])
            };

            Vector3 CalculatePosition(float xIndex, float yIndex)
            {
                return viewport.Unproject(new Vector3(viewport.X + paddingX + (spacingX * xIndex), viewport.Y + paddingY + (spacingY * yIndex), 0f), camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            }
        }

        if (Input.IsKeyPressed(Keys.C))
        {
            ClearAllCards();
        }
    }

    private void ClearAllCards()
    {
        for (int i = CardStorage.Cards.Count - 1; i >= 0; i--)
        {
            Entity.Scene.Entities.Remove(CardStorage.Cards[i]);
            CardStorage.Cards[i] = null;
            CardStorage.Cards.Remove(CardStorage.Cards[i]);
        }
    }

    private List<(int index, string matchCriterium)> GenerateShuffledSpriteIndexes()
    {
        List<(int index, string matchCriterium)> spriteIndexes = new();
        Random rng = new((int)Game.UpdateTime.Elapsed.TotalSeconds);

        for (int i = 1; i <= 5; i++)
        {
            int nextIndex = rng.Next(0, CardBorderSprite.Sprites.Count);
            while (spriteIndexes.Any(x => x.index == nextIndex))
            {
                nextIndex = rng.Next(0, CardBorderSprite.Sprites.Count);
            }

            string matchingCriterium = Guid.NewGuid().ToString();
            spriteIndexes.Add((nextIndex, matchingCriterium));
            spriteIndexes.Add((nextIndex, matchingCriterium));
        }

        ShuffleSprites(spriteIndexes);

        return spriteIndexes;
    }

    private static void ShuffleSprites(List<(int index, string matchCriterium)> spriteIndexes)
    {
        Random rng = new(DateTime.Now.Millisecond);
        int n = spriteIndexes.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (int index, string matchCriterium) value = spriteIndexes[k];
            spriteIndexes[k] = spriteIndexes[n];
            spriteIndexes[n] = value;
        }
    }

    private Entity CreateCard(Vector3 location, (int spriteFrame, string matchCriterium) cardDetails)
    {
        location.Z = 0f;

        Entity cardContainer = new();
        cardContainer.Transform.Scale *= 1f;
        cardContainer.Transform.Position = location;

        RigidbodyComponent rigidBody = cardContainer.GetOrCreate<RigidbodyComponent>();
        rigidBody.RigidBodyType = RigidBodyTypes.Kinematic;

        rigidBody.ColliderShape = new BoxColliderShape(true, new Vector3(1.28f, 1.28f, 1f));
        rigidBody.CollisionGroup = CollisionFilterGroups.DefaultFilter;

        Entity spriteChild = new("SpriteChild");
        spriteChild.Transform.Scale *= 0f;
        spriteChild.Transform.Position *= 0f;
        SpriteComponent cardSpriteComponent = spriteChild.GetOrCreate<SpriteComponent>();
        SpriteFromSheet spriteProvider = new()
        {
            Sheet = CardBorderSprite
        };
        cardSpriteComponent.SpriteProvider = spriteProvider;
        cardContainer.AddChild(spriteChild);

        CardAction cardAction = cardContainer.GetOrCreate<CardAction>();
        cardAction.Card = new()
        {
            BackSpriteStartIndex = 0,
            FrontSpriteStartIndex = cardDetails.spriteFrame,
            IsFrontFacing = false,
            IsPlayable = true,
            IsMatched = false,
            MatchingCriterium = cardDetails.matchCriterium
        };
        cardAction.SpriteSheet = spriteProvider;
        cardAction.Camera = camera;
        cardAction.SpriteChild = spriteChild;

        Entity.Scene.Entities.Add(cardContainer);

        return cardContainer;
    }
}