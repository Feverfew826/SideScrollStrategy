public class GameRuleManager<GameRule>
{
    public static GameRule defaultGameRule;
    public GameRule overridedGameRule;

    public GameRule Rule => (overridedGameRule == null)? defaultGameRule : overridedGameRule;
}