using AZDOI.Commands.Settings;

namespace AZDOI.Commands;

public partial class InventoryAllCommand(
    InventoryCommandServices inventoryCommandServices,
    ILogger<InventoryAllCommand> logger
    ) : InventoryCommand<AZDOIAllSettings>(
        inventoryCommandServices,
        logger
    );