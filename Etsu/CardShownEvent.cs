using Stride.Engine;
using Stride.Engine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etsu;

public class CardShownEvent : SyncScript
{
    private EventReceiver cardShownEvent;

    public override void Start()
    {
        cardShownEvent = new EventReceiver(CardStorage.CardShownEventKey);
    }

    public override void Update()
    {
        if (cardShownEvent.TryReceive())
        {
            if (CardStorage.Cards.Count(x => !x.Get<CardAction>().Card.IsMatched && x.Get<CardAction>().Card.IsPlayable && x.Get<CardAction>().Card.IsFrontFacing) == 2)
            {
                string matchingCriterium = CardStorage.Cards.First(x => !x.Get<CardAction>().Card.IsMatched && x.Get<CardAction>().Card.IsPlayable && x.Get<CardAction>().Card.IsFrontFacing).Get<CardAction>().Card.MatchingCriterium;

                List<Entity> matchingCards = CardStorage.Cards.Where(x => !x.Get<CardAction>().Card.IsMatched && x.Get<CardAction>().Card.IsPlayable && x.Get<CardAction>().Card.IsFrontFacing && x.Get<CardAction>().Card.MatchingCriterium.Equals(matchingCriterium))
                                                              .ToList();

                if (matchingCards.Count == 2)
                {
                    foreach (Entity matchingEntity in matchingCards)
                    {
                        Card card = matchingEntity.Get<CardAction>().Card;
                        card.IsMatched = true;
                        card.IsPlayable = false;
                    }
                    // do some bling per match

                    if (CardStorage.Cards.Count == CardStorage.Cards.Count(x => !x.Get<CardAction>().Card.IsMatched && x.Get<CardAction>().Card.IsPlayable && x.Get<CardAction>().Card.IsFrontFacing))
                    {
                        // do some big bling for winning!
                    }
                }
            }
        }
    }
}