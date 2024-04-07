using Stride.Engine;
using Stride.Engine.Events;
using System.Collections.Generic;

namespace Etsu;

public static class CardStorage
{
    public static List<Entity> Cards;
    public static EventKey CardShownEventKey = new EventKey("CardEvent", "CardShown");
}