using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using backend.Models;

namespace backend.Repositories
{
    public class ModuleRepository
    {
        private readonly List<Module> _modules;

        public ModuleRepository()
        {
            // Adjust the path to your JSON file as needed
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules.json");
            var json = File.ReadAllText(jsonFilePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var moduleData = JsonSerializer.Deserialize<ModuleData>(json, options);
            _modules = moduleData.Modules;
        }

        public Module GetModuleByName(string name)
        {
            return _modules.Find(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class ModuleData
    {
        public List<Module> Modules { get; set; }
    }
}
