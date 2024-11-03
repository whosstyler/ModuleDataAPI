#include "curl_client.h"
#include "api_module_manager.h"

signature* module_t::get_signature(const std::string& sig_name) {
    auto it = signatures.find(sig_name);
    if(it != signatures.end()) {
        return &it->second;
    }
    return nullptr;
}

member* module_t::get_member(const std::string& class_name, const std::string& member_name) {
    auto class_it = members_by_class.find(class_name);
    if(class_it != members_by_class.end()) {
        auto member_it = class_it->second.find(member_name);
        if(member_it != class_it->second.end()) {
            return &member_it->second;
        }
    }
    return nullptr;
}

module_t* api_module_manager::get_module(const std::string& module_name) {
    auto it = modules_.find(module_name);
    if(it != modules_.end()) {
        return &it->second;
    }
    return nullptr;
}

void api_module_manager::fetch_and_add_module(c_server_client& client, const std::string& module_name) {
    auto response = client.get("/api/" + module_name, token_);
    if(response.has_value()) {
        json json_data = json::parse(response->body);
        module_t mod = parse_module(json_data);
        modules_[module_name] = mod;
    }
}

void api_module_manager::initialize(c_server_client& client, std::string token) {
    token_ = token;

    fetch_and_add_module(client, "engine");
    fetch_and_add_module(client, "client");
}


module_t api_module_manager::parse_module(const json& json_data) {
    module_t mod;
    mod.name = json_data["name"];

    // Parse signatures
    for(const auto& sig : json_data["signatures"]) {
        mod.signatures[sig["name"]] = signature{sig["name"], sig["pattern"]};
    }

    // Parse members by class
    for(const auto& mem : json_data["members"]) {
        mod.members_by_class[mem["className"]][mem["memberName"]] = member{
            mem["className"], mem["memberName"], mem["type"]
        };
    }

    return mod;
}