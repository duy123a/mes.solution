dotnet ef migrations add InitIdentityOpenIddict --project S1.Core

dotnet ef migrations remove --project S1.Core

dotnet ef database update --project S1.Core

dotnet ef database update 0 --project S1.Core

dotnet ef database drop --project S1.Core

dotnet publish -c Release

DOTNET_URLS="https://localhost:7225;http://localhost:5019" dotnet S1.Core.dll

dotnet tool update --global dotnet-ef

"DefaultConnection": "Data Source=172.27.190.134,1433;Initial Catalog=Mes_Deha;TrustServerCertificate=True;User ID=sa;Password=7551656As!"

"DefaultConnection": "Data Source=172.24.17.122,1433,1433;Initial Catalog=Mes_Deha;TrustServerCertificate=True;User ID=sa;Password=7551656As!"