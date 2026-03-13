using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Domain.Categories;
using Bintainer.Modules.Catalog.Domain.Components;
using Bintainer.Modules.Catalog.Domain.Footprints;
using Bintainer.Modules.Catalog.Infrastructure.Database;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Bintainer.Modules.Users.Domain.Users;
using Bintainer.Modules.Users.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Api;

internal static class DatabaseSeeder
{
    internal static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseSeeder");

        try
        {
            Guid userId = await SeedDemoUserAsync(scope.ServiceProvider, logger);

            if (userId == Guid.Empty)
            {
                return;
            }

            var categories = await SeedCategoriesAsync(scope.ServiceProvider, logger);
            var footprints = await SeedFootprintsAsync(scope.ServiceProvider, logger);
            var components = await SeedComponentsAsync(scope.ServiceProvider, logger, categories, footprints);
            await SeedInventoryAsync(scope.ServiceProvider, logger, userId, components);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
        }
    }

    private static async Task<Guid> SeedDemoUserAsync(IServiceProvider services, ILogger logger)
    {
        var dbContext = services.GetRequiredService<UsersDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        const string email = "demo@bintainer.com";
        const string password = "Demo123!";

        // Ensure Identity user exists with correct password
        IdentityUser? identityUser = await userManager.FindByEmailAsync(email);

        if (identityUser is null)
        {
            identityUser = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            IdentityResult result = await userManager.CreateAsync(identityUser, password);

            if (!result.Succeeded)
            {
                logger.LogError("Failed to create demo user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return Guid.Empty;
            }
        }
        else
        {
            // Ensure email is confirmed and password is correct
            if (!identityUser.EmailConfirmed)
            {
                identityUser.EmailConfirmed = true;
                await userManager.UpdateAsync(identityUser);
            }

            if (!await userManager.CheckPasswordAsync(identityUser, password))
            {
                await userManager.RemovePasswordAsync(identityUser);
                await userManager.AddPasswordAsync(identityUser, password);
                logger.LogInformation("Reset demo user password");
            }
        }

        // Ensure domain user exists
        User? existingUser = await dbContext.DomainUsers
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser is not null)
        {
            logger.LogInformation("Demo user already exists, skipping seed");
            return existingUser.Id;
        }

        var user = User.Create(email, "Demo", "User", identityUser.Id);
        dbContext.DomainUsers.Add(user);
        ClearDomainEvents(dbContext);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded demo user: {Email}", email);
        return user.Id;
    }

    private static async Task<Dictionary<string, Category>> SeedCategoriesAsync(
        IServiceProvider services, ILogger logger)
    {
        var dbContext = services.GetRequiredService<CatalogDbContext>();

        if (await dbContext.Categories.AnyAsync())
        {
            logger.LogInformation("Categories already exist, skipping seed");
            return await dbContext.Categories.ToDictionaryAsync(c => c.Name);
        }

        string[] categoryNames =
        [
            "Resistors", "Capacitors", "Inductors", "Transistors",
            "Microcontrollers", "LEDs", "Connectors", "ICs"
        ];

        var categories = new Dictionary<string, Category>();

        foreach (string name in categoryNames)
        {
            var category = Category.Create(name);
            categories[name] = category;
            dbContext.Categories.Add(category);
        }

        ClearDomainEvents(dbContext);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} categories", categories.Count);

        return categories;
    }

    private static async Task<Dictionary<string, Footprint>> SeedFootprintsAsync(
        IServiceProvider services, ILogger logger)
    {
        var dbContext = services.GetRequiredService<CatalogDbContext>();

        if (await dbContext.Footprints.AnyAsync())
        {
            logger.LogInformation("Footprints already exist, skipping seed");
            return await dbContext.Footprints.ToDictionaryAsync(f => f.Name);
        }

        string[] footprintNames =
        [
            "0402", "0603", "0805", "1206", "SOT-23", "SOIC-8",
            "LQFP-48", "QFN-56", "TO-92", "TO-220", "3mm", "5mm"
        ];

        var footprints = new Dictionary<string, Footprint>();

        foreach (string name in footprintNames)
        {
            var footprint = Footprint.Create(name);
            footprints[name] = footprint;
            dbContext.Footprints.Add(footprint);
        }

        ClearDomainEvents(dbContext);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} footprints", footprints.Count);

        return footprints;
    }

    private static async Task<List<Component>> SeedComponentsAsync(
        IServiceProvider services,
        ILogger logger,
        Dictionary<string, Category> categories,
        Dictionary<string, Footprint> footprints)
    {
        var dbContext = services.GetRequiredService<CatalogDbContext>();

        if (await dbContext.Components.AnyAsync())
        {
            logger.LogInformation("Components already exist, skipping seed");
            return await dbContext.Components.ToListAsync();
        }

        var components = new List<Component>
        {
            // Resistors
            Component.Create(
                partNumber: "R-10K-0805",
                manufacturerPartNumber: "RC0805FR-0710KL",
                description: "10K Ohm 1% 1/8W SMD Resistor",
                detailedDescription: "Thick film chip resistor, 10K Ohm, 1% tolerance, 0805 package",
                imageUrl: null, url: null, provider: "Yageo", providerPartNumber: "RC0805FR-0710KL",
                categoryId: categories["Resistors"].Id,
                footprintId: footprints["0805"].Id,
                attributes: new Dictionary<string, string> { ["Resistance"] = "10kΩ", ["Tolerance"] = "1%", ["Power"] = "0.125W" },
                tags: "resistor,smd,0805",
                unitPrice: 0.01m, manufacturer: "Yageo", lowStockThreshold: 50),

            Component.Create(
                partNumber: "R-1K-0603",
                manufacturerPartNumber: "RC0603FR-071KL",
                description: "1K Ohm 1% 1/10W SMD Resistor",
                detailedDescription: "Thick film chip resistor, 1K Ohm, 1% tolerance, 0603 package",
                imageUrl: null, url: null, provider: "Yageo", providerPartNumber: "RC0603FR-071KL",
                categoryId: categories["Resistors"].Id,
                footprintId: footprints["0603"].Id,
                attributes: new Dictionary<string, string> { ["Resistance"] = "1kΩ", ["Tolerance"] = "1%", ["Power"] = "0.1W" },
                tags: "resistor,smd,0603",
                unitPrice: 0.01m, manufacturer: "Yageo", lowStockThreshold: 50),

            Component.Create(
                partNumber: "R-100R-0402",
                manufacturerPartNumber: "RC0402FR-07100RL",
                description: "100 Ohm 1% 1/16W SMD Resistor",
                detailedDescription: "Thick film chip resistor, 100 Ohm, 1% tolerance, 0402 package",
                imageUrl: null, url: null, provider: "Yageo", providerPartNumber: "RC0402FR-07100RL",
                categoryId: categories["Resistors"].Id,
                footprintId: footprints["0402"].Id,
                attributes: new Dictionary<string, string> { ["Resistance"] = "100Ω", ["Tolerance"] = "1%", ["Power"] = "0.0625W" },
                tags: "resistor,smd,0402",
                unitPrice: 0.01m, manufacturer: "Yageo", lowStockThreshold: 100),

            Component.Create(
                partNumber: "R-4K7-1206",
                manufacturerPartNumber: "RC1206FR-074K7L",
                description: "4.7K Ohm 1% 1/4W SMD Resistor",
                detailedDescription: "Thick film chip resistor, 4.7K Ohm, 1% tolerance, 1206 package",
                imageUrl: null, url: null, provider: "Yageo", providerPartNumber: "RC1206FR-074K7L",
                categoryId: categories["Resistors"].Id,
                footprintId: footprints["1206"].Id,
                attributes: new Dictionary<string, string> { ["Resistance"] = "4.7kΩ", ["Tolerance"] = "1%", ["Power"] = "0.25W" },
                tags: "resistor,smd,1206",
                unitPrice: 0.02m, manufacturer: "Yageo", lowStockThreshold: 30),

            // Capacitors
            Component.Create(
                partNumber: "C-100NF-0805",
                manufacturerPartNumber: "CL21B104KBCNNNC",
                description: "100nF 50V X7R MLCC Capacitor",
                detailedDescription: "Multilayer ceramic capacitor, 100nF, 50V, X7R dielectric, 0805 package",
                imageUrl: null, url: null, provider: "Samsung Electro-Mechanics", providerPartNumber: "CL21B104KBCNNNC",
                categoryId: categories["Capacitors"].Id,
                footprintId: footprints["0805"].Id,
                attributes: new Dictionary<string, string> { ["Capacitance"] = "100nF", ["Voltage"] = "50V", ["Dielectric"] = "X7R" },
                tags: "capacitor,mlcc,smd,0805,decoupling",
                unitPrice: 0.02m, manufacturer: "Samsung", lowStockThreshold: 100),

            Component.Create(
                partNumber: "C-10UF-0805",
                manufacturerPartNumber: "CL21A106KAYNNNE",
                description: "10uF 25V X5R MLCC Capacitor",
                detailedDescription: "Multilayer ceramic capacitor, 10uF, 25V, X5R dielectric, 0805 package",
                imageUrl: null, url: null, provider: "Samsung Electro-Mechanics", providerPartNumber: "CL21A106KAYNNNE",
                categoryId: categories["Capacitors"].Id,
                footprintId: footprints["0805"].Id,
                attributes: new Dictionary<string, string> { ["Capacitance"] = "10µF", ["Voltage"] = "25V", ["Dielectric"] = "X5R" },
                tags: "capacitor,mlcc,smd,0805,bulk",
                unitPrice: 0.05m, manufacturer: "Samsung", lowStockThreshold: 50),

            Component.Create(
                partNumber: "C-1UF-0603",
                manufacturerPartNumber: "CL10A105KB8NNNC",
                description: "1uF 50V X5R MLCC Capacitor",
                detailedDescription: "Multilayer ceramic capacitor, 1uF, 50V, X5R dielectric, 0603 package",
                imageUrl: null, url: null, provider: "Samsung Electro-Mechanics", providerPartNumber: "CL10A105KB8NNNC",
                categoryId: categories["Capacitors"].Id,
                footprintId: footprints["0603"].Id,
                attributes: new Dictionary<string, string> { ["Capacitance"] = "1µF", ["Voltage"] = "50V", ["Dielectric"] = "X5R" },
                tags: "capacitor,mlcc,smd,0603",
                unitPrice: 0.03m, manufacturer: "Samsung", lowStockThreshold: 50),

            // Transistors
            Component.Create(
                partNumber: "Q-2N3904",
                manufacturerPartNumber: "2N3904",
                description: "NPN General Purpose Transistor",
                detailedDescription: "NPN bipolar transistor, 40V, 200mA, TO-92 package",
                imageUrl: null, url: null, provider: "ON Semiconductor", providerPartNumber: "2N3904",
                categoryId: categories["Transistors"].Id,
                footprintId: footprints["TO-92"].Id,
                attributes: new Dictionary<string, string> { ["Type"] = "NPN", ["Vceo"] = "40V", ["Ic"] = "200mA", ["hFE"] = "100-300" },
                tags: "transistor,npn,through-hole",
                unitPrice: 0.05m, manufacturer: "ON Semiconductor", lowStockThreshold: 20),

            Component.Create(
                partNumber: "Q-IRLZ44N",
                manufacturerPartNumber: "IRLZ44NPBF",
                description: "N-Channel Logic Level MOSFET 55V 47A",
                detailedDescription: "N-Channel HEXFET Power MOSFET, 55V, 47A, TO-220 package, logic level gate",
                imageUrl: null, url: null, provider: "Infineon", providerPartNumber: "IRLZ44NPBF",
                categoryId: categories["Transistors"].Id,
                footprintId: footprints["TO-220"].Id,
                attributes: new Dictionary<string, string> { ["Type"] = "N-MOSFET", ["Vds"] = "55V", ["Id"] = "47A", ["Rds(on)"] = "22mΩ" },
                tags: "mosfet,n-channel,power,through-hole",
                unitPrice: 1.20m, manufacturer: "Infineon", lowStockThreshold: 10),

            // Microcontrollers
            Component.Create(
                partNumber: "MCU-STM32F103",
                manufacturerPartNumber: "STM32F103C8T6",
                description: "ARM Cortex-M3 MCU 72MHz 64KB Flash",
                detailedDescription: "STM32F103C8T6 - 32-bit ARM Cortex-M3, 72MHz, 64KB Flash, 20KB SRAM, LQFP-48",
                imageUrl: null, url: null, provider: "STMicroelectronics", providerPartNumber: "STM32F103C8T6",
                categoryId: categories["Microcontrollers"].Id,
                footprintId: footprints["LQFP-48"].Id,
                attributes: new Dictionary<string, string> { ["Core"] = "Cortex-M3", ["Speed"] = "72MHz", ["Flash"] = "64KB", ["RAM"] = "20KB" },
                tags: "mcu,stm32,arm,cortex-m3",
                unitPrice: 2.50m, manufacturer: "STMicroelectronics", lowStockThreshold: 5),

            Component.Create(
                partNumber: "MCU-ESP32-S3",
                manufacturerPartNumber: "ESP32-S3-WROOM-1-N16R8",
                description: "ESP32-S3 WiFi+BLE5 MCU Module 16MB Flash 8MB PSRAM",
                detailedDescription: "ESP32-S3 SoC module with WiFi and BLE 5, dual-core 240MHz, 16MB Flash, 8MB PSRAM, QFN-56",
                imageUrl: null, url: null, provider: "Espressif", providerPartNumber: "ESP32-S3-WROOM-1-N16R8",
                categoryId: categories["Microcontrollers"].Id,
                footprintId: footprints["QFN-56"].Id,
                attributes: new Dictionary<string, string> { ["Core"] = "Xtensa LX7 Dual", ["Speed"] = "240MHz", ["Flash"] = "16MB", ["RAM"] = "8MB PSRAM", ["WiFi"] = "802.11 b/g/n", ["BLE"] = "5.0" },
                tags: "mcu,esp32,wifi,bluetooth,iot",
                unitPrice: 3.80m, manufacturer: "Espressif", lowStockThreshold: 5),

            // LEDs
            Component.Create(
                partNumber: "LED-RED-3MM",
                manufacturerPartNumber: "HLMP-1301",
                description: "Red LED 3mm 20mA 2V",
                detailedDescription: "Standard red LED, 3mm, 20mA forward current, 2V forward voltage, 625nm wavelength",
                imageUrl: null, url: null, provider: "Broadcom", providerPartNumber: "HLMP-1301",
                categoryId: categories["LEDs"].Id,
                footprintId: footprints["3mm"].Id,
                attributes: new Dictionary<string, string> { ["Color"] = "Red", ["Vf"] = "2V", ["If"] = "20mA", ["Wavelength"] = "625nm" },
                tags: "led,red,through-hole,indicator",
                unitPrice: 0.08m, manufacturer: "Broadcom", lowStockThreshold: 20),

            Component.Create(
                partNumber: "LED-GREEN-5MM",
                manufacturerPartNumber: "HLMP-1790",
                description: "Green LED 5mm 20mA 2.2V",
                detailedDescription: "Standard green LED, 5mm, 20mA forward current, 2.2V forward voltage, 565nm wavelength",
                imageUrl: null, url: null, provider: "Broadcom", providerPartNumber: "HLMP-1790",
                categoryId: categories["LEDs"].Id,
                footprintId: footprints["5mm"].Id,
                attributes: new Dictionary<string, string> { ["Color"] = "Green", ["Vf"] = "2.2V", ["If"] = "20mA", ["Wavelength"] = "565nm" },
                tags: "led,green,through-hole,indicator",
                unitPrice: 0.10m, manufacturer: "Broadcom", lowStockThreshold: 20),

            // ICs
            Component.Create(
                partNumber: "IC-NE555",
                manufacturerPartNumber: "NE555DR",
                description: "555 Timer IC SOIC-8",
                detailedDescription: "Classic 555 timer/oscillator IC in SOIC-8 package, 4.5V-16V supply",
                imageUrl: null, url: null, provider: "Texas Instruments", providerPartNumber: "NE555DR",
                categoryId: categories["ICs"].Id,
                footprintId: footprints["SOIC-8"].Id,
                attributes: new Dictionary<string, string> { ["Type"] = "Timer", ["Vcc"] = "4.5V-16V", ["Frequency"] = "500kHz" },
                tags: "ic,timer,555,smd",
                unitPrice: 0.30m, manufacturer: "Texas Instruments", lowStockThreshold: 10),

            Component.Create(
                partNumber: "IC-LM7805",
                manufacturerPartNumber: "LM7805CT",
                description: "5V 1.5A Linear Voltage Regulator TO-220",
                detailedDescription: "Positive voltage regulator, 5V output, 1.5A max, TO-220 package",
                imageUrl: null, url: null, provider: "Texas Instruments", providerPartNumber: "LM7805CT",
                categoryId: categories["ICs"].Id,
                footprintId: footprints["TO-220"].Id,
                attributes: new Dictionary<string, string> { ["Type"] = "Voltage Regulator", ["Vout"] = "5V", ["Iout"] = "1.5A" },
                tags: "ic,regulator,linear,through-hole",
                unitPrice: 0.40m, manufacturer: "Texas Instruments", lowStockThreshold: 10),

            // Connectors
            Component.Create(
                partNumber: "CONN-USB-C",
                manufacturerPartNumber: "USB4105-GF-A",
                description: "USB Type-C Receptacle SMD",
                detailedDescription: "USB Type-C 2.0 receptacle, surface mount, mid-mount, 16-pin",
                imageUrl: null, url: null, provider: "GCT", providerPartNumber: "USB4105-GF-A",
                categoryId: categories["Connectors"].Id,
                footprintId: null,
                attributes: new Dictionary<string, string> { ["Type"] = "USB-C", ["Mounting"] = "SMD", ["Pins"] = "16" },
                tags: "connector,usb,usb-c,smd",
                unitPrice: 0.80m, manufacturer: "GCT", lowStockThreshold: 10),

            // Inductors
            Component.Create(
                partNumber: "L-10UH-0805",
                manufacturerPartNumber: "LQM21FN100N00D",
                description: "10uH Ferrite Chip Inductor 0805",
                detailedDescription: "Multilayer ferrite chip inductor, 10uH, 100mA, 0805 package",
                imageUrl: null, url: null, provider: "Murata", providerPartNumber: "LQM21FN100N00D",
                categoryId: categories["Inductors"].Id,
                footprintId: footprints["0805"].Id,
                attributes: new Dictionary<string, string> { ["Inductance"] = "10µH", ["Current"] = "100mA", ["DCR"] = "2.5Ω" },
                tags: "inductor,ferrite,smd,0805",
                unitPrice: 0.15m, manufacturer: "Murata", lowStockThreshold: 20),
        };

        dbContext.Components.AddRange(components);
        ClearDomainEvents(dbContext);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} components", components.Count);
        return components;
    }

    private static async Task SeedInventoryAsync(
        IServiceProvider services,
        ILogger logger,
        Guid userId,
        List<Component> components)
    {
        var dbContext = services.GetRequiredService<InventoryDbContext>();

        if (await dbContext.Inventories.AnyAsync())
        {
            logger.LogInformation("Inventory already exists, skipping seed");
            return;
        }

        // Create inventory
        var inventory = Bintainer.Modules.Inventory.Domain.Inventories.Inventory.Create("My Lab", userId);
        dbContext.Inventories.Add(inventory);

        // Create storage units (StorageUnit.Create auto-generates bins and compartments)
        var resistorStorage = Bintainer.Modules.Inventory.Domain.StorageUnits.StorageUnit.Create(
            "Resistors", columns: 10, rows: 8, compartmentCount: 4, inventoryId: inventory.Id);
        dbContext.StorageUnits.Add(resistorStorage);

        var capacitorStorage = Bintainer.Modules.Inventory.Domain.StorageUnits.StorageUnit.Create(
            "Capacitors", columns: 8, rows: 6, compartmentCount: 4, inventoryId: inventory.Id);
        dbContext.StorageUnits.Add(capacitorStorage);

        var mcuStorage = Bintainer.Modules.Inventory.Domain.StorageUnits.StorageUnit.Create(
            "Microcontrollers", columns: 5, rows: 4, compartmentCount: 2, inventoryId: inventory.Id);
        dbContext.StorageUnits.Add(mcuStorage);

        ClearDomainEvents(dbContext);
        await dbContext.SaveChangesAsync();

        // Assign components to compartments
        // We need to reload compartments since SaveChanges might have cleared tracked entity state
        var resistorBins = await dbContext.Bins
            .Include(b => b.Compartments)
            .Where(b => b.StorageUnitId == resistorStorage.Id)
            .OrderBy(b => b.Column).ThenBy(b => b.Row)
            .ToListAsync();

        var capacitorBins = await dbContext.Bins
            .Include(b => b.Compartments)
            .Where(b => b.StorageUnitId == capacitorStorage.Id)
            .OrderBy(b => b.Column).ThenBy(b => b.Row)
            .ToListAsync();

        var mcuBins = await dbContext.Bins
            .Include(b => b.Compartments)
            .Where(b => b.StorageUnitId == mcuStorage.Id)
            .OrderBy(b => b.Column).ThenBy(b => b.Row)
            .ToListAsync();

        // Build a lookup by part number prefix for easy assignment
        var componentsByPart = components.ToDictionary(c => c.PartNumber);

        // Assign resistors to the first few bins of the resistor storage
        AssignComponent(resistorBins, 0, 0, componentsByPart["R-10K-0805"], 200);
        AssignComponent(resistorBins, 0, 1, componentsByPart["R-1K-0603"], 150);
        AssignComponent(resistorBins, 1, 0, componentsByPart["R-100R-0402"], 500);
        AssignComponent(resistorBins, 1, 1, componentsByPart["R-4K7-1206"], 75);

        // Assign capacitors
        AssignComponent(capacitorBins, 0, 0, componentsByPart["C-100NF-0805"], 300);
        AssignComponent(capacitorBins, 0, 1, componentsByPart["C-10UF-0805"], 100);
        AssignComponent(capacitorBins, 1, 0, componentsByPart["C-1UF-0603"], 200);

        // Assign MCUs and other ICs
        AssignComponent(mcuBins, 0, 0, componentsByPart["MCU-STM32F103"], 12);
        AssignComponent(mcuBins, 0, 1, componentsByPart["MCU-ESP32-S3"], 8);
        AssignComponent(mcuBins, 1, 0, componentsByPart["IC-NE555"], 25);
        AssignComponent(mcuBins, 1, 1, componentsByPart["IC-LM7805"], 15);

        ClearDomainEvents(dbContext);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Seeded inventory with storage units and component assignments");
    }

    private static void AssignComponent(
        List<Bintainer.Modules.Inventory.Domain.Bins.Bin> bins,
        int col, int row,
        Component component, int quantity)
    {
        var bin = bins.FirstOrDefault(b => b.Column == col && b.Row == row);
        if (bin is null) return;

        var compartment = bin.Compartments.FirstOrDefault();
        compartment?.AssignComponent(component.Id, quantity);
    }

    /// <summary>
    /// Clears all domain events from tracked entities to prevent the
    /// PublishDomainEventsInterceptor from firing during seed operations
    /// (it requires scoped services that aren't available at startup).
    /// </summary>
    private static void ClearDomainEvents(DbContext context)
    {
        var entities = context.ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }
    }
}
