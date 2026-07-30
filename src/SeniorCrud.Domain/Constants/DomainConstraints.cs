namespace SeniorCrud.Domain.Constants;

public static class DomainConstraints
{
    public const int MinUserNameLength = 3;
    public const int MaxUserNameLength = 120;

    public const int MaxEmailLength = 255;
    public const int MinPasswordHashLength = 20;
    public const int MaxPasswordHashLength = 200;

    public const int CepLength = 8;
    public const int CpfLength = 11;
    public const int StateLength = 2;

    public const int MaxStreetLength = 150;
    public const int MaxAddressNumberLength = 20;
    public const int MaxComplementLength = 120;
    public const int MaxNeighborhoodLength = 120;
    public const int MaxCityLength = 120;
}
