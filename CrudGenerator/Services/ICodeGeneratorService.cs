using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CrudGenerator.Models;

namespace CrudGenerator.Services
{
    // Esta interface foi mantida para compatibilidade, mas agora é implementada apenas pelo CodeGeneratorFileService
    public interface ICodeGeneratorService
    {
        /// <summary>
        /// Gera todos os arquivos necessários para um CRUD completo
        /// </summary>
        /// <param name="request">Requisição com informações da entidade</param>
        /// <returns>Lista de arquivos gerados com seus caminhos</returns>
        Task<List<GeneratedFile>> GenerateCrudAsync(EntityGenerationRequest request);
    }

    public class GeneratedFile
    {
        /// <summary>
        /// Caminho relativo do arquivo gerado
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Conteúdo do arquivo gerado
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Tipo do arquivo gerado (Entidade, Repositório, etc.)
        /// </summary>
        public string FileType { get; set; }
    }
}
