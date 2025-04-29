using System;
using System.Collections.Generic;

namespace CrudGenerator.Models
{
    public class EntityGenerationRequest
    {
        /// <summary>
        /// Nome da entidade a ser criada
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Namespace base do projeto
        /// </summary>
        public string BaseNamespace { get; set; }

        /// <summary>
        /// Lista de propriedades da entidade
        /// </summary>
        public List<EntityProperty> Properties { get; set; } = new List<EntityProperty>();
    }

    public class EntityProperty
    {
        /// <summary>
        /// Nome da propriedade
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tipo da propriedade (string, int, Guid, etc.)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Indica se a propriedade é obrigatória
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Tamanho máximo para propriedades do tipo string
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Descrição da propriedade para documentação
        /// </summary>
        public string? Description { get; set; }
    }
}
