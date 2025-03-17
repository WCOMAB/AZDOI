using System.Text.RegularExpressions;

public partial class ValidateOrgAttribute() : ParameterValidationAttribute(errorMessage: null!)
{
    private static readonly Regex OrganizationRegex = MyRegex();

    public override ValidationResult Validate(CommandParameterContext context)
        => (
            context.Value is string orgName
                ? (IsString: true, IsValidFormat: OrganizationRegex.IsMatch(orgName), Value: orgName) 
                : (IsString: false, IsValidFormat: false, Value: string.Empty)
                ) switch
        {
            { IsString: false } =>
                ValidationResult.Error($"Invalid {context.Parameter?.PropertyName}: Value must be a string."),

            { IsValidFormat: false } invalidValue =>
                ValidationResult.Error($"VS850015: The specified name '{invalidValue.Value}' is not allowed. " +
                    "It must be between 3 and 50 characters long, start and end with a letter or number, and may only contain letters, numbers, and hyphens (-)."),

            _ => ValidationResult.Success()
        };
    
    [GeneratedRegex(@"^(?=.{3,50}$)[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}