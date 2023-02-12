namespace OdysseyAPI.Tests.Integration;

[CollectionDefinition(Definition)]
public sealed class OdysseyAPICollection : ICollectionFixture<OdysseyAPIFactory>
{
  public const string Definition = nameof(OdysseyAPICollection);
}
