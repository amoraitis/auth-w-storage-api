# About AuthWithStorage

This project's goal is to provide an authentication API with integrated storage capabilities, leveraging .NET 8.0 and SQL Server.

## Heads-up

This project is only partially implemented and some of the task's goals are not implemented.

## Requirements

- .NET 8.0 or superior
- SQL Server Express 2016 or superior
- Optional: Docker for one-click deployment

## Set up

### VS/Terminal

1. Clone this project by running `git clone https://github.com/amoraitis/auth-w-storage-api.git` in your terminal. If you don't have Git installed, you can download and unzip the project.

2. Add mandatory environment variables.

**Windows only examples**

*Terminal (as admin)*

```bash
setx AuthAPIWithStorage_ConnectionString "YourConnectionString" /M
setx AuthAPIWithStorage_Connections_BlobStorage "YourStorageConnectionString" /M
setx AuthAPIWithStorage_DataCache "YourRedisConnectionString" /M
setx AuthAPIWithStorage_SysConnectionString "YourSysConnectionString" /M
```

*Powershell (as admin)*

```bash
```powershell
[System.Environment]::SetEnvironmentVariable("AuthAPIWithStorage_ConnectionString", "YourConnectionString", [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("AuthAPIWithStorage_Connections_BlobStorage", "YourStorageConnectionString", [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("AuthAPIWithStorage_DataCache", "YourRedisConnectionString", [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("AuthAPIWithStorage_SysConnectionString", "YourSysConnectionString", [System.EnvironmentVariableTarget]::Machine)
```
```

3. Navigate to the `src/AuthWithStorage.API` folder by running `cd src/AuthWithStorage.API` or manually navigating to the folder.

4. Run the command `dotnet restore` to install all dependencies.

5. Run the command `dotnet build` to compile the project.

6. Run the command `dotnet run` to start the API.

7. Your application will be running at `http://localhost:5000/swagger`.

### Docker

1. Clone this project by running `git clone https://github.com/your-repo/auth-api-with-storage.git` in your terminal. If you don't have Git installed, you can download and unzip the project.

2. Navigate to the project folder by running `cd auth-api-with-storage` or manually navigating to the folder.

3. Add mandatory environment variables in a `.env` file in the root of the project. See the example below. (Run `touch .env` in the terminal to create a new file)

```env
DB_CONNECTION_STRING=YourValue
SYS_DB_CONNECTION_STRING=YourValue
DB_PASSWORD=YourValue
ASPNETCORE_ENVIRONMENT=Production
AzureStorage_CONNECTION_STRING=YourValue
Redis_CONNECTION_STRING=YourValue
```

4. Run the command `docker-compose --env-file demo.env up --build -d` to start the API in a Docker container.

5. Your application will be running at `http://localhost:8000/swagger`.