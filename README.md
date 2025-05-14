dotnet ef migrations remove
dotnet ef migrations add UpdateFromDB
dotnet ef database update
Server=localhost\\SQLEXPRESS02;Database=PBL3;Trusted_Connection=True;TrustServerCertificate=True;