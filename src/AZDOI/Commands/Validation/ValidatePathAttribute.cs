namespace AZDOI.Commands.Validation;

public class ValidatePathAttribute() : ParameterValidationAttribute(null!)
{
    public override ValidationResult Validate(CommandParameterContext context)
    {
        var fileSystem = context.GetRequiredService<IFileSystem>();

        return context.Value switch
        {
            FilePath filePath when fileSystem.Exist(filePath)
                => ValidationResult.Success(),

            DirectoryPath directoryPath when fileSystem.Exist(directoryPath)
                => ValidationResult.Success(),

            _ => ValidationResult.Error($"Invalid {context.Parameter?.PropertyName} ({context.Value}) specified.")
        };
    }
}

