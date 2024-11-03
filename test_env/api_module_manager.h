#pragma once
using json = nlohmann::json;

struct signature {
    std::string name;
    std::string pattern;
};

struct member {
    std::string class_name;
    std::string member_name;
    std::string type;
};

class module_t {
public:
    std::string name;
    std::map<std::string, signature> signatures;
    std::map<std::string, std::map<std::string, member>> members_by_class;

    signature* get_signature(const std::string& sig_name);
    member* get_member(const std::string& class_name, const std::string& member_name);
};

class api_module_manager {
public:
    void initialize(c_server_client& client, std::string token);
    void fetch_and_add_module(c_server_client& client, const std::string& module_name);
    module_t* get_module(const std::string& module_name);

private:
    std::map<std::string, module_t> modules_;

	std::string token_;

    module_t parse_module(const json& json_data);
};
