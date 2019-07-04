namespace OAuth2SecurityService.Manager.DbContexts.SeedData
{
    public enum SeedingType
    {
        NotSet = 0,
        IntegrationTest,
        Development,
        Staging,
        Production
    }
}