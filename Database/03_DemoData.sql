USE PCBuilderDB;
GO
/* Demo data only. Prices and specifications are illustrative; verify before real use. */
IF NOT EXISTS (SELECT 1 FROM dbo.Components)
BEGIN
 DECLARE @AMD INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'AMD');
 DECLARE @Intel INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'Intel');
 DECLARE @Asus INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'ASUS');
 DECLARE @MSI INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'MSI');
 DECLARE @Corsair INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'Corsair');
 DECLARE @Kingston INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'Kingston');
 DECLARE @Nvidia INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'NVIDIA');
 DECLARE @Seasonic INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'Seasonic');
 DECLARE @Samsung INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'Samsung');
 DECLARE @CM INT=(SELECT BrandID FROM dbo.Brands WHERE BrandName=N'Cooler Master');
 DECLARE @CPU INT=(SELECT CategoryID FROM dbo.Categories WHERE CategoryCode='CPU');
 DECLARE @MB INT=(SELECT CategoryID FROM dbo.Categories WHERE CategoryCode='MOTHERBOARD');
 DECLARE @RAM INT=(SELECT CategoryID FROM dbo.Categories WHERE CategoryCode='RAM');
 DECLARE @GPU INT=(SELECT CategoryID FROM dbo.Categories WHERE CategoryCode='GPU');
 DECLARE @PSU INT=(SELECT CategoryID FROM dbo.Categories WHERE CategoryCode='PSU');
 DECLARE @SSD INT=(SELECT CategoryID FROM dbo.Categories WHERE CategoryCode='STORAGE');
 DECLARE @CASE INT=(SELECT CategoryID FROM dbo.Categories WHERE CategoryCode='CASE');
 DECLARE @COOL INT=(SELECT CategoryID FROM dbo.Categories WHERE CategoryCode='COOLING');
 IF @CPU IS NULL OR @MB IS NULL OR @RAM IS NULL OR @GPU IS NULL OR @PSU IS NULL OR @SSD IS NULL OR @CASE IS NULL OR @COOL IS NULL
     THROW 50001, 'Required Categories are missing. Run 01_CreateDatabase.sql first.', 1;
 IF @AMD IS NULL OR @Intel IS NULL OR @Asus IS NULL OR @MSI IS NULL OR @Corsair IS NULL OR @Kingston IS NULL OR @Nvidia IS NULL OR @Seasonic IS NULL OR @Samsung IS NULL OR @CM IS NULL
     THROW 50002, 'Required Brands are missing. Run 01_CreateDatabase.sql first.', 1;
 INSERT dbo.Components(CategoryID,BrandID,ModelName,SKU,Price,StockQuantity) VALUES
 (@CPU,@AMD,N'Ryzen 5 7600',N'DEMO-CPU-AM5',4890000,10),
 (@CPU,@Intel,N'Core i5-12400F',N'DEMO-CPU-LGA1700',3290000,10),
 (@MB,@Asus,N'B650M DDR5 Demo',N'DEMO-MB-AM5',3290000,8),
 (@MB,@MSI,N'B760M DDR4 Demo',N'DEMO-MB-LGA1700',2590000,8),
 (@RAM,@Corsair,N'Vengeance DDR5 32GB',N'DEMO-RAM-DDR5',2190000,20),
 (@RAM,@Kingston,N'Fury DDR4 16GB',N'DEMO-RAM-DDR4',990000,20),
 (@GPU,@Nvidia,N'GeForce RTX Demo 12GB',N'DEMO-GPU',9990000,5),
 (@PSU,@Seasonic,N'650W 80+ Bronze Demo',N'DEMO-PSU',1590000,10),
 (@SSD,@Samsung,N'NVMe SSD 1TB Demo',N'DEMO-SSD',1690000,12),
 (@CASE,@CM,N'Micro-ATX Airflow Case Demo',N'DEMO-CASE',1190000,7),
 (@COOL,@CM,N'Tower Air Cooler Demo',N'DEMO-COOL',590000,10);
 INSERT dbo.CPU_Specs(ComponentID,Socket,CoreCount,ThreadCount,BaseClockGHz,BoostClockGHz,TDP_W,IntegratedGPU,MemoryType)
 SELECT ComponentID, CASE ModelName WHEN N'Ryzen 5 7600' THEN N'AM5' ELSE N'LGA1700' END,6,12,3.5,5.1,65,CASE WHEN ModelName=N'Ryzen 5 7600' THEN 1 ELSE 0 END,CASE WHEN ModelName=N'Ryzen 5 7600' THEN 'DDR5' ELSE 'DDR4' END FROM dbo.Components WHERE SKU IN ('DEMO-CPU-AM5','DEMO-CPU-LGA1700');
 INSERT dbo.Motherboard_Specs(ComponentID,Socket,Chipset,FormFactor,MemoryType,MemorySlots,MaxMemoryGB,MaxMemorySpeedMHz,M2Slots,SATAConnections)
 SELECT ComponentID,CASE SKU WHEN 'DEMO-MB-AM5' THEN N'AM5' ELSE N'LGA1700' END,CASE SKU WHEN 'DEMO-MB-AM5' THEN N'B650' ELSE N'B760' END,'Micro-ATX',CASE SKU WHEN 'DEMO-MB-AM5' THEN 'DDR5' ELSE 'DDR4' END,4,128,6000,2,4 FROM dbo.Components WHERE SKU IN ('DEMO-MB-AM5','DEMO-MB-LGA1700');
 INSERT dbo.RAM_Specs(ComponentID,MemoryType,CapacityGB,SpeedMHz,ModuleCount,CASLatency,FormFactor)
 SELECT ComponentID,CASE SKU WHEN 'DEMO-RAM-DDR5' THEN 'DDR5' ELSE 'DDR4' END,CASE SKU WHEN 'DEMO-RAM-DDR5' THEN 32 ELSE 16 END,CASE SKU WHEN 'DEMO-RAM-DDR5' THEN 6000 ELSE 3200 END,2,CASE SKU WHEN 'DEMO-RAM-DDR5' THEN 36 ELSE 16 END,'DIMM' FROM dbo.Components WHERE SKU IN ('DEMO-RAM-DDR5','DEMO-RAM-DDR4');
 INSERT dbo.GPU_Specs(ComponentID,Chipset,VRAM_GB,LengthMM,SlotWidth,RecommendedPSU_W,PowerDraw_W,PowerConnector)
 SELECT ComponentID,N'Demo GPU',12,280,2.5,650,250,N'2x8-pin' FROM dbo.Components WHERE SKU='DEMO-GPU';
 INSERT dbo.PSU_Specs(ComponentID,Wattage_W,EfficiencyRating,FormFactor,ModularType)
 SELECT ComponentID,650,'80+ Bronze','ATX','Semi-Modular' FROM dbo.Components WHERE SKU='DEMO-PSU';
 INSERT dbo.Storage_Specs(ComponentID,StorageType,CapacityGB,Interface,FormFactor,ReadSpeedMBps,WriteSpeedMBps)
 SELECT ComponentID,'NVMe SSD',1000,'PCIe 4.0','M.2 2280',5000,4200 FROM dbo.Components WHERE SKU='DEMO-SSD';
 INSERT dbo.Case_Specs(ComponentID,SupportedFormFactors,MaxGPULengthMM,MaxCoolerHeightMM,MaxPSULengthMM,RadiatorSupport)
 SELECT ComponentID,N'Micro-ATX,Mini-ITX',330,160,180,N'240mm front' FROM dbo.Components WHERE SKU='DEMO-CASE';
 INSERT dbo.Cooling_Specs(ComponentID,CoolingType,SupportedSockets,HeightMM,RadiatorSizeMM,TDP_Rating_W)
 SELECT ComponentID,'Air',N'AM5,LGA1700',155,NULL,180 FROM dbo.Components WHERE SKU='DEMO-COOL';
END;
GO

/* DEMO-ONLY illustrative score values. Replace with scores from cited benchmark sources before real recommendations. */
INSERT dbo.BenchmarkScores(ComponentID, MetricCode, BenchmarkName, Score, SourceName, SourceUrl, IsVerified)
SELECT c.ComponentID, v.MetricCode, N'DEMO illustrative index', v.Score, N'Demo data - not measured', NULL, 0
FROM dbo.Components c
JOIN (VALUES
 (N'DEMO-CPU-AM5','CPU_MULTI',14500.0),
 (N'DEMO-CPU-LGA1700','CPU_MULTI',12300.0),
 (N'DEMO-GPU','GPU_GAMING',16000.0)
) v(SKU,MetricCode,Score) ON v.SKU=c.SKU
WHERE NOT EXISTS (SELECT 1 FROM dbo.BenchmarkScores b WHERE b.ComponentID=c.ComponentID AND b.MetricCode=v.MetricCode AND b.BenchmarkName=N'DEMO illustrative index');
GO
