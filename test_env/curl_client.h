#include "curl/curl.h"
#include "json.h"
#include <string>
#include <memory>
#include <iostream>

using json = nlohmann::json;

class c_server_client {
public:
    c_server_client(const std::string& base_url)
        : base_url_(base_url) {
        curl_global_init(CURL_GLOBAL_DEFAULT);
        curl_ = curl_easy_init();
    }

    ~c_server_client() {
        if(curl_) {
            curl_easy_cleanup(curl_);
        }
        curl_global_cleanup();
    }

    struct Response {
        long status_code;
        std::string body;
    };

    // POST request
    std::optional<Response> post(const std::string& endpoint, const std::string& payload, const std::string& content_type) {
        if(!curl_) return std::nullopt;

        std::string url = base_url_ + endpoint;
        curl_easy_setopt(curl_, CURLOPT_URL, url.c_str());
        curl_easy_setopt(curl_, CURLOPT_POSTFIELDS, payload.c_str());

        struct curl_slist* headers = nullptr;
        headers = curl_slist_append(headers, ("Content-Type: " + content_type).c_str());
        curl_easy_setopt(curl_, CURLOPT_HTTPHEADER, headers);

        std::string response_body;
        curl_easy_setopt(curl_, CURLOPT_WRITEFUNCTION, c_server_client::write_callback);
        curl_easy_setopt(curl_, CURLOPT_WRITEDATA, &response_body);

        CURLcode res = curl_easy_perform(curl_);
        if(res != CURLE_OK) {
            fprintf(stderr, "curl_easy_perform() failed: %s\n", curl_easy_strerror(res));
            curl_slist_free_all(headers);
            return std::nullopt;
        }

        long http_code = 0;
        curl_easy_getinfo(curl_, CURLINFO_RESPONSE_CODE, &http_code);
        curl_slist_free_all(headers);

        return Response{http_code, response_body};
    }

    // GET request with Bearer token
    std::optional<Response> get(const std::string& endpoint, const std::string& bearer_token) {
        if(!curl_) return std::nullopt;

        // Reset CURL handle to ensure no previous POST or custom settings affect the request
        curl_easy_reset(curl_);

        std::string url = base_url_ + endpoint;
        curl_easy_setopt(curl_, CURLOPT_URL, url.c_str());

        // Ensure the request is a GET
        curl_easy_setopt(curl_, CURLOPT_HTTPGET, 1L);

        struct curl_slist* headers = nullptr;
        if(!bearer_token.empty()) {
            std::string auth_header = "Authorization: Bearer " + bearer_token;
            headers = curl_slist_append(headers, auth_header.c_str());
        }

        // Set Content-Type and Accept headers
        headers = curl_slist_append(headers, "Content-Type: application/json");
        headers = curl_slist_append(headers, "Accept: application/json");

        curl_easy_setopt(curl_, CURLOPT_HTTPHEADER, headers);

        // Enable verbose mode for debugging
      //  curl_easy_setopt(curl_, CURLOPT_VERBOSE, 1L);

        std::string response_body;
        curl_easy_setopt(curl_, CURLOPT_WRITEFUNCTION, c_server_client::write_callback);
        curl_easy_setopt(curl_, CURLOPT_WRITEDATA, &response_body);

        // Perform the GET request
        CURLcode res = curl_easy_perform(curl_);
        if(res != CURLE_OK) {
            fprintf(stderr, "curl_easy_perform() failed: %s\n", curl_easy_strerror(res));
            if(headers) curl_slist_free_all(headers);
            return std::nullopt;
        }

        long http_code = 0;
        curl_easy_getinfo(curl_, CURLINFO_RESPONSE_CODE, &http_code);

        if(headers) curl_slist_free_all(headers);  // Free headers after the request

        return Response{http_code, response_body};
    }

private:
    CURL* curl_;
    std::string base_url_;

    static size_t write_callback(void* contents, size_t size, size_t nmemb, std::string* userp) {
        size_t total_size = size * nmemb;
        userp->append((char*)contents, total_size);
        return total_size;
    }
};
