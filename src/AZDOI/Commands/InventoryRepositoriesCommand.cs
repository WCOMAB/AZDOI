using AZDOI.Commands.Settings;

namespace AZDOI.Commands;

public partial class InventoryRepositoriesCommand(
    InventoryCommandServices inventoryCommandServices,
    ILogger<InventoryRepositoriesCommand> logger
    )
    : InventoryCommand<AZDOIRepositorySettings>(
            inventoryCommandServices,
            logger
    );
