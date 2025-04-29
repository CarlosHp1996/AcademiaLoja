using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CrudGenerator.Models;

namespace CrudGenerator.Services
{
    public class TemplateService : ITemplateService
    {
        public Task<string> RenderEntityTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Domain.Entities");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {request.EntityName}");
            sb.AppendLine("    {");
            sb.AppendLine("        public Guid Id { get; set; }");
            
            foreach (var property in request.Properties)
            {
                if (!string.IsNullOrEmpty(property.Description))
                {
                    sb.AppendLine($"        /// <summary>");
                    sb.AppendLine($"        /// {property.Description}");
                    sb.AppendLine($"        /// </summary>");
                }
                sb.AppendLine($"        public {property.Type} {property.Name} {{ get; set; }}");
            }
            
            sb.AppendLine();
            //sb.AppendLine("        // Navegação");
            //sb.AppendLine($"        public virtual ICollection<Product{request.EntityName}> Product{request.EntityName}s {{ get; private set; }} = new List<Product{request.EntityName}>();");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderRepositoryInterfaceTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Entities;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Interfaces");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface I{request.EntityName}Repository : IBaseRepository<{request.EntityName}>");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderRepositoryImplementationTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Interfaces;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Entities;");
            sb.AppendLine($"using {request.BaseNamespace}.Infra.Data;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Infra.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {request.EntityName}Repository : BaseRepository<{request.EntityName}>, I{request.EntityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly AppDbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"        public {request.EntityName}Repository(AppDbContext dbContext) : base(dbContext)");
            sb.AppendLine("        {");
            sb.AppendLine("            _context = dbContext;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderCreateCommandTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Requests.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Commands.{request.EntityName}s");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Create{request.EntityName}Command : IRequest<Result<{request.EntityName}Response>>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public Create{request.EntityName}Request Request;");
            sb.AppendLine($"        public Create{request.EntityName}Command(Create{request.EntityName}Request request)");
            sb.AppendLine("        {");
            sb.AppendLine("            Request = request;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderUpdateCommandTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Requests.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Commands.{request.EntityName}s");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Update{request.EntityName}Command : IRequest<Result<{request.EntityName}Response>>");
            sb.AppendLine("    {");
            sb.AppendLine("        public Guid Id { get; set; }");
            sb.AppendLine($"        public Update{request.EntityName}Request Request;");
            sb.AppendLine($"        public Update{request.EntityName}Command(Guid id, Update{request.EntityName}Request request)");
            sb.AppendLine("        {");
            sb.AppendLine("            Id = id;");
            sb.AppendLine("            Request = request;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderDeleteCommandTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Commands.{request.EntityName}s");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Delete{request.EntityName}Command : IRequest<Result<{request.EntityName}Response>>");
            sb.AppendLine("    {");
            sb.AppendLine("        public Guid Id { get; set; }");
            sb.AppendLine($"        public Delete{request.EntityName}Command(Guid id)");
            sb.AppendLine("        {");
            sb.AppendLine("            Id = id;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderGetQueryTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Filters;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Queries.{request.EntityName}s");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Get{request.EntityName}sQuery : IRequest<Result<IEnumerable<{request.EntityName}Response>>>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public Get{request.EntityName}sRequestFilter Filter {{ get; set; }}");
            sb.AppendLine();
            sb.AppendLine($"        public Get{request.EntityName}sQuery(Get{request.EntityName}sRequestFilter filter)");
            sb.AppendLine("        {");
            sb.AppendLine("            Filter = filter;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderGetByIdQueryTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Queries.{request.EntityName}s");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Get{request.EntityName}ByIdQuery : IRequest<Result<{request.EntityName}Response>>");
            sb.AppendLine("    {");
            sb.AppendLine("        public Guid Id { get; set; }");
            sb.AppendLine();
            sb.AppendLine($"        public Get{request.EntityName}ByIdQuery(Guid id)");
            sb.AppendLine("        {");
            sb.AppendLine("            Id = id;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderCreateCommandHandlerTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Interfaces;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Entities;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Commands.{request.EntityName}s.Handlers");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Create{request.EntityName}CommandHandler : IRequestHandler<Create{request.EntityName}Command, Result<{request.EntityName}Response>>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{request.EntityName}Repository _{request.EntityName.ToLower()}Repository;");
            sb.AppendLine($"        public Create{request.EntityName}CommandHandler(I{request.EntityName}Repository {request.EntityName.ToLower()}Repository)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _{request.EntityName.ToLower()}Repository = {request.EntityName.ToLower()}Repository;");
            sb.AppendLine("        }");
            sb.AppendLine($"        public async Task<Result<{request.EntityName}Response>> Handle(Create{request.EntityName}Command request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = new Result<{request.EntityName}Response>();");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine($"                var {request.EntityName.ToLower()} = new {request.EntityName}()");
            sb.AppendLine("                {");
            
            foreach (var property in request.Properties)
            {
                sb.AppendLine($"                    {property.Name} = request.Request.{property.Name},");
            }
            
            sb.AppendLine("                };");
            sb.AppendLine();
            sb.AppendLine($"                _ = await _{request.EntityName.ToLower()}Repository.AddAsync({request.EntityName.ToLower()});");
            sb.AppendLine();
            sb.AppendLine($"                var response = new {request.EntityName}Response");
            sb.AppendLine("                {");
            sb.AppendLine($"                    Id = {request.EntityName.ToLower()}.Id,");
            
            foreach (var property in request.Properties)
            {
                sb.AppendLine($"                    {property.Name} = {request.EntityName.ToLower()}.{property.Name},");
            }
            
            sb.AppendLine("                };");
            sb.AppendLine();
            sb.AppendLine("                result.Value = response;");
            sb.AppendLine("                result.Count = 1;");
            sb.AppendLine("                result.HasSuccess = true;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception)");
            sb.AppendLine("            {");
            sb.AppendLine("                throw;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return result;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderUpdateCommandHandlerTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Interfaces;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Commands.{request.EntityName}s.Handlers");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Update{request.EntityName}CommandHandler : IRequestHandler<Update{request.EntityName}Command, Result<{request.EntityName}Response>>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{request.EntityName}Repository _{request.EntityName.ToLower()}Repository;");
            sb.AppendLine($"        public Update{request.EntityName}CommandHandler(I{request.EntityName}Repository {request.EntityName.ToLower()}Repository)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _{request.EntityName.ToLower()}Repository = {request.EntityName.ToLower()}Repository;");
            sb.AppendLine("        }");
            sb.AppendLine($"        public async Task<Result<{request.EntityName}Response>> Handle(Update{request.EntityName}Command request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = new Result<{request.EntityName}Response>();");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine($"                var {request.EntityName.ToLower()} = await _{request.EntityName.ToLower()}Repository.GetById(request.Id);");
            sb.AppendLine();
            sb.AppendLine($"                if ({request.EntityName.ToLower()} == null)");
            sb.AppendLine("                {");
            sb.AppendLine("                    result.HasSuccess = false;");
            sb.AppendLine($"                    result.Message = \"{request.EntityName} não encontrado.\";");
            sb.AppendLine("                    return result;");
            sb.AppendLine("                }");
            sb.AppendLine();
            
            foreach (var property in request.Properties)
            {
                sb.AppendLine($"                {request.EntityName.ToLower()}.{property.Name} = request.Request.{property.Name};");
            }
            
            sb.AppendLine();
            sb.AppendLine($"                _ = await _{request.EntityName.ToLower()}Repository.UpdateAsync({request.EntityName.ToLower()});");
            sb.AppendLine();
            sb.AppendLine($"                var response = new {request.EntityName}Response");
            sb.AppendLine("                {");
            sb.AppendLine($"                    Id = {request.EntityName.ToLower()}.Id,");
            
            foreach (var property in request.Properties)
            {
                sb.AppendLine($"                    {property.Name} = {request.EntityName.ToLower()}.{property.Name},");
            }
            
            sb.AppendLine("                };");
            sb.AppendLine();
            sb.AppendLine("                result.Value = response;");
            sb.AppendLine("                result.Count = 1;");
            sb.AppendLine("                result.HasSuccess = true;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception)");
            sb.AppendLine("            {");
            sb.AppendLine("                throw;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return result;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderDeleteCommandHandlerTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Interfaces;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Commands.{request.EntityName}s.Handlers");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Delete{request.EntityName}CommandHandler : IRequestHandler<Delete{request.EntityName}Command, Result<{request.EntityName}Response>>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{request.EntityName}Repository _{request.EntityName.ToLower()}Repository;");
            sb.AppendLine($"        public Delete{request.EntityName}CommandHandler(I{request.EntityName}Repository {request.EntityName.ToLower()}Repository)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _{request.EntityName.ToLower()}Repository = {request.EntityName.ToLower()}Repository;");
            sb.AppendLine("        }");
            sb.AppendLine($"        public async Task<Result<{request.EntityName}Response>> Handle(Delete{request.EntityName}Command request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = new Result<{request.EntityName}Response>();");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine($"                var {request.EntityName.ToLower()} = await _{request.EntityName.ToLower()}Repository.GetById(request.Id);");
            sb.AppendLine();
            sb.AppendLine($"                if ({request.EntityName.ToLower()} == null)");
            sb.AppendLine("                {");
            sb.AppendLine("                    result.HasSuccess = false;");
            sb.AppendLine($"                    result.Message = \"{request.EntityName} não encontrado.\";");
            sb.AppendLine("                    return result;");
            sb.AppendLine("                }");
            sb.AppendLine();
            sb.AppendLine($"                _ = await _{request.EntityName.ToLower()}Repository.DeleteAsync({request.EntityName.ToLower()});");
            sb.AppendLine();
            sb.AppendLine($"                var response = new {request.EntityName}Response");
            sb.AppendLine("                {");
            sb.AppendLine($"                    Id = {request.EntityName.ToLower()}.Id,");
            
            foreach (var property in request.Properties)
            {
                sb.AppendLine($"                    {property.Name} = {request.EntityName.ToLower()}.{property.Name},");
            }
            
            sb.AppendLine("                };");
            sb.AppendLine();
            sb.AppendLine("                result.Value = response;");
            sb.AppendLine("                result.Count = 1;");
            sb.AppendLine("                result.HasSuccess = true;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception)");
            sb.AppendLine("            {");
            sb.AppendLine("                throw;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return result;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderGetQueryHandlerTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Interfaces;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Queries.{request.EntityName}s.Handlers");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Get{request.EntityName}sQueryHandler : IRequestHandler<Get{request.EntityName}sQuery, Result<IEnumerable<{request.EntityName}Response>>>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{request.EntityName}Repository _{request.EntityName.ToLower()}Repository;");
            sb.AppendLine($"        public Get{request.EntityName}sQueryHandler(I{request.EntityName}Repository {request.EntityName.ToLower()}Repository)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _{request.EntityName.ToLower()}Repository = {request.EntityName.ToLower()}Repository;");
            sb.AppendLine("        }");
            sb.AppendLine($"        public async Task<Result<IEnumerable<{request.EntityName}Response>>> Handle(Get{request.EntityName}sQuery query, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = new Result<IEnumerable<{request.EntityName}Response>>();");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine($"                var {request.EntityName.ToLower()}s = await _{request.EntityName.ToLower()}Repository.GetAll(query.Filter.Take, query.Filter.Offset, query.Filter.SortingProp, query.Filter.Ascending);");
            sb.AppendLine();
            sb.AppendLine("                // Aplicar filtros se necessário");
            sb.AppendLine($"                if (!{request.EntityName.ToLower()}s.Result(out var count).Any())");
            sb.AppendLine("                {");
            sb.AppendLine($"                    result.WithError(\"Nenhum item encontrado.\");");
            sb.AppendLine($"                    return result;");
            sb.AppendLine("                }");
            sb.AppendLine();
            sb.AppendLine("                // Paginação");
            sb.AppendLine($"                var pagedItems = {request.EntityName.ToLower()}s.Result(out count);");
            sb.AppendLine();
            sb.AppendLine($"                var response = new List<{request.EntityName}Response>();");
            sb.AppendLine();
            sb.AppendLine($"                foreach (var {request.EntityName.ToLower()} in pagedItems)");
            sb.AppendLine("                {");
            sb.AppendLine($"                var responseItem = new {request.EntityName}Response");
            sb.AppendLine("                {");
            sb.AppendLine($"                    Id = {request.EntityName.ToLower()}.Id,");
            foreach (var property in request.Properties)
            {
            sb.AppendLine($"                    {property.Name} = {request.EntityName.ToLower()}.{property.Name},");
            }
            sb.AppendLine("                };");
            //sb.AppendLine($"                    {request.pro .Type} {property.Name} = {request.EntityName.ToLower()}.Id,");
            //sb.AppendLine($"                    Name = {request.EntityName.ToLower()}.Name,");
            //sb.AppendLine($"                    Description = {request.EntityName.ToLower()}.Description");
            sb.AppendLine("                response.Add(responseItem);");

            //ajustar foreach para pegar todos os campos inseridos na request
            //foreach (var property in request.Properties)
            //{
            //    sb.AppendLine($"                        {property.Name} = {request.EntityName.ToLower()}.{property.Name},");              
            //}
            sb.AppendLine("                }");
            sb.AppendLine();
            sb.AppendLine("                result.Value = response;");
            sb.AppendLine($"                result.Count = 1;");
            sb.AppendLine("                result.HasSuccess = true;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception)");
            sb.AppendLine("            {");
            sb.AppendLine("                throw;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return result;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderGetByIdQueryHandlerTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Interfaces;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Queries.{request.EntityName}s.Handlers");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Get{request.EntityName}ByIdQueryHandler : IRequestHandler<Get{request.EntityName}ByIdQuery, Result<{request.EntityName}Response>>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{request.EntityName}Repository _{request.EntityName.ToLower()}Repository;");
            sb.AppendLine($"        public Get{request.EntityName}ByIdQueryHandler(I{request.EntityName}Repository {request.EntityName.ToLower()}Repository)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _{request.EntityName.ToLower()}Repository = {request.EntityName.ToLower()}Repository;");
            sb.AppendLine("        }");
            sb.AppendLine($"        public async Task<Result<{request.EntityName}Response>> Handle(Get{request.EntityName}ByIdQuery request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = new Result<{request.EntityName}Response>();");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine($"                var {request.EntityName.ToLower()} = await _{request.EntityName.ToLower()}Repository.GetById(request.Id);");
            sb.AppendLine();
            sb.AppendLine($"                if ({request.EntityName.ToLower()} == null)");
            sb.AppendLine("                {");
            sb.AppendLine("                    result.HasSuccess = false;");
            sb.AppendLine($"                    result.Message = \"{request.EntityName} não encontrado.\";");
            sb.AppendLine("                    return result;");
            sb.AppendLine("                }");
            sb.AppendLine();
            sb.AppendLine($"                var response = new {request.EntityName}Response");
            sb.AppendLine("                {");
            sb.AppendLine($"                    Id = {request.EntityName.ToLower()}.Id,");
            
            foreach (var property in request.Properties)
            {
                sb.AppendLine($"                    {property.Name} = {request.EntityName.ToLower()}.{property.Name},");
            }
            
            sb.AppendLine("                };");
            sb.AppendLine();
            sb.AppendLine("                result.Value = response;");
            sb.AppendLine("                result.Count = 1;");
            sb.AppendLine("                result.HasSuccess = true;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception)");
            sb.AppendLine("            {");
            sb.AppendLine("                throw;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return result;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderControllerTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Application.Commands.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Filters;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Requests.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Application.Queries.{request.EntityName}s;");
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Helpers;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Swashbuckle.AspNetCore.Annotations;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Web.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("    [ApiController]");
            sb.AppendLine("    [Route(\"api/[controller]\")]");
            sb.AppendLine($"    public class {request.EntityName}Controller : ControllerBase");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly IMediator _mediator;");
            sb.AppendLine($"        public {request.EntityName}Controller(IMediator mediator)");
            sb.AppendLine("        {");
            sb.AppendLine("            _mediator = mediator;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [SwaggerOperation(");
            sb.AppendLine($"             Summary = \"Create {request.EntityName.ToLower()}s\",");
            sb.AppendLine("             Description = \"All fields are required.\")]");
            sb.AppendLine($"        [SwaggerResponse(200, \"Success\", typeof(Result<{request.EntityName}Response>))]");
            sb.AppendLine("        [HttpPost(\"create\")]");
            sb.AppendLine($"        public async Task<IActionResult> Create([FromBody] Create{request.EntityName}Request request)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var command = new Create{request.EntityName}Command(request);");
            sb.AppendLine("            var response = await _mediator.Send(command);");
            sb.AppendLine();
            sb.AppendLine("            return Ok(response);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [SwaggerOperation(");
            sb.AppendLine($"         Summary = \"Update {request.EntityName}s\",");
            sb.AppendLine($"         Description = \"{request.EntityName} 'Id' is required.\")]");
            sb.AppendLine($"        [SwaggerResponse(200, \"Success\", typeof(Result<{request.EntityName}Response>))]");
            sb.AppendLine("        [HttpPut(\"{id}\")]");
            sb.AppendLine("        [AllowAnonymous]");
            sb.AppendLine($"        public async Task<IActionResult> Update(Guid id, [FromBody] Update{request.EntityName}Request request)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var command = new Update{request.EntityName}Command(id, request);");
            sb.AppendLine("            var response = await _mediator.Send(command);");
            sb.AppendLine();
            sb.AppendLine("            return Ok(response);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [SwaggerOperation(");
            sb.AppendLine($"             Summary = \"List all {request.EntityName}s\",");
            sb.AppendLine($"             Description = \"List all {request.EntityName}s in a paginated manner.\")]");
            sb.AppendLine($"        [SwaggerResponse(200, \"Sucesso\", typeof(Result<{request.EntityName}Response>))]");
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine("        [AllowAnonymous]");
            sb.AppendLine($"        public async Task<IActionResult> Get([FromQuery] Get{request.EntityName}sRequestFilter filter)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var command = new Get{request.EntityName}sQuery(filter);");
            sb.AppendLine("            var response = await _mediator.Send(command);");
            sb.AppendLine();
            sb.AppendLine("            return Ok(response);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [SwaggerOperation(");
            sb.AppendLine($"            Summary = \"List all {request.EntityName}s according to id\",");
            sb.AppendLine($"            Description = \"The {request.EntityName} 'Id' is mandatory.\")]");
            sb.AppendLine($"        [SwaggerResponse(200, \"Success\", typeof(Result<{request.EntityName}Response>))]");
            sb.AppendLine("        [HttpGet(\"{id}\")]");
            sb.AppendLine("        [AllowAnonymous]");
            sb.AppendLine("        public async Task<IActionResult> GetById(Guid id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var command = new Get{request.EntityName}ByIdQuery(id);");
            sb.AppendLine("            var response = await _mediator.Send(command);");
            sb.AppendLine();
            sb.AppendLine("            return Ok(response);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [SwaggerOperation(");
            sb.AppendLine($"            Summary = \"Delete {request.EntityName}\",");
            sb.AppendLine($"            Description = \"{request.EntityName} 'Id' is required.\")]");
            sb.AppendLine($"        [SwaggerResponse(200, \"Success\", typeof(Result<{request.EntityName}Response>))]");
            sb.AppendLine("        [HttpDelete(\"{id}\")]");
            sb.AppendLine("        [AllowAnonymous]");
            sb.AppendLine("        public async Task<IActionResult> Delete(Guid id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var command = new Delete{request.EntityName}Command(id);");
            sb.AppendLine("            var response = await _mediator.Send(command);");
            sb.AppendLine();
            sb.AppendLine("            return Ok(response);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> RenderDbContextConfigurationTemplateAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"using {request.BaseNamespace}.Domain.Entities;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Infra.Data.Configuration");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {request.EntityName}Configuration : IEntityTypeConfiguration<{request.EntityName}>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public void Configure(EntityTypeBuilder<{request.EntityName}> builder)");
            sb.AppendLine("        {");
            sb.AppendLine($"            builder.ToTable(\"{request.EntityName}s\");");
            sb.AppendLine("            builder.HasKey(e => e.Id);");
            sb.AppendLine();
            
            foreach (var property in request.Properties)
            {
                sb.AppendLine($"            builder.Property(e => e.{property.Name})");
                
                if (property.IsRequired)
                {
                    sb.AppendLine("                .IsRequired()");
                }
                
                if (property.Type.ToLower() == "string" && property.MaxLength.HasValue)
                {
                    sb.AppendLine($"                .HasMaxLength({property.MaxLength.Value})");
                }
                
                sb.AppendLine("                ;");
                sb.AppendLine();
            }
            
            //sb.AppendLine("            // Relacionamentos");
            //sb.AppendLine($"            builder.HasMany(e => e.Product{request.EntityName}s)");
            //sb.AppendLine($"                .WithOne(e => e.{request.EntityName})");
            //sb.AppendLine($"                .HasForeignKey(e => e.{request.EntityName}Id);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return Task.FromResult(sb.ToString());
        }
    }
}
