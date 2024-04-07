namespace Etsu;

public class Card
{
    public int FrontSpriteStartIndex { get; set; }
    public int BackSpriteStartIndex { get; set; }
    public bool IsFrontFacing { get; set; }
    public bool IsPlayable { get; set; }
    public bool IsMatched { get; set; }
    public string MatchingCriterium { get; set; }
}