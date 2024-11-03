using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using backend.Models;
using backend.Repositories; // Make sure this namespace matches where your ModuleRepository is located

namespace backend.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize] // Require authentication for all actions in this controller
    public class ModuleController : ControllerBase
    {
        private readonly ModuleRepository _moduleRepository;

        public ModuleController(ModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        // GET /api/client
        [HttpGet("client")]
        public IActionResult GetClientModule()
        {
            var clientModule = _moduleRepository.GetModuleByName("client.dll");
            if (clientModule == null)
            {
                return NotFound(new { message = "Module 'client.dll' not found" });
            }
            return Ok(clientModule);
        }

        // GET /api/client/sigs
        [HttpGet("client/sigs")]
        public IActionResult GetClientSignatures()
        {
            var clientModule = _moduleRepository.GetModuleByName("client.dll");
            if (clientModule == null)
            {
                return NotFound(new { message = "Module 'client.dll' not found" });
            }
            return Ok(clientModule.Signatures);
        }

        // GET /api/client/sigs/{sigName}
        [HttpGet("client/sigs/{sigName}")]
        public IActionResult GetClientSignature(string sigName)
        {
            var clientModule = _moduleRepository.GetModuleByName("client.dll");
            if (clientModule == null)
            {
                return NotFound(new { message = "Module 'client.dll' not found" });
            }
            var sig = clientModule.Signatures.Find(s => s.Name == sigName);
            if (sig == null)
            {
                return NotFound(new { message = $"Signature '{sigName}' not found in client.dll" });
            }
            return Ok(sig);
        }

        // GET /api/client/members/{className}
        [HttpGet("client/members/{className}")]
        public IActionResult GetClientClassMembers(string className)
        {
            var clientModule = _moduleRepository.GetModuleByName("client.dll");
            if (clientModule == null)
            {
                return NotFound(new { message = "Module 'client.dll' not found" });
            }
            var members = clientModule.Members.FindAll(m => m.ClassName == className);
            if (members.Count == 0)
            {
                return NotFound(new { message = $"Class '{className}' not found in client.dll" });
            }
            return Ok(members);
        }

        // GET /api/engine
        [HttpGet("engine")]
        public IActionResult GetEngineModule()
        {
            var engineModule = _moduleRepository.GetModuleByName("engine.dll");
            if (engineModule == null)
            {
                return NotFound(new { message = "Module 'engine.dll' not found" });
            }
            return Ok(engineModule);
        }

        // GET /api/engine/sigs
        [HttpGet("engine/sigs")]
        public IActionResult GetEngineSignatures()
        {
            var engineModule = _moduleRepository.GetModuleByName("engine.dll");
            if (engineModule == null)
            {
                return NotFound(new { message = "Module 'engine.dll' not found" });
            }
            return Ok(engineModule.Signatures);
        }

        // GET /api/engine/sigs/{sigName}
        [HttpGet("engine/sigs/{sigName}")]
        public IActionResult GetEngineSignature(string sigName)
        {
            var engineModule = _moduleRepository.GetModuleByName("engine.dll");
            if (engineModule == null)
            {
                return NotFound(new { message = "Module 'engine.dll' not found" });
            }
            var sig = engineModule.Signatures.Find(s => s.Name == sigName);
            if (sig == null)
            {
                return NotFound(new { message = $"Signature '{sigName}' not found in engine.dll" });
            }
            return Ok(sig);
        }

        // GET /api/engine/members/{className}
        [HttpGet("engine/members/{className}")]
        public IActionResult GetEngineClassMembers(string className)
        {
            var engineModule = _moduleRepository.GetModuleByName("engine.dll");
            if (engineModule == null)
            {
                return NotFound(new { message = "Module 'engine.dll' not found" });
            }
            var members = engineModule.Members.FindAll(m => m.ClassName == className);
            if (members.Count == 0)
            {
                return NotFound(new { message = $"Class '{className}' not found in engine.dll" });
            }
            return Ok(members);
        }
    }
}
