CREATE DATABASE PCBuilderDB;
GO
USE PCBuilderDB;
GO

/* =========================================================
   1. USERS, CATEGORIES, BRANDS
   ========================================================= */

CREATE TABLE dbo.Users (
    UserID          INT IDENTITY(1,1) PRIMARY KEY,
    Username        NVARCHAR(50) NOT NULL,
    Email           NVARCHAR(255) NOT NULL,
    PasswordHash    NVARCHAR(255) NOT NULL,
    FullName        NVARCHAR(150) NULL,
    UserRole        VARCHAR(10) NOT NULL
        CONSTRAINT DF_Users_UserRole DEFAULT ('User'),
    IsActive        BIT NOT NULL
        CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedAt       DATETIME2(0) NOT NULL
        CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(0) NULL,
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_UserRole CHECK (UserRole IN ('Admin', 'User'))
);
GO

CREATE TABLE dbo.Categories (
    CategoryID      INT IDENTITY(1,1) PRIMARY KEY,
    CategoryCode    VARCHAR(30) NOT NULL,
    CategoryName    NVARCHAR(100) NOT NULL,
    Description     NVARCHAR(500) NULL,
    IsActive        BIT NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT (1),
    CONSTRAINT UQ_Categories_Code UNIQUE (CategoryCode),
    CONSTRAINT UQ_Categories_Name UNIQUE (CategoryName)
);
GO

CREATE TABLE dbo.Brands (
    BrandID         INT IDENTITY(1,1) PRIMARY KEY,
    BrandName       NVARCHAR(100) NOT NULL,
    Website         NVARCHAR(300) NULL,
    IsActive        BIT NOT NULL CONSTRAINT DF_Brands_IsActive DEFAULT (1),
    CONSTRAINT UQ_Brands_Name UNIQUE (BrandName)
);
GO

/* =========================================================
   2. COMPONENTS: shared fields for all hardware
   ========================================================= */

CREATE TABLE dbo.Components (
    ComponentID     INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID      INT NOT NULL,
    BrandID         INT NOT NULL,
    ModelName       NVARCHAR(200) NOT NULL,
    SKU             NVARCHAR(100) NULL,
    Description     NVARCHAR(MAX) NULL,
    Price           DECIMAL(18,2) NOT NULL,
    StockQuantity   INT NOT NULL CONSTRAINT DF_Components_Stock DEFAULT (0),
    ImageURL        NVARCHAR(1000) NULL,
    IsActive        BIT NOT NULL CONSTRAINT DF_Components_IsActive DEFAULT (1),
    CreatedAt       DATETIME2(0) NOT NULL
        CONSTRAINT DF_Components_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(0) NULL,
    CONSTRAINT FK_Components_Categories FOREIGN KEY (CategoryID)
        REFERENCES dbo.Categories(CategoryID),
    CONSTRAINT FK_Components_Brands FOREIGN KEY (BrandID)
        REFERENCES dbo.Brands(BrandID),
    CONSTRAINT CK_Components_Price CHECK (Price >= 0),
    CONSTRAINT CK_Components_Stock CHECK (StockQuantity >= 0),
    CONSTRAINT UQ_Components_Brand_Model UNIQUE (BrandID, ModelName)
);
GO

CREATE INDEX IX_Components_Category_Active
    ON dbo.Components(CategoryID, IsActive);
CREATE INDEX IX_Components_Price
    ON dbo.Components(Price);
CREATE INDEX IX_Components_ModelName
    ON dbo.Components(ModelName);
GO

/* =========================================================
   3. TYPE-SPECIFIC TECHNICAL SPECS
   Each component should have at most one matching specs row.
   ========================================================= */

CREATE TABLE dbo.CPU_Specs (
    ComponentID       INT PRIMARY KEY,
    Socket            NVARCHAR(50) NOT NULL,
    CoreCount         TINYINT NULL,
    ThreadCount       SMALLINT NULL,
    BaseClockGHz      DECIMAL(5,2) NULL,
    BoostClockGHz     DECIMAL(5,2) NULL,
    TDP_W              SMALLINT NULL,
    IntegratedGPU     BIT NOT NULL CONSTRAINT DF_CPU_IGPU DEFAULT (0),
    MemoryType        VARCHAR(10) NULL,
    CONSTRAINT FK_CPU_Specs_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID) ON DELETE CASCADE,
    CONSTRAINT CK_CPU_Cores CHECK (CoreCount IS NULL OR CoreCount > 0),
    CONSTRAINT CK_CPU_Threads CHECK (ThreadCount IS NULL OR ThreadCount > 0),
    CONSTRAINT CK_CPU_TDP CHECK (TDP_W IS NULL OR TDP_W > 0),
    CONSTRAINT CK_CPU_MemoryType CHECK (MemoryType IS NULL OR MemoryType IN ('DDR4','DDR5'))
);
GO

CREATE TABLE dbo.Motherboard_Specs (
    ComponentID          INT PRIMARY KEY,
    Socket               NVARCHAR(50) NOT NULL,
    Chipset              NVARCHAR(50) NULL,
    FormFactor           VARCHAR(20) NOT NULL,
    MemoryType           VARCHAR(10) NOT NULL,
    MemorySlots          TINYINT NOT NULL,
    MaxMemoryGB          SMALLINT NOT NULL,
    MaxMemorySpeedMHz    INT NULL,
    M2Slots              TINYINT NOT NULL CONSTRAINT DF_MB_M2Slots DEFAULT (0),
    SATAConnections      TINYINT NOT NULL CONSTRAINT DF_MB_SATA DEFAULT (0),
    CONSTRAINT FK_MB_Specs_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID) ON DELETE CASCADE,
    CONSTRAINT CK_MB_FormFactor CHECK (FormFactor IN ('ATX','Micro-ATX','Mini-ITX','E-ATX','Other')),
    CONSTRAINT CK_MB_MemoryType CHECK (MemoryType IN ('DDR4','DDR5')),
    CONSTRAINT CK_MB_MemorySlots CHECK (MemorySlots > 0),
    CONSTRAINT CK_MB_MaxMemory CHECK (MaxMemoryGB > 0)
);
GO

CREATE TABLE dbo.RAM_Specs (
    ComponentID       INT PRIMARY KEY,
    MemoryType        VARCHAR(10) NOT NULL,
    CapacityGB        SMALLINT NOT NULL,
    SpeedMHz          INT NOT NULL,
    ModuleCount       TINYINT NOT NULL CONSTRAINT DF_RAM_ModuleCount DEFAULT (1),
    CASLatency        SMALLINT NULL,
    IsECC             BIT NOT NULL CONSTRAINT DF_RAM_ECC DEFAULT (0),
    FormFactor        VARCHAR(10) NOT NULL
        CONSTRAINT DF_RAM_FormFactor DEFAULT ('DIMM'),
    CONSTRAINT FK_RAM_Specs_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID) ON DELETE CASCADE,
    CONSTRAINT CK_RAM_Type CHECK (MemoryType IN ('DDR4','DDR5')),
    CONSTRAINT CK_RAM_Capacity CHECK (CapacityGB > 0),
    CONSTRAINT CK_RAM_Speed CHECK (SpeedMHz > 0),
    CONSTRAINT CK_RAM_Modules CHECK (ModuleCount > 0),
    CONSTRAINT CK_RAM_FormFactor CHECK (FormFactor IN ('DIMM','SO-DIMM'))
);
GO

CREATE TABLE dbo.GPU_Specs (
    ComponentID       INT PRIMARY KEY,
    Chipset           NVARCHAR(100) NULL,
    VRAM_GB           SMALLINT NOT NULL,
    LengthMM          DECIMAL(10,2) NOT NULL,
    SlotWidth         DECIMAL(4,1) NULL,
    RecommendedPSU_W  SMALLINT NULL,
    PowerDraw_W       SMALLINT NULL,
    PowerConnector    NVARCHAR(100) NULL,
    CONSTRAINT FK_GPU_Specs_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID) ON DELETE CASCADE,
    CONSTRAINT CK_GPU_VRAM CHECK (VRAM_GB > 0),
    CONSTRAINT CK_GPU_Length CHECK (LengthMM > 0),
    CONSTRAINT CK_GPU_Power CHECK (PowerDraw_W IS NULL OR PowerDraw_W > 0)
);
GO

CREATE TABLE dbo.PSU_Specs (
    ComponentID       INT PRIMARY KEY,
    Wattage_W         SMALLINT NOT NULL,
    EfficiencyRating  VARCHAR(20) NULL,
    FormFactor        VARCHAR(30) NULL,
    ModularType       VARCHAR(20) NULL,
    CONSTRAINT FK_PSU_Specs_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID) ON DELETE CASCADE,
    CONSTRAINT CK_PSU_Wattage CHECK (Wattage_W > 0),
    CONSTRAINT CK_PSU_Modular CHECK (ModularType IS NULL OR ModularType IN ('Non-Modular','Semi-Modular','Fully-Modular'))
);
GO

CREATE TABLE dbo.Storage_Specs (
    ComponentID       INT PRIMARY KEY,
    StorageType       VARCHAR(20) NOT NULL,
    CapacityGB        INT NOT NULL,
    Interface         VARCHAR(30) NOT NULL,
    FormFactor        VARCHAR(20) NULL,
    ReadSpeedMBps     INT NULL,
    WriteSpeedMBps    INT NULL,
    CONSTRAINT FK_Storage_Specs_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID) ON DELETE CASCADE,
    CONSTRAINT CK_Storage_Type CHECK (StorageType IN ('HDD','SSD','NVMe SSD')),
    CONSTRAINT CK_Storage_Capacity CHECK (CapacityGB > 0)
);
GO

CREATE TABLE dbo.Case_Specs (
    ComponentID          INT PRIMARY KEY,
    SupportedFormFactors NVARCHAR(200) NOT NULL,
    MaxGPULengthMM       DECIMAL(10,2) NOT NULL,
    MaxCoolerHeightMM    DECIMAL(10,2) NULL,
    MaxPSULengthMM       DECIMAL(10,2) NULL,
    RadiatorSupport      NVARCHAR(200) NULL,
    CONSTRAINT FK_Case_Specs_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID) ON DELETE CASCADE,
    CONSTRAINT CK_Case_GPULength CHECK (MaxGPULengthMM > 0),
    CONSTRAINT CK_Case_CoolerHeight CHECK (MaxCoolerHeightMM IS NULL OR MaxCoolerHeightMM > 0)
);
GO

CREATE TABLE dbo.Cooling_Specs (
    ComponentID          INT PRIMARY KEY,
    CoolingType          VARCHAR(20) NOT NULL,
    SupportedSockets     NVARCHAR(500) NOT NULL,
    HeightMM             DECIMAL(10,2) NULL,
    RadiatorSizeMM       SMALLINT NULL,
    TDP_Rating_W         SMALLINT NULL,
    CONSTRAINT FK_Cooling_Specs_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID) ON DELETE CASCADE,
    CONSTRAINT CK_Cooling_Type CHECK (CoolingType IN ('Air','Liquid','Other'))
);
GO

CREATE TABLE dbo.OtherComponent_Specs (
    ComponentID       INT PRIMARY KEY,
    SpecificationText NVARCHAR(MAX) NULL,
    CONSTRAINT FK_OtherSpecs_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID) ON DELETE CASCADE
);
GO

/* =========================================================
   4. COMPATIBILITY RULES
   RuleValue fields support configurable rules; application
   code interprets RuleType, Operator and values.
   ========================================================= */

CREATE TABLE dbo.CompatibilityRules (
    RuleID              INT IDENTITY(1,1) PRIMARY KEY,
    RuleName            NVARCHAR(150) NOT NULL,
    SourceCategoryID    INT NOT NULL,
    TargetCategoryID    INT NOT NULL,
    RuleType            VARCHAR(30) NOT NULL,
    Operator            VARCHAR(20) NOT NULL,
    SourceSpecKey       VARCHAR(100) NULL,
    TargetSpecKey       VARCHAR(100) NULL,
    ExpectedValue       NVARCHAR(200) NULL,
    Severity            VARCHAR(10) NOT NULL
        CONSTRAINT DF_CompatibilityRules_Severity DEFAULT ('Error'),
    MessageTemplate     NVARCHAR(500) NOT NULL,
    IsActive            BIT NOT NULL
        CONSTRAINT DF_CompatibilityRules_Active DEFAULT (1),
    CONSTRAINT FK_Rules_SourceCategory FOREIGN KEY (SourceCategoryID)
        REFERENCES dbo.Categories(CategoryID),
    CONSTRAINT FK_Rules_TargetCategory FOREIGN KEY (TargetCategoryID)
        REFERENCES dbo.Categories(CategoryID),
    CONSTRAINT CK_Rules_Type CHECK (RuleType IN
        ('ExactMatch','NumericLimit','Boolean','Custom')),
    CONSTRAINT CK_Rules_Operator CHECK (Operator IN
        ('=','<>','>','>=','<','<=','IN','CUSTOM')),
    CONSTRAINT CK_Rules_Severity CHECK (Severity IN ('Info','Warning','Error'))
);
GO

CREATE INDEX IX_CompatibilityRules_Categories
    ON dbo.CompatibilityRules(SourceCategoryID, TargetCategoryID, IsActive);
GO

/* =========================================================
   5. USER BUILDS
   ========================================================= */

CREATE TABLE dbo.PCBuilds (
    BuildID           INT IDENTITY(1,1) PRIMARY KEY,
    UserID            INT NOT NULL,
    BuildName         NVARCHAR(150) NOT NULL,
    Notes             NVARCHAR(1000) NULL,
    Budget             DECIMAL(18,2) NULL,
    Status             VARCHAR(20) NOT NULL
        CONSTRAINT DF_PCBuilds_Status DEFAULT ('Draft'),
    CreatedAt          DATETIME2(0) NOT NULL
        CONSTRAINT DF_PCBuilds_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt          DATETIME2(0) NULL,
    CONSTRAINT FK_PCBuilds_Users FOREIGN KEY (UserID)
        REFERENCES dbo.Users(UserID),
    CONSTRAINT CK_PCBuilds_Budget CHECK (Budget IS NULL OR Budget >= 0),
    CONSTRAINT CK_PCBuilds_Status CHECK (Status IN ('Draft','Saved','Archived'))
);
GO

CREATE TABLE dbo.PCBuildItems (
    BuildItemID       INT IDENTITY(1,1) PRIMARY KEY,
    BuildID           INT NOT NULL,
    ComponentID       INT NOT NULL,
    Quantity          TINYINT NOT NULL
        CONSTRAINT DF_PCBuildItems_Quantity DEFAULT (1),
    UnitPriceSnapshot DECIMAL(18,2) NOT NULL,
    AddedAt            DATETIME2(0) NOT NULL
        CONSTRAINT DF_PCBuildItems_AddedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_BuildItems_Build FOREIGN KEY (BuildID)
        REFERENCES dbo.PCBuilds(BuildID) ON DELETE CASCADE,
    CONSTRAINT FK_BuildItems_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID),
    CONSTRAINT CK_BuildItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_BuildItems_Price CHECK (UnitPriceSnapshot >= 0),
    CONSTRAINT UQ_BuildItems_Build_Component UNIQUE (BuildID, ComponentID)
);
GO

CREATE INDEX IX_PCBuilds_User_Created
    ON dbo.PCBuilds(UserID, CreatedAt DESC);
GO

/* =========================================================
   6. PREBUILT CONFIGURATIONS
   ========================================================= */

CREATE TABLE dbo.PrebuiltConfigs (
    PrebuiltConfigID  INT IDENTITY(1,1) PRIMARY KEY,
    ConfigName        NVARCHAR(150) NOT NULL,
    Description       NVARCHAR(1000) NULL,
    TargetAudience    NVARCHAR(100) NULL,
    BudgetMin         DECIMAL(18,2) NOT NULL,
    BudgetMax         DECIMAL(18,2) NOT NULL,
    IsActive          BIT NOT NULL
        CONSTRAINT DF_PrebuiltConfigs_Active DEFAULT (1),
    CreatedAt         DATETIME2(0) NOT NULL
        CONSTRAINT DF_PrebuiltConfigs_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT CK_PrebuiltConfigs_Budget CHECK
        (BudgetMin >= 0 AND BudgetMax >= BudgetMin)
);
GO

CREATE TABLE dbo.PrebuiltConfigItems (
    PrebuiltConfigItemID INT IDENTITY(1,1) PRIMARY KEY,
    PrebuiltConfigID     INT NOT NULL,
    ComponentID          INT NOT NULL,
    Quantity             TINYINT NOT NULL
        CONSTRAINT DF_PrebuiltItems_Quantity DEFAULT (1),
    CONSTRAINT FK_PrebuiltItems_Config FOREIGN KEY (PrebuiltConfigID)
        REFERENCES dbo.PrebuiltConfigs(PrebuiltConfigID) ON DELETE CASCADE,
    CONSTRAINT FK_PrebuiltItems_Component FOREIGN KEY (ComponentID)
        REFERENCES dbo.Components(ComponentID),
    CONSTRAINT CK_PrebuiltItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT UQ_PrebuiltItems_Config_Component
        UNIQUE (PrebuiltConfigID, ComponentID)
);
GO

/* =========================================================
   7. TROUBLESHOOTING DECISION TREE
   Root node has ParentNodeID = NULL.
   Option rows connect a question's answer to the next node.
   A terminal node contains a diagnosis.
   ========================================================= */

CREATE TABLE dbo.TroubleshootingNodes (
    NodeID             INT IDENTITY(1,1) PRIMARY KEY,
    NodeCode           VARCHAR(50) NOT NULL,
    Title              NVARCHAR(200) NOT NULL,
    NodeType           VARCHAR(20) NOT NULL,
    QuestionText       NVARCHAR(1000) NULL,
    Diagnosis          NVARCHAR(2000) NULL,
    RecommendedAction  NVARCHAR(2000) NULL,
    IsActive            BIT NOT NULL
        CONSTRAINT DF_TroubleshootingNodes_Active DEFAULT (1),
    CONSTRAINT UQ_TroubleshootingNodes_Code UNIQUE (NodeCode),
    CONSTRAINT CK_TroubleshootingNodes_Type
        CHECK (NodeType IN ('Question','Diagnosis')),
    CONSTRAINT CK_TroubleshootingNodes_Content CHECK (
        (NodeType = 'Question' AND QuestionText IS NOT NULL)
        OR
        (NodeType = 'Diagnosis' AND Diagnosis IS NOT NULL)
    )
);
GO

CREATE TABLE dbo.TroubleshootingOptions (
    OptionID           INT IDENTITY(1,1) PRIMARY KEY,
    NodeID             INT NOT NULL,
    OptionText         NVARCHAR(300) NOT NULL,
    NextNodeID         INT NOT NULL,
    SortOrder          SMALLINT NOT NULL
        CONSTRAINT DF_TroubleshootingOptions_Sort DEFAULT (1),
    CONSTRAINT FK_TroubleshootingOptions_Node FOREIGN KEY (NodeID)
        REFERENCES dbo.TroubleshootingNodes(NodeID) ON DELETE CASCADE,
    CONSTRAINT FK_TroubleshootingOptions_NextNode FOREIGN KEY (NextNodeID)
        REFERENCES dbo.TroubleshootingNodes(NodeID),
    CONSTRAINT CK_TroubleshootingOptions_Sort CHECK (SortOrder > 0),
    CONSTRAINT UQ_TroubleshootingOptions_Node_Order UNIQUE (NodeID, SortOrder)
);
GO

/* =========================================================
   8. BASIC SEED DATA
   ========================================================= */

INSERT INTO dbo.Categories (CategoryCode, CategoryName, Description)
VALUES
('CPU', N'CPU', N'Bộ xử lý'),
('MOTHERBOARD', N'Mainboard', N'Bo mạch chủ'),
('RAM', N'RAM', N'Bộ nhớ trong'),
('GPU', N'GPU', N'Card đồ họa'),
('PSU', N'PSU', N'Bộ nguồn'),
('STORAGE', N'Storage', N'Ổ lưu trữ'),
('CASE', N'Case', N'Vỏ máy tính'),
('COOLING', N'Cooling', N'Tản nhiệt'),
('OTHER', N'Other', N'Linh kiện khác');

INSERT INTO dbo.Brands (BrandName)
VALUES
(N'Intel'), (N'AMD'), (N'NVIDIA'), (N'ASUS'), (N'MSI'),
(N'Gigabyte'), (N'Corsair'), (N'Kingston'), (N'Samsung'),
(N'Western Digital'), (N'Seasonic'), (N'Cooler Master');

GO

/* =========================================================
   12. BENCHMARK SCORES (source-aware; not all scores are comparable)
   MetricCode examples: CPU_MULTI, CPU_SINGLE, GPU_GAMING, GPU_PRODUCTIVITY
   Score values must come from a recorded benchmark source.
   ========================================================= */
CREATE TABLE dbo.BenchmarkScores (
    BenchmarkScoreID INT IDENTITY(1,1) PRIMARY KEY,
    ComponentID INT NOT NULL,
    MetricCode VARCHAR(40) NOT NULL,
    BenchmarkName NVARCHAR(150) NOT NULL,
    Score DECIMAL(12,3) NOT NULL,
    SourceName NVARCHAR(150) NOT NULL,
    SourceUrl NVARCHAR(1000) NULL,
    RecordedAt DATETIME2(0) NOT NULL CONSTRAINT DF_BenchmarkScores_RecordedAt DEFAULT (SYSUTCDATETIME()),
    IsVerified BIT NOT NULL CONSTRAINT DF_BenchmarkScores_IsVerified DEFAULT (0),
    CONSTRAINT FK_BenchmarkScores_Components FOREIGN KEY (ComponentID) REFERENCES dbo.Components(ComponentID),
    CONSTRAINT CK_BenchmarkScores_Score CHECK (Score > 0),
    CONSTRAINT UQ_BenchmarkScores_ComponentMetric UNIQUE (ComponentID, MetricCode, BenchmarkName)
);
CREATE INDEX IX_BenchmarkScores_Metric ON dbo.BenchmarkScores(MetricCode, ComponentID) INCLUDE (Score, IsVerified);
GO
