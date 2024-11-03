#pragma once
#include "curl_client.h"
#include <Windows.h>
#include <memory>

#ifdef _DEBUG
#define U_LOG( format, ... ) printf( "[ Azalea ] " format "\n", ##__VA_ARGS__ )
#else:
#define U_LOG( format, ... ) void(0);
#endif 

class c_server_api
{
public:
	c_server_api();

	bool auth_by_token(const std::string& token);
	bool auth_by_login(std::tuple<std::string, std::string, std::string> login_data);

	std::string auth_token;
	std::unique_ptr<c_server_client> m_client;
private:
}; inline c_server_api server_api;