using AZDOI.Commands.Settings;

namespace AZDOI.Commands;

public partial class InventoryPipelinesCommand(
    InventoryCommandServices inventoryCommandServices,
    ILogger<InventoryPipelinesCommand> logger
    )
    : InventoryCommand<AZDOIPipelinesSettings>(
            inventoryCommandServices,
            logger
    );
