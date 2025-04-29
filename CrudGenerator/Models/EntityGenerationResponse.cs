using System;
using System.Collections.Generic;

namespace CrudGenerator.Models
{
    public class EntityGenerationResponse
    {
        /// <summary>
        /// Indica se a geração foi bem-sucedida
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem de sucesso ou erro
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Lista de arquivos gerados
        /// </summary>
        public List<GeneratedFileInfo> GeneratedFiles { get; set; } = new List<GeneratedFileInfo>();
    }

    public class GeneratedFileInfo
    {
        /// <summary>
        /// Caminho relativo do arquivo gerado
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Tipo do arquivo gerado (Entidade, Repositório, etc.)
        /// </summary>
        public string FileType { get; set; }
    }
}
