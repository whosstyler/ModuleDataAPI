# Module Data Streaming API

This API provides secure server-side streaming of important module data, which can be accessed by a C++ client. The client authenticates with the server, fetches module data, and manages it locally. While `engine.dll` and `client.dll` modules are used as examples, this API can be configured to serve any desired module data.

## Project Structure

### .NET Backend

- **Controllers**
  - `ModuleController.cs`: Manages API requests related to module data.
  - `AuthController.cs`: Manages user registration, login, and token validation.
- **Repositories**
  - `ModuleRepository.cs`: Loads module data from a JSON file (`modules.json`), which can be customized to include any module.

### C++ Client

- **`curl_client.h`** - Manages HTTP requests.
- **`server_api.h` / `server_api.cpp`** - Handles authentication requests to the server.
- **`api_module_manager.h` / `api_module_manager.cpp`** - Manages module data, including fetching and parsing JSON data.

---

## Endpoints

### Authentication Endpoints (`/api/oauth`)

- **POST /register**: Register a new user.
- **POST /login**: User login, returning a token.
- **POST /token**: Validates an existing token.

### Module Data Endpoints (`/api`)

These endpoints provide access to module data. Although `client.dll` and `engine.dll` are shown as examples, the API can be adapted to support any module by configuring `modules.json`.

- **GET /client**: Retrieve general information about `client.dll`.
- **GET /client/sigs**: Retrieve all signatures for `client.dll`.
- **GET /client/members/{className}**: Retrieve class members within `client.dll`.
- **GET /engine**: Retrieve general information about `engine.dll`.
- **GET /engine/sigs**: Retrieve all signatures for `engine.dll`.

## C++ Client Setup

### Prerequisites

- **libcurl**: Required for HTTP requests.
- **JSON for Modern C++** (`nlohmann/json.hpp`): Used for JSON parsing.

### Building the C++ Client

1. **Include Directories**: Ensure `libcurl` and `nlohmann/json.hpp` are accessible.
2. **Compile**: Link against `libcurl`.

### Usage Example

1. **Authentication**:
   ```cpp
   c_server_api server_api;
   if (server_api.auth_by_login({"username", "password", "HWID"})) {
       std::cout << "Authentication successful!" << std::endl;
   }
   ```

2. **Fetching Module Data**:
   ```cpp
   c_server_client client("https://localhost:7232");
   api_module_manager manager;
   manager.initialize(client, server_api.auth_token);
   module_t* engine_module = manager.get_module("engine");
   ```

   In this example, the module `engine` is retrieved, but any other module specified in `modules.json` can be accessed similarly.

---

## Customizing Module Data

To adapt the API for additional or different modules, update `modules.json` with the new module details. This flexibility allows you to configure the server to stream any module data required by your application, making `engine` and `client` just sample configurations.

### Example `modules.json` Configuration

```json
{
  "modules": [
    {
      "Name": "client.dll",
      "Signatures": [
        { "Name": "CCollisionProperty", "Pattern": "A1 ? ? ? ? 56 57 8B F9" },
        { "Name": "m_usSolidFlags", "Pattern": "80 3D ? ? ? ? 00 74 09" }
      ],
      "Members": [
        { "ClassName": "CCollisionProperty", "MemberName": "m_usSolidFlags", "Type": "uint8_t" }
      ]
    },
    {
      "Name": "engine.dll",
      "Signatures": [
        { "Name": "CEngineClient", "Pattern": "B9 ? ? ? ? E8 ? ? ? ? 8B 0D ? ? ? ?" }
      ],
      "Members": [
        { "ClassName": "CEngineClient", "MemberName": "m_iMaxClients", "Type": "int" }
      ]
    }
  ]
}
```