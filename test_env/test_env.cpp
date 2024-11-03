#include <iostream>
#include "server_api.h"
#include "api_module_manager.h"

int main()
{
    bool login_attempt = server_api.auth_by_login(
        std::make_tuple("Azalea", "Azalea", "Azalea"));
    
    if(!login_attempt)
    {
        U_LOG("Login attempt failed.");
        return FALSE;
    }
    
    U_LOG("Login attempt successful.");
   
    api_module_manager manager;

    // print server_api auth tokne
	//std::cout << server_api.auth_token << std::endl;
     
    auto server_client = server_api.m_client.get(); // get raw pointer
    manager.initialize(*(c_server_client*)server_client, server_api.auth_token); // pass the pointer to initialize
    
	auto engine_module = manager.get_module("engine");
	auto client_module = manager.get_module("client");
  
	std::cout << "engine_module: " << engine_module->name << std::endl;
	std::cout << "client_module: " << client_module->name << std::endl;

	auto m_iMaxClients = engine_module->get_member("CEngineClient", "m_iMaxClients");
	std::cout << "m_iMaxClients: " << m_iMaxClients->class_name << "." 
        << m_iMaxClients->member_name << ", Type: " << m_iMaxClients->type << std::endl;


    // Accessing engine module
    //if(module_t* engine = manager.get_module("engine")) {
    //    if(signature* sig = engine->get_signature("CEngineClient")) {
    //        std::cout << "Signature: " << sig->name << ", Pattern: " << sig->pattern << std::endl;
    //    }
    //
    //    if(member* mem = engine->get_member("CEngineClient", "m_iMaxClients")) {
    //        std::cout << "Member: " << mem->class_name << "." << mem->member_name
    //            << ", Type: " << mem->type << std::endl;
    //    }
    //}
    //
    //// Accessing client module
    //if(module_t* client_mod = manager.get_module("client")) {
    //    if(member* mem = client_mod->get_member("CCollisionProperty", "m_usSolidFlags")) {
    //        std::cout << "Member: " << mem->class_name << "." << mem->member_name
    //            << ", Type: " << mem->type << std::endl;
    //    }
    //
    //    if(signature* sig = client_mod->get_signature("CCollisionProperty")) {
    //        std::cout << "Signature: " << sig->name << ", Pattern: " << sig->pattern << std::endl;
    //    }
    //}

	system("pause");

}
