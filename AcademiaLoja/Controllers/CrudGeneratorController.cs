using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CrudGenerator.Models;
using CrudGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrudGeneratorController : ControllerBase
    {
        private readonly ICodeGeneratorService _codeGeneratorService;

        public CrudGeneratorController(ICodeGeneratorService codeGeneratorService)
        {
            _codeGeneratorService = codeGeneratorService;
        }

        /// <summary>
        /// Gera um CRUD completo para uma entidade
        /// </summary>
        /// <param name="request">Informações da entidade a ser criada</param>
        /// <returns>Resultado da geração com lista de arquivos gerados</returns>
        [HttpPost("generate")]
        [SwaggerOperation(
            Summary = "Gerar CRUD completo",
            Description = "Gera todos os arquivos necessários para um CRUD completo seguindo o padrão DDD com CQRS")]
        [SwaggerResponse(200, "Sucesso", typeof(EntityGenerationResponse))]
        public async Task<IActionResult> GenerateCrud([FromBody] EntityGenerationRequest request)
        {
            try
            {
                // Validar a requisição
                if (string.IsNullOrEmpty(request.EntityName))
                {
                    return BadRequest(new EntityGenerationResponse
                    {
                        Success = false,
                        Message = "O nome da entidade é obrigatório"
                    });
                }

                if (string.IsNullOrEmpty(request.BaseNamespace))
                {
                    return BadRequest(new EntityGenerationResponse
                    {
                        Success = false,
                        Message = "O namespace base é obrigatório"
                    });
                }

                if (request.Properties == null || request.Properties.Count == 0)
                {
                    return BadRequest(new EntityGenerationResponse
                    {
                        Success = false,
                        Message = "A entidade deve ter pelo menos uma propriedade"
                    });
                }

                // Gerar os arquivos
                var generatedFiles = await _codeGeneratorService.GenerateCrudAsync(request);

                // Criar a resposta
                var response = new EntityGenerationResponse
                {
                    Success = true,
                    Message = $"CRUD para a entidade {request.EntityName} gerado com sucesso",
                    GeneratedFiles = new List<GeneratedFileInfo>()
                };

                // Adicionar informações dos arquivos gerados
                foreach (var file in generatedFiles)
                {
                    response.GeneratedFiles.Add(new GeneratedFileInfo
                    {
                        FilePath = file.FilePath,
                        FileType = file.FileType
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityGenerationResponse
                {
                    Success = false,
                    Message = $"Erro ao gerar CRUD: {ex.Message}"
                });
            }
        }
    }
}
