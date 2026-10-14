USE PCBuilderDB;
GO
CREATE OR ALTER PROCEDURE dbo.sp_User_GetForLogin @Username NVARCHAR(50)
AS
BEGIN
 SET NOCOUNT ON;
 SELECT TOP (1) UserID, Username, FullName, UserRole, PasswordHash
 FROM dbo.Users WHERE Username=@Username AND IsActive=1;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_User_Create
 @Username NVARCHAR(50), @Email NVARCHAR(255), @PasswordHash NVARCHAR(255), @FullName NVARCHAR(150)=NULL, @UserRole VARCHAR(10)='User'
AS
BEGIN
 SET NOCOUNT ON;
 IF @UserRole NOT IN ('Admin','User') THROW 50001, N'Vai trò không hợp lệ.', 1;
 INSERT dbo.Users(Username,Email,PasswordHash,FullName,UserRole) VALUES(@Username,@Email,@PasswordHash,@FullName,@UserRole);
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Component_Search
 @Keyword NVARCHAR(200)=NULL, @CategoryID INT=NULL, @IncludeInactive BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SELECT c.ComponentID,c.CategoryID,cat.CategoryCode,cat.CategoryName,b.BrandID,b.BrandName,c.ModelName,c.SKU,c.Price,c.StockQuantity,c.IsActive,
        cpu.Socket AS Socket, COALESCE(cpu.MemoryType, mb.MemoryType, ram.MemoryType) AS MemoryType,
        cpu.TDP_W, gpu.PowerDraw_W, psu.Wattage_W, gpu.LengthMM, cs.MaxGPULengthMM, cs.MaxCoolerHeightMM,
        mb.FormFactor AS FormFactor,
        cpuScore.Score AS CpuBenchmarkScore, gpuScore.Score AS GpuBenchmarkScore
 FROM dbo.Components c JOIN dbo.Categories cat ON cat.CategoryID=c.CategoryID JOIN dbo.Brands b ON b.BrandID=c.BrandID
 LEFT JOIN dbo.CPU_Specs cpu ON cpu.ComponentID=c.ComponentID
 LEFT JOIN dbo.Motherboard_Specs mb ON mb.ComponentID=c.ComponentID
 LEFT JOIN dbo.RAM_Specs ram ON ram.ComponentID=c.ComponentID
 LEFT JOIN dbo.GPU_Specs gpu ON gpu.ComponentID=c.ComponentID
 LEFT JOIN dbo.PSU_Specs psu ON psu.ComponentID=c.ComponentID
 LEFT JOIN dbo.Case_Specs cs ON cs.ComponentID=c.ComponentID
 OUTER APPLY (SELECT TOP (1) bs.Score FROM dbo.BenchmarkScores bs WHERE bs.ComponentID=c.ComponentID AND bs.MetricCode IN ('CPU_MULTI','CPU_SINGLE') ORDER BY CASE WHEN bs.MetricCode='CPU_MULTI' THEN 0 ELSE 1 END, bs.IsVerified DESC, bs.RecordedAt DESC) cpuScore
 OUTER APPLY (SELECT TOP (1) bs.Score FROM dbo.BenchmarkScores bs WHERE bs.ComponentID=c.ComponentID AND bs.MetricCode IN ('GPU_GAMING','GPU_PRODUCTIVITY') ORDER BY CASE WHEN bs.MetricCode='GPU_GAMING' THEN 0 ELSE 1 END, bs.IsVerified DESC, bs.RecordedAt DESC) gpuScore
 WHERE (@IncludeInactive=1 OR c.IsActive=1)
 AND (@CategoryID IS NULL OR c.CategoryID=@CategoryID)
 AND (@Keyword IS NULL OR c.ModelName LIKE N'%'+@Keyword+'%' OR c.SKU LIKE N'%'+@Keyword+'%' OR b.BrandName LIKE N'%'+@Keyword+'%' OR cat.CategoryName LIKE N'%'+@Keyword+'%')
 ORDER BY cat.CategoryName,b.BrandName,c.ModelName;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Component_Create
 @CategoryID INT,@BrandID INT,@ModelName NVARCHAR(200),@SKU NVARCHAR(100)=NULL,@Description NVARCHAR(MAX)=NULL,@Price DECIMAL(18,2),@StockQuantity INT=0,@ImageURL NVARCHAR(1000)=NULL
AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.Components(CategoryID,BrandID,ModelName,SKU,Description,Price,StockQuantity,ImageURL)
 VALUES(@CategoryID,@BrandID,@ModelName,@SKU,@Description,@Price,@StockQuantity,@ImageURL);
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Component_Update
 @ComponentID INT,@CategoryID INT,@BrandID INT,@ModelName NVARCHAR(200),@SKU NVARCHAR(100)=NULL,@Description NVARCHAR(MAX)=NULL,@Price DECIMAL(18,2),@StockQuantity INT=0,@ImageURL NVARCHAR(1000)=NULL,@IsActive BIT=1
AS
BEGIN
 SET NOCOUNT ON;
 UPDATE dbo.Components SET CategoryID=@CategoryID,BrandID=@BrandID,ModelName=@ModelName,SKU=@SKU,Description=@Description,Price=@Price,StockQuantity=@StockQuantity,ImageURL=@ImageURL,IsActive=@IsActive,UpdatedAt=SYSUTCDATETIME()
 WHERE ComponentID=@ComponentID;
 IF @@ROWCOUNT=0 THROW 50002, N'Không tìm thấy linh kiện.', 1;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Component_Delete @ComponentID INT
AS
BEGIN
 SET NOCOUNT ON;
 -- Soft delete để giữ lịch sử cấu hình và tránh phá vỡ khóa ngoại.
 UPDATE dbo.Components SET IsActive=0,UpdatedAt=SYSUTCDATETIME() WHERE ComponentID=@ComponentID;
 IF @@ROWCOUNT=0 THROW 50003, N'Không tìm thấy linh kiện.', 1;
END;
GO
