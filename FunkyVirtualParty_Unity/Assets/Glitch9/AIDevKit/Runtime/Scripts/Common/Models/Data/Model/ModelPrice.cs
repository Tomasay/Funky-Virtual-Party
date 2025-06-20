using System;

namespace Glitch9.AIDevKit
{
    [Serializable]
    public class ModelPrice
    {
        public UsageType type;
        public double cost;
        public bool isEstimated;

        public ModelPrice() { }
        public ModelPrice(UsageType type, double cost, bool isEstimated = false)
        {
            this.type = type;
            this.cost = cost;
            this.isEstimated = isEstimated;
        }

        internal static ModelPrice[] Free()
            => new[] { new ModelPrice(UsageType.Free, 0) };
        internal static ModelPrice[] PerCharacter(double cost)
            => new[] { new ModelPrice(UsageType.PerCharacter, cost) };
        internal static ModelPrice[] PerRequest(double cost)
            => new[] { new ModelPrice(UsageType.PerRequest, cost) };
        internal static ModelPrice[] PerMinute(double cost, bool isEstimated = false)
            => new[] { new ModelPrice(UsageType.PerMinute, cost, isEstimated) };
        internal static ModelPrice[] PerInputToken(double cost)
            => new[] { new ModelPrice(UsageType.InputToken, cost) };
        internal static ModelPrice[] PerIO(double input, double output)
            => new[] { new ModelPrice(UsageType.InputToken, input), new ModelPrice(UsageType.OutputToken, output) };
        internal static ModelPrice[] PerICO(double input, double cachedInput, double output)
            => new[] { new ModelPrice(UsageType.InputToken, input), new ModelPrice(UsageType.CachedInputToken, cachedInput), new ModelPrice(UsageType.OutputToken, output) };
        internal static ModelPrice[] PerImage(double cost)
            => new[] { new ModelPrice(UsageType.Image, cost) };
    }

    // internal static class ModelPriceArrayExtensions
    // {
    //     internal static bool IsFree(this ModelPrice[] prices)
    //     {
    //         if (prices == null || prices.Length == 0) return false;

    //         foreach (var price in prices)
    //         {
    //             if (price.type == UsageType.Free) return true;
    //         }

    //         return false;
    //     }
    // }
}