namespace Glitch9.AIDevKit.Editor
{
    public interface ICatalogueEntry : IData
    {
        Api Api { get; }
        bool IsDeprecated { get; set; }
    }
}