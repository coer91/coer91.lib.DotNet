using coer91.NET.Files;  

namespace coer91.NET.ORM
{
    public abstract class ScaffoldWritter
    {
        protected string _rootPath = FilesManagement.GetSolutionPath();
        protected ScaffoldProfile _profile;
        protected string _database;
        protected string _dbSet;
        protected string _dto;
        protected string _class;
        protected string _variable;
        protected string _property;


        protected async Task WriteRepositoryInterface(string path)
        {
            string _interfaceNamespace = _profile.OutputFiles.IRepositoryOutput.Replace("/", ".");
            string _contextNamespace   = _profile.ContextNamespace;

            using StreamWriter streamWriter = File.CreateText(path);            
            streamWriter.WriteLine($"using {_contextNamespace};");
            streamWriter.WriteLine($"using System.Linq.Expressions;");
            streamWriter.WriteLine();
            streamWriter.WriteLine($"namespace {_interfaceNamespace}");
            streamWriter.WriteLine($"{{");
            streamWriter.WriteLine($"\tpublic interface I{_class}Repository");
            streamWriter.WriteLine($"\t{{");
            streamWriter.WriteLine($"\t\tTask<bool> Exists{_class}(Expression<Func<{_dbSet}, bool>> expression);");
            streamWriter.WriteLine($"\t\tTask<{_dbSet}> Get{_class}By(Expression<Func<{_dbSet}, bool>> expression);");
            streamWriter.WriteLine($"\t\tTask<List<{_dbSet}>> Get{_class}List(Expression<Func<{_dbSet}, bool>> expression);");
            streamWriter.WriteLine($"\t\tTask<{_dbSet}> Create{_class}({_dbSet} entity);");
            streamWriter.WriteLine($"\t\tTask<{_dbSet}> Update{_class}({_dbSet} entity);");
            streamWriter.WriteLine($"\t\tTask<int> Delete{_class}({_dbSet} entity);");
            streamWriter.WriteLine($"\t}}");
            streamWriter.Write($"}}");
            
            streamWriter.Close();
            await streamWriter.DisposeAsync();
        }


        protected async Task WriteRepository(string path)
        {
            string _namespace          = _profile.OutputFiles.RepositoryOutput.Replace("/", ".");
            string _interfaceNamespace = _profile.OutputFiles.IRepositoryOutput.Replace("/", ".");
            string _contextName        = _profile.ContextName; 
            string _contextNamespace   = _profile.ContextNamespace;

            using StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine($"using Microsoft.EntityFrameworkCore;");
            streamWriter.WriteLine($"using {_interfaceNamespace};");
            streamWriter.WriteLine($"using {_contextNamespace};");
            streamWriter.WriteLine($"using System.Linq.Expressions;");
            streamWriter.WriteLine();
            streamWriter.WriteLine($"namespace {_namespace}");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine($"\tpublic class {_class}Repository({_contextName} _context) : I{_class}Repository");
            streamWriter.WriteLine($"\t{{");
            streamWriter.WriteLine();

            //Exist
            streamWriter.WriteLine($"\t\tpublic async Task<bool> Exists{_class}(Expression<Func<{_dbSet}, bool>> expression)");
            streamWriter.WriteLine($"\t\t\t=> await _context.{_property}.AsNoTracking().AnyAsync(expression);"); 
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //GetBy
            streamWriter.WriteLine($"\t\tpublic async Task<{_dbSet}> Get{_class}By(Expression<Func<{_dbSet}, bool>> expression)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\treturn await _context.{_property}");
            streamWriter.WriteLine("\t\t\t\t.AsNoTracking()");
            streamWriter.WriteLine("\t\t\t\t.FirstOrDefaultAsync(expression);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //GetList
            streamWriter.WriteLine($"\t\tpublic async Task<List<{_dbSet}>> Get{_class}List(Expression<Func<{_dbSet}, bool>> expression)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\treturn await _context.{_property}");
            streamWriter.WriteLine("\t\t\t\t.Where(expression)");
            streamWriter.WriteLine("\t\t\t\t.AsNoTracking()");
            streamWriter.WriteLine("\t\t\t\t.ToListAsync();");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //Create
            streamWriter.WriteLine($"\t\tpublic async Task<{_dbSet}> Create{_class}({_dbSet} entity)");
            streamWriter.WriteLine($"\t\t{{");
            streamWriter.WriteLine($"\t\t\tawait _context.{_property}.AddAsync(entity);");
            streamWriter.WriteLine($"\t\t\tawait _context.SaveChangesAsync();");
            streamWriter.WriteLine($"\t\t\treturn entity;");
            streamWriter.WriteLine($"\t\t}}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //Update
            streamWriter.WriteLine($"\t\tpublic async Task<{_dbSet}> Update{_class}({_dbSet} entity)");
            streamWriter.WriteLine($"\t\t{{");
            streamWriter.WriteLine($"\t\t\t_context.{_property}.Update(entity);");
            streamWriter.WriteLine($"\t\t\tawait _context.SaveChangesAsync();");
            streamWriter.WriteLine($"\t\t\treturn entity;");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //Delete
            streamWriter.WriteLine($"\t\tpublic async Task<int> Delete{_class}({_dbSet} entity)");
            streamWriter.WriteLine($"\t\t{{");
            streamWriter.WriteLine($"\t\t\t_context.{_property}.Remove(entity);");
            streamWriter.WriteLine($"\t\t\tint rows = await _context.SaveChangesAsync();");
            streamWriter.WriteLine($"\t\t\t_context.ChangeTracker.Clear();");
            streamWriter.WriteLine($"\t\t\treturn rows;");
            streamWriter.WriteLine($"\t\t}}");

            //End Class
            streamWriter.WriteLine($"\t}}");
            streamWriter.Write($"}}");

            streamWriter.Close();
            await streamWriter.DisposeAsync();
        }


        protected async Task WriteDTO(string path)
        {
            string _contextPath = $"{_rootPath}/{_profile.Project}/{_profile.ContextOutput}";
            string _dtoNamespace = _profile.OutputFiles.DtoOutput.Replace("/", ".");

            using StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine($"namespace {_dtoNamespace}");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine($"\tpublic class {_dto}");
            streamWriter.WriteLine("\t{");

            //Get Properties
            string type;
            IEnumerable<string> lines;
            foreach (string modelFile in Directory.GetFiles(_contextPath))
            {
                lines = GetContentFile(modelFile.Replace("\\", "/")).Values;
                if (lines.Any(x => x.CleanUpBlanks().EndsWith($"class {_dbSet}") || x.CleanUpBlanks().EndsWith($"class {_dbSet} {{")))
                {
                    foreach (string line in lines.Where(x => x.Contains("public") && !x.Contains("class")).Select(x => x.CleanUpBlanks()))
                    {
                        type = line.Split(' ').Except(["public", "private", "protected", "internal", "override", "virtual"]).FirstOrDefault();

                        streamWriter.Write(
                            new[] { "int", "float", "double", "decimal", "long", "bool", "string", "byte[]", "DateTime", "DateOnly", "TimeSpan", "TimeOnly" }.Any(x => type.StartsWith(x))
                            ? "\t\t" : "\t\t//"
                        );

                        streamWriter.WriteLine(line + "\n");
                    }

                    break;
                }
            }

            streamWriter.WriteLine("\t}");
            streamWriter.Write("}");

            streamWriter.Close();
            await streamWriter.DisposeAsync();
        } 


        protected async Task WriteAutomapper(string path, bool actionMapper)
        {
            string _contextNamespace = _profile.ContextNamespace;
            string _dtoNamespace = _profile.OutputFiles.DtoOutput.Replace("/", ".");
            string _automapperNamespace = _profile.OutputFiles.AutoMapperOutput.Replace("/", ".");

            using StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine($"using {_contextNamespace};");
            streamWriter.WriteLine($"using {_dtoNamespace};");
            streamWriter.WriteLine($"using AutoMapper;");
            streamWriter.WriteLine();
            streamWriter.WriteLine($"namespace {_automapperNamespace}");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine($"\tpublic class {_class}Mapper : Profile");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine($"\t\tpublic {_class}Mapper()");
            streamWriter.WriteLine("\t\t{");

            streamWriter.Write($"\t\t\tCreateMap<{_dbSet}, {_dto}>()");

            if (actionMapper) 
            {  
                streamWriter.WriteLine($"\n\t\t\t\t.AfterMap<{_class}Action>();");
                streamWriter.WriteLine();
            }

            else streamWriter.WriteLine(";");

            streamWriter.Write($"\t\t\tCreateMap<{_dto}, {_dbSet}>()");

            if (actionMapper)
            { 
                streamWriter.WriteLine($"\n\t\t\t\t.AfterMap<{_class}Action>();");
                streamWriter.WriteLine("\t\t}");
            }

            else streamWriter.WriteLine(";");

            if (actionMapper)
            { 
                streamWriter.WriteLine();
                streamWriter.WriteLine();
                streamWriter.WriteLine($"\t\tprivate class {_class}Action :");
                streamWriter.WriteLine($"\t\t\tIMappingAction<{_dbSet}, {_dto}>,");
                streamWriter.WriteLine($"\t\t\tIMappingAction<{_dto}, {_dbSet}>");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tpublic void Process({_dbSet} source, {_dto} destination, ResolutionContext context) {{ }}");
                streamWriter.WriteLine();
                streamWriter.WriteLine();
                streamWriter.WriteLine($"\t\t\tpublic void Process({_dto} source, {_dbSet} destination, ResolutionContext context) {{ }}");
            }
             
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine("\t}");
            streamWriter.Write("}");

            streamWriter.Close();
            await streamWriter.DisposeAsync();
        }


        protected async Task WriteServiceInterface(string path)
        {
            string _serviceInterfaceNamespace = _profile.OutputFiles.IServiceOutput.Replace("/", ".");  
            string _dtoNamespace = _profile.OutputFiles.DtoOutput.Replace("/", ".");

            using StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine("using Microsoft.AspNetCore.JsonPatch;");
            streamWriter.WriteLine($"using {_dtoNamespace};");
            streamWriter.WriteLine("using coer91.NET;");

            streamWriter.WriteLine();
            streamWriter.WriteLine($"namespace {_serviceInterfaceNamespace}");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine($"\tpublic interface I{_class}Service");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine($"\t\tTask<ResponseDTO<{_dto}>> Get{_class}ById(int {_variable}Id);");
            streamWriter.WriteLine($"\t\tTask<ResponseList<{_dto}>> Get{_class}List();");
            streamWriter.WriteLine($"\t\tTask<ResponseDTO<{_dto}>> Create{_class}({_dto} {_dto.FirstCharToLower()});");
            streamWriter.WriteLine($"\t\tTask<ResponseDTO<{_dto}>> Update{_class}({_dto} {_dto.FirstCharToLower()});");
            streamWriter.WriteLine($"\t\tTask<ResponseDTO<{_dto}>> Patch{_class}(int {_variable}Id, JsonPatchDocument patch);");
            streamWriter.WriteLine($"\t\tTask<ResponseDTO<{_dto}>> Delete{_class}(int {_variable}Id);");
            streamWriter.WriteLine("\t}");
            streamWriter.Write("}");

            streamWriter.Close();
            await streamWriter.DisposeAsync();
        }


        protected async Task WriteService(string path, bool addLogic)
        {
            string _contextNamespace = _profile.ContextNamespace;
            string _repositoryInterfaceNamespace = _profile.OutputFiles.IRepositoryOutput.Replace("/", ".");
            string _serviceInterfaceNamespace = _profile.OutputFiles.IServiceOutput.Replace("/", ".");
            string _serviceNamespace = _profile.OutputFiles.ServiceOutput.Replace("/", "."); 
            string _dtoNamespace = _profile.OutputFiles.DtoOutput.Replace("/", ".");
            string dto = _dto.FirstCharToLower();

            using StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine($"using Microsoft.AspNetCore.JsonPatch;");
            streamWriter.WriteLine($"using {_repositoryInterfaceNamespace};");
            streamWriter.WriteLine($"using {_contextNamespace};");
            streamWriter.WriteLine($"using {_serviceInterfaceNamespace};");
            streamWriter.WriteLine($"using {_dtoNamespace};");
            streamWriter.WriteLine($"using AutoMapper;");
            streamWriter.WriteLine($"using coer91.NET;");
            streamWriter.WriteLine();
            streamWriter.WriteLine($"namespace {_serviceNamespace}");
            streamWriter.WriteLine($"{{");

            //Class
            streamWriter.WriteLine($"\tpublic class {_class}Service(");
            streamWriter.WriteLine($"\t\tI{_class}Repository _repository,");
            streamWriter.WriteLine($"\t\tIMapper _mapper");
            streamWriter.WriteLine($"\t) : I{_class}Service {{");
            streamWriter.WriteLine();  

            //GET ById 
            streamWriter.WriteLine($"\t\tpublic async Task<ResponseDTO<{_dto}>> Get{_class}ById(int {_variable}Id)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tResponseDTO<{_dto}> response = new();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\ttry");
            streamWriter.WriteLine("\t\t\t{");

            if (addLogic)
            {
                streamWriter.WriteLine($"\t\t\t\t{_dbSet} entity = await _repository.Get{_class}By(x => x.Id == {_variable}Id);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\tif (entity is null)");
                streamWriter.WriteLine("\t\t\t\t\treturn response.NotFound();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Response");
                streamWriter.WriteLine($"\t\t\t\tresponse.Data = _mapper.Map<{_dto}>(entity);");
            }

            else streamWriter.WriteLine();

            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tcatch (Exception ex)");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn response.Exception(ex);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn response;");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //GET 
            streamWriter.WriteLine($"\t\tpublic async Task<ResponseList<{_dto}>> Get{_class}List()");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tResponseList<{_dto}> response = new();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\ttry");
            streamWriter.WriteLine("\t\t\t{");

            if (addLogic)
            {
                streamWriter.WriteLine($"\t\t\t\tList<{_dbSet}> entities = await _repository.Get{_class}List(x => true);");
                streamWriter.WriteLine($"\t\t\t\tList<{_dto}> dtoList = _mapper.Map<List<{_dto}>>(entities);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Response");
                streamWriter.WriteLine("\t\t\t\tresponse.Data = dtoList;");
            }

            else streamWriter.WriteLine();

            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tcatch (Exception ex)");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn response.Exception(ex);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn response;");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //CREATE 
            streamWriter.WriteLine($"\t\tpublic async Task<ResponseDTO<{_dto}>> Create{_class}({_dto} {dto})");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tResponseDTO<{_dto}> response = new();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\ttry");
            streamWriter.WriteLine("\t\t\t{");

            if (addLogic)
            {
                streamWriter.WriteLine("\t\t\t\t//Clean Data");
                streamWriter.WriteLine($"\t\t\t\t{dto}.Name = {dto}.Name.CleanUpBlanks().FirstCharToUpper();");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"\t\t\t\tif (string.IsNullOrWhiteSpace({dto}.Name))");
                streamWriter.WriteLine("\t\t\t\t\treturn response.BadRequest();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Exists?");
                streamWriter.WriteLine($"\t\t\t\tif (await _repository.Exists{_class}(x => x.Name.ToUpper().Equals({dto}.Name.ToUpper())))");
                streamWriter.WriteLine($"\t\t\t\t\treturn response.Conflict($\"<b>{{{dto}.Name}}</b> already exists\");");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Mapping");
                streamWriter.WriteLine($"\t\t\t\t{_dbSet} entity = _mapper.Map<{_dbSet}>({dto});");
                streamWriter.WriteLine("\t\t\t\tentity.Id = 0;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Create");
                streamWriter.WriteLine("\t\t\t\tentity = Clean.NoNesting(entity);");
                streamWriter.WriteLine($"\t\t\t\tentity = await _repository.Create{_class}(entity);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Response");
                streamWriter.WriteLine($"\t\t\t\tresponse.Data = _mapper.Map<{_dto}>(entity);");
            }

            else streamWriter.WriteLine();

            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tcatch (Exception ex)");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn response.Exception(ex);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn response;");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //Update 
            streamWriter.WriteLine($"\t\tpublic async Task<ResponseDTO<{_dto}>> Update{_class}({_dto} {dto})");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tResponseDTO<{_dto}> response = new();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\ttry");
            streamWriter.WriteLine("\t\t\t{");

            if (addLogic)
            {
                streamWriter.WriteLine("\t\t\t\t//Clean Data");
                streamWriter.WriteLine($"\t\t\t\t{dto}.Name = {dto}.Name.CleanUpBlanks().FirstCharToUpper();");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"\t\t\t\tif (string.IsNullOrWhiteSpace({dto}.Name))");
                streamWriter.WriteLine("\t\t\t\t\treturn response.BadRequest();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Exists?");
                streamWriter.WriteLine($"\t\t\t\tif (await _repository.Exists{_class}(x => x.Id != {dto}.Id && x.Name.ToUpper().Equals({dto}.Name.ToUpper())))");
                streamWriter.WriteLine($"\t\t\t\t\treturn response.Conflict($\"<b>{{{dto}.Name}}</b> already exists\"); ");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Get");
                streamWriter.WriteLine($"\t\t\t\t{_dbSet} entity = await _repository.Get{_class}By(x => x.Id == {dto}.Id);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\tif (entity is null)");
                streamWriter.WriteLine("\t\t\t\t\treturn response.NotFound();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Mapping");
                streamWriter.WriteLine($"\t\t\t\tentity = _mapper.Map<{_dbSet}>({dto});");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Update");
                streamWriter.WriteLine("\t\t\t\tentity = Clean.NoNesting(entity);");
                streamWriter.WriteLine($"\t\t\t\tentity = await _repository.Update{_class}(entity);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Response");
                streamWriter.WriteLine($"\t\t\t\tresponse.Data = _mapper.Map<{_dto}>(entity);");
            }

            else streamWriter.WriteLine();

            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tcatch (Exception ex)");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn response.Exception(ex);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn response;");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //PATCH 
            streamWriter.WriteLine($"\t\tpublic async Task<ResponseDTO<{_dto}>> Patch{_class}(int {_variable}Id, JsonPatchDocument patch)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tResponseDTO<{_dto}> response = new();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\ttry");
            streamWriter.WriteLine("\t\t\t{");

            if (addLogic)
            {
                streamWriter.WriteLine("\t\t\t\t//Get");
                streamWriter.WriteLine($"\t\t\t\t{_dbSet} entity = await _repository.Get{_class}By(x => x.Id == {_variable}Id);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\tif (entity is null)");
                streamWriter.WriteLine("\t\t\t\t\treturn response.NotFound();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Mapping");
                streamWriter.WriteLine("\t\t\t\tpatch.ApplyTo(entity);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Clean Data");
                streamWriter.WriteLine("\t\t\t\tentity.Name = entity.Name.CleanUpBlanks().FirstCharToUpper();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\tif (string.IsNullOrWhiteSpace(entity.Name))");
                streamWriter.WriteLine("\t\t\t\t\treturn response.BadRequest();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Exists?");
                streamWriter.WriteLine($"\t\t\t\tif (await _repository.Exists{_class}(x => x.Id != entity.Id && x.Name.ToUpper().Equals(entity.Name.ToUpper())))");
                streamWriter.WriteLine("\t\t\t\t\treturn response.Conflict($\"<b>{entity.Name}</b> already exists\"); ");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Update");
                streamWriter.WriteLine("\t\t\t\tentity = Clean.NoNesting(entity);");
                streamWriter.WriteLine($"\t\t\t\tentity = await _repository.Update{_class}(entity);");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"\t\t\t\t//Response");
                streamWriter.WriteLine($"\t\t\t\tresponse.Data = _mapper.Map<{_dto}>(entity);");
            }

            else streamWriter.WriteLine();

            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tcatch (Exception ex)");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn response.Exception(ex);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn response;");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //DELETE
            streamWriter.WriteLine($"\t\tpublic async Task<ResponseDTO<{_dto}>> Delete{_class}(int {_variable}Id)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tResponseDTO<{_dto}> response = new();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\ttry");
            streamWriter.WriteLine("\t\t\t{");

            if (addLogic)
            {
                streamWriter.WriteLine("\t\t\t\t//Get");
                streamWriter.WriteLine($"\t\t\t\t{_dbSet} entity = await _repository.Get{_class}By(x => x.Id == {_variable}Id);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\tif (entity is null)");
                streamWriter.WriteLine("\t\t\t\t\treturn response.NotFound();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Delete");
                streamWriter.WriteLine("\t\t\t\tentity = Clean.NoNesting(entity);");
                streamWriter.WriteLine($"\t\t\t\tawait _repository.Delete{_class}(entity);");
            }

            else streamWriter.WriteLine();

            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tcatch (Exception ex)");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\treturn response.Exception(ex);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn response;");
            streamWriter.WriteLine("\t\t}");

            //End Class
            streamWriter.WriteLine("\t}");
            streamWriter.Write("}");

            streamWriter.Close();
            await streamWriter.DisposeAsync();
        }


        protected async Task WriteController(string path)
        {
            string _controllerNamespace = _profile.OutputFiles.ControllerOutput.Replace("/", ".");
            string _serviceInterfaceNamespace = _profile.OutputFiles.IServiceOutput.Replace("/", ".");
            string _dtoNamespace = _profile.OutputFiles.DtoOutput.Replace("/", ".");

            string dto = _dto.FirstCharToLower();

            using StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine("using Microsoft.AspNetCore.JsonPatch;");
            streamWriter.WriteLine("using Microsoft.AspNetCore.Mvc;");
            streamWriter.WriteLine($"using {_serviceInterfaceNamespace};");
            streamWriter.WriteLine($"using {_dtoNamespace};");
            streamWriter.WriteLine();
            streamWriter.WriteLine($"namespace {_controllerNamespace}");
            streamWriter.WriteLine("{");

            //Class 
            streamWriter.WriteLine("\t[ApiController]");
            streamWriter.WriteLine($"\t[Route(\"api/{_class}\")]");
            streamWriter.WriteLine($"\tpublic class {_class}Controller(I{_class}Service _service) : ControllerBase");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine();

            //GET ById
            streamWriter.WriteLine("\t\t[HttpGet]");
            streamWriter.WriteLine($"\t\t[Route(\"Get{_class}ById/{{{_variable}Id}}\")]");
            streamWriter.WriteLine($"\t\tpublic async Task<ActionResult> Get{_class}ById([FromRoute] int {_variable}Id)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tvar response = await _service.Get{_class}ById({_variable}Id);");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tif (response.Failure)");
            streamWriter.WriteLine("\t\t\t\treturn StatusCode(response.HttpCode, response.MessageList.FirstOrDefault());");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn Ok(response.Data);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //GET
            streamWriter.WriteLine("\t\t[HttpGet]");
            streamWriter.WriteLine($"\t\t[Route(\"Get{_class}List\")]");
            streamWriter.WriteLine($"\t\tpublic async Task<ActionResult> Get{_class}List()");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tvar response = await _service.Get{_class}List();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tif (response.Failure)");
            streamWriter.WriteLine("\t\t\t\treturn StatusCode(response.HttpCode, response.MessageList.FirstOrDefault());");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn Ok(response.Data);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //POST
            streamWriter.WriteLine("\t\t[HttpPost]");
            streamWriter.WriteLine($"\t\t[Route(\"Create{_class}\")]");
            streamWriter.WriteLine($"\t\tpublic async Task<ActionResult> Create{_class}([FromBody] {_dto} {dto})");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tvar response = await _service.Create{_class}({dto});");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tif (response.Failure)");
            streamWriter.WriteLine("\t\t\t\treturn StatusCode(response.HttpCode, response.MessageList.FirstOrDefault());");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn StatusCode(201, response.Data);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //PUT
            streamWriter.WriteLine("\t\t[HttpPut]");
            streamWriter.WriteLine($"\t\t[Route(\"Update{_class}\")]");
            streamWriter.WriteLine($"\t\tpublic async Task<ActionResult> Update{_class}([FromBody] {_dto} {dto})");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tvar response = await _service.Update{_class}({dto});");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tif (response.Failure)");
            streamWriter.WriteLine("\t\t\t\treturn StatusCode(response.HttpCode, response.MessageList.FirstOrDefault());");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn Ok(response.Data);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //PATCH
            streamWriter.WriteLine("\t\t[HttpPatch]");
            streamWriter.WriteLine($"\t\t[Route(\"Patch{_class}/{{{_variable}Id}}\")]");
            streamWriter.WriteLine($"\t\tpublic async Task<ActionResult> Patch{_class}([FromRoute] int {_variable}Id, [FromBody] JsonPatchDocument patch)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tvar response = await _service.Patch{_class}({_variable}Id, patch);");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tif (response.Failure)");
            streamWriter.WriteLine("\t\t\t\treturn StatusCode(response.HttpCode, response.MessageList.FirstOrDefault());");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn Ok(response.Data);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //DELETE
            streamWriter.WriteLine("\t\t[HttpDelete]");
            streamWriter.WriteLine($"\t\t[Route(\"Delete{_class}/{{{_variable}Id}}\")]");
            streamWriter.WriteLine($"\t\tpublic async Task<ActionResult> Delete{_class}([FromRoute] int {_variable}Id)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine($"\t\t\tvar response = await _service.Delete{_class}({_variable}Id);");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\tif (response.Failure)");
            streamWriter.WriteLine("\t\t\t\treturn StatusCode(response.HttpCode, response.MessageList.FirstOrDefault());");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn NoContent();");
            streamWriter.WriteLine("\t\t}");

            //End Class
            streamWriter.WriteLine("\t}");
            streamWriter.Write("}");

            streamWriter.Close();
            await streamWriter.DisposeAsync();
        }


        protected async Task WriteTests(string path)
        {
            string _testNamespace = _profile.OutputFiles.TestOutput.Replace("/", ".");

            using StreamWriter streamWriter = File.CreateText(path); 
            streamWriter.WriteLine($"using Setup;");
            streamWriter.WriteLine($"using Xunit;");
            streamWriter.WriteLine();
            streamWriter.WriteLine($"namespace {_testNamespace}");
            streamWriter.WriteLine($"{{");

            //Class
            streamWriter.WriteLine($"\tpublic class {_class}Test : {_class}Injection");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine();

            //GET ById 
            streamWriter.WriteLine("\t\t[Fact]");
            streamWriter.WriteLine($"\t\tpublic void Get{_class}ById()");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tAssert.True(true);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //GET 
            streamWriter.WriteLine("\t\t[Fact]");
            streamWriter.WriteLine($"\t\tpublic void Get{_class}List()");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tAssert.True(true);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //CREATE 
            streamWriter.WriteLine("\t\t[Fact]");
            streamWriter.WriteLine($"\t\tpublic void Create{_class}()");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tAssert.True(true);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //Update 
            streamWriter.WriteLine("\t\t[Fact]");
            streamWriter.WriteLine($"\t\tpublic void Update{_class}()");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tAssert.True(true);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //PATCH 
            streamWriter.WriteLine("\t\t[Fact]");
            streamWriter.WriteLine($"\t\tpublic void Patch{_class}()");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tAssert.True(true);");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            //DELETE
            streamWriter.WriteLine("\t\t[Fact]");
            streamWriter.WriteLine($"\t\tpublic void Delete{_class}()");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tAssert.True(true);");
            streamWriter.WriteLine("\t\t}");

            //End Class
            streamWriter.WriteLine("\t}");
            streamWriter.Write("}");


            streamWriter.Close();
            await streamWriter.DisposeAsync();
        }


        protected async Task WriteTestInjection(string path)
        {
            string _repositoryInterfaceNamespace = _profile.OutputFiles.IRepositoryOutput.Replace("/", ".");
            string _repositoryNamespace = _profile.OutputFiles.RepositoryOutput.Replace("/", ".");
            string _serviceInterfaceNamespace = _profile.OutputFiles.IServiceOutput.Replace("/", ".");
            string _serviceNamespace = _profile.OutputFiles.ServiceOutput.Replace("/", ".");

            using StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine($"using {_repositoryInterfaceNamespace};"); 
            streamWriter.WriteLine($"using {_repositoryNamespace};");
            streamWriter.WriteLine($"using {_serviceInterfaceNamespace};");
            streamWriter.WriteLine($"using {_serviceNamespace};");
            streamWriter.WriteLine(); 
            streamWriter.WriteLine($"namespace Setup");
            streamWriter.WriteLine($"{{");

            //Class
            streamWriter.WriteLine($"\tpublic abstract class {_class}Injection : DBConnectionTests");
            streamWriter.WriteLine($"\t{{"); 
            streamWriter.WriteLine($"\t\tprotected readonly I{_class}Repository _{_variable}Repository;");
            streamWriter.WriteLine($"\t\tprotected readonly I{_class}Service _{_variable}Service;");
            streamWriter.WriteLine();

            streamWriter.WriteLine($"\t\tpublic {_class}Injection()");
            streamWriter.WriteLine($"\t\t{{");
            streamWriter.WriteLine($"\t\t\t_{_variable}Repository = new {_class}Repository(_{_profile.ContextName.FirstCharToLower()});");
            streamWriter.WriteLine($"\t\t\t_{_variable}Service    = new {_class}Service(_{_variable}Repository, _mapper);");  
            streamWriter.WriteLine($"\t\t}}"); 
                        
            //End Class
            streamWriter.WriteLine("\t}");
            streamWriter.Write("}");


            streamWriter.Close();
            await streamWriter.DisposeAsync();
        }


        protected static Dictionary<double, string> GetContentFile(string path)
        {
            int row = 0;
            string line;
            Dictionary<double, string> dictionary = [];

            //Read Collections
            using StreamReader streamReader = new(path);

            do
            {
                line = streamReader.ReadLine();
                if (line is null) break;
                else dictionary.Add(double.Parse($"{row++}"), line);
            } while (line is not null);

            streamReader.Close();
            streamReader.Dispose();

            return dictionary;
        }
    }
} 