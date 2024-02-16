# Ensek

This solution consists of 2 applications: Client - a React app, Server - .NET Web Api project

Steps to start both applications:
1. Clone this repository on your local machine
2. Open the Ensek folder
3. To run the Web Api - open PowerShell and run this command `dotnet run --project ./Server/Ensek.Api/Ensek.Api.csproj`
   This will start the appication on this address https://localhost:7264/ (assuming the port is free)
   You can access the Swagger page via this link https://localhost:7264/swagger/index.html
4. To run the React app - open PowerShell and run this command `npm --prefix ./Client run dev`
   This will start the appication on this address http://localhost:5173/ (assuming the port is free, if not - a new one will be generated)