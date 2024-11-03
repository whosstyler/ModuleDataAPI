#include "server_api.h"

using namespace nlohmann;

c_server_api::c_server_api()
{
	m_client = std::make_unique<c_server_client>("https://localhost:7232");
}

bool c_server_api::auth_by_token(const std::string& token)
{
	json payload_jsn;
	payload_jsn["token"] = token;

	auto response = m_client->post("/api/oauth/token", payload_jsn.dump(), "application/json");
	if(!response.has_value()) {
		return false;
	}

	json auth_status = json::parse(response->body);
	return auth_status["authorized"].get<bool>();
}

bool c_server_api::auth_by_login(std::tuple<std::string, std::string, std::string> login_data)
{
    json payload_jsn;
    payload_jsn["username"] = std::get<0>(login_data);
    payload_jsn["password"] = std::get<1>(login_data);
    payload_jsn["hwid"] = std::get<2>(login_data);

    auto response = m_client->post("/api/oauth/login", payload_jsn.dump(), "application/json");
    if(!response.has_value()) {
        return false;
    }

    json auth_status = json::parse(response->body);

    // Save the token
    auth_token = auth_status["token"].get<std::string>();
    std::cout << "Auth Token: " << auth_token << std::endl;

    return auth_by_token(auth_token);
}

