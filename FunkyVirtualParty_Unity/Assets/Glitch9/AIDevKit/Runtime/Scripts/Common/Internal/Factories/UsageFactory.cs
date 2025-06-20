namespace Glitch9.AIDevKit
{
    internal static class UsageFactory
    {
        internal static Usage Create(string modelId, UsageType type, int count)
        {
            return new Usage()
            {
                modelId = modelId,
                usages = { [type] = count }
            };
        }

        internal static Usage Empty() => new();
        internal static Usage Free() => new() { usages = { [UsageType.Free] = 0 } };
        internal static Usage PerMinute(double cost) => new() { usages = { [UsageType.PerMinute] = (int)cost } };
        internal static Usage PerCharacter(double cost) => new() { usages = { [UsageType.PerCharacter] = (int)cost } };
    }
}