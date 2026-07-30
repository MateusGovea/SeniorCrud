namespace SeniorCrud.Application.Common.Caching;

public static class ApplicationCacheKeys
{
    public const string UsersListVersion = "users:list";

    public static string User(Guid userId) => $"user:{userId:N}";

    public static string Address(Guid addressId) => $"address:{addressId:N}";

    public static string UserAddresses(Guid userId) => $"user:{userId:N}:addresses";

    public static string ViaCep(string cep)
    {
        var normalized = new string((cep ?? string.Empty).Where(char.IsDigit).ToArray());
        return $"viacep:{normalized}";
    }

    public const string AddressesListVersion = "addresses:list";

    public static string UsersList(string version, int pageNumber, int pageSize, string? search)
    {
        var normalizedSearch = (search ?? string.Empty).Trim().ToLowerInvariant();
        return $"users:list:{version}:page:{pageNumber}:size:{pageSize}:search:{normalizedSearch}";
    }

    public static string AddressesList(string version, int pageNumber, int pageSize, string? search)
    {
        var normalizedSearch = (search ?? string.Empty).Trim().ToLowerInvariant();
        return $"addresses:list:{version}:page:{pageNumber}:size:{pageSize}:search:{normalizedSearch}";
    }
}
