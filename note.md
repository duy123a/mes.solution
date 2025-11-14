dotnet ef migrations add InitIdentityOpenIddict --project S1.Core

dotnet ef migrations remove --project S1.Core

dotnet ef database update --project S1.Core

dotnet ef database update 0 --project S1.Core

dotnet ef database drop --project S1.Core

dotnet publish -c Release

DOTNET_URLS="https://localhost:7225;http://localhost:5019" dotnet S1.Core.dll

dotnet tool update --global dotnet-ef