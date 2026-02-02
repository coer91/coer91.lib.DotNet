namespace coer91
{
    public abstract class ScaffoldBuilder
    {
        protected string _project;
        protected string _contextPath;
        protected string _contextName;
        protected string _repositoryInterfaceOutput;
        protected string _repositoryOutput;
        protected string _dtoOutput;
        protected string _mapperOutput;
        protected string _serviceInterfaceOutput;
        protected string _serviceOutput;
        protected string _controllerOutput;
        protected string _testOutput;
        protected string _serviceCollectionPath; 

        private string _consoleMessage;
        private string _dbSet;
        private string _dto;
        private string _class;
        private string _variable;
        private string _property;
        private bool _allFiles;
        private bool _setRepositoryConfig = false;
        private bool _setServiceConfig = false; 


        private struct BUILD 
        {
            public const string InterfaceRepository = "Interface Repository";
            public const string Repository = "Repository";
            public const string DTO = "Data Transfer Object";
            public const string Mapper = "Mapper";
            public const string InterfaceService = "Interface Service";
            public const string Service = "Service";
            public const string Controller = "Controller"; 
            public const string Tests = "Tests";
        }


        protected void PrintHeader()
        {
            Console.Clear();
            _consoleMessage = $"{"".PadRight(50, '*')}\n";
            _consoleMessage += $"{"".PadLeft(15, '*')}{"",-5}COERSystem{"",-5}{"".PadRight(15, '*')}\n";
            _consoleMessage += $"{"".PadLeft(15, '*')}{"",-6}Scaffold{"",-6}{"".PadRight(15, '*')}\n";
            _consoleMessage += $"{"".PadRight(50, '*')}\n";
            Console.WriteLine(_consoleMessage);
        }


        protected void GetDbSet()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(_consoleMessage);
                Console.Write($"DbSet: ");

                string dbSet = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(dbSet))
                {
                    Alert("Enter a DbSet...");
                    continue;
                }

                dbSet = dbSet.ToPascalCase();

                if (ExistsDbSet(dbSet))
                {
                    _dbSet = dbSet;
                    _consoleMessage += $"\nDbSet: {_dbSet}\n\n";
                    break;
                }

                Alert($"DbSet<{dbSet}> Not Found...");
            }
        }


        private bool ExistsDbSet(string dbSet)
        {
            try
            {
                string path = $"{_contextPath}/{_project}Context.cs";

                if (File.Exists(path))
                {
                    string row = string.Empty;
                    string line = string.Empty;
                    Dictionary<string, string> dbSetDictionary = [];
                    using StreamReader streamReader = new(path);

                    do
                    {
                        line = streamReader.ReadLine();
                        line = line.CleanUpBlanks();

                        if (line is null) break;
                        else if (line.Contains("DbSet<") && line.EndsWith("{ get; set; }"))
                        {
                            row = line.Split("DbSet")[1];
                            row = row.Split("{")[0];
                            row = row.Replace("<", "");
                            row = row.Replace(">", "");
                            row = row.Trim();

                            dbSetDictionary.Add(row.Split(" ")[0], row.Split(" ")[1]);
                            row = string.Empty;
                        }

                    } while (line is not null);

                    streamReader.Close();
                    streamReader.Dispose();
                    if (dbSetDictionary.ContainsKey(dbSet))
                    {
                        _dto = dbSet.StartsWith("Tbl") ? dbSet.Replace("Tbl", "") : dbSet;
                        _dto = _dto.EndsWith("DTO") ? _dto : $"{_dto}DTO";
                        _dto = _dto.FirstCharToUpper();
                        _class = _dto.Replace("DTO", "");
                        _variable = _class.FirstCharToLower();
                        _property = dbSetDictionary.GetValueOrDefault(dbSet);
                        return true;
                    }
                }
            }

            catch (Exception ex)
            {
                Alert(ex?.InnerException?.Message ?? ex.Message);
            }

            return false;
        } 


        protected void CreateIRepository()
        {
            string path = BeginBuild(BUILD.InterfaceRepository, _repositoryInterfaceOutput, $"I{_class}Repository.cs");

            if (path is not null)
            {
                using StreamWriter streamWriter = File.CreateText(path);
                streamWriter.WriteLine("using System.Linq.Expressions;");
                streamWriter.WriteLine($"using Repository.Context;");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"namespace Repository.Interfaces");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine($"\tpublic interface I{_class}Repository");
                streamWriter.WriteLine("\t{");
                streamWriter.WriteLine($"\t\tTask<bool> Exists{_class}(Expression<Func<{_dbSet}, bool>> expression);");
                streamWriter.WriteLine($"\t\tTask<{_dbSet}> Get{_class}By(Expression<Func<{_dbSet}, bool>> expression);");
                streamWriter.WriteLine($"\t\tTask<List<{_dbSet}>> Get{_class}List(Expression<Func<{_dbSet}, bool>> expression);");
                streamWriter.WriteLine($"\t\tTask<{_dbSet}> Create{_class}({_dbSet} entity);");
                streamWriter.WriteLine($"\t\tTask<List<{_dbSet}>> Create{_class}(IEnumerable<{_dbSet}> entities);");
                streamWriter.WriteLine($"\t\tTask<{_dbSet}> Update{_class}({_dbSet} entity);");
                streamWriter.WriteLine($"\t\tTask<List<{_dbSet}>> Update{_class}(IEnumerable<{_dbSet}> entities);");
                streamWriter.WriteLine($"\t\tTask<int> Delete{_class}({_dbSet} entity);");
                streamWriter.WriteLine($"\t\tTask<int> Delete{_class}(IEnumerable<{_dbSet}> entities);");
                streamWriter.WriteLine("\t}");
                streamWriter.Write("}");

                EndBuild(streamWriter, path);  
            }
            
        }


        protected void CreateRepository()
        {
            string path = BeginBuild(BUILD.Repository, _repositoryOutput, $"{_class}Repository.cs");

            if (path is not null) 
            {
                using StreamWriter streamWriter = File.CreateText(path);
                streamWriter.WriteLine("using Microsoft.EntityFrameworkCore;");
                streamWriter.WriteLine("using System.Linq.Expressions;");
                streamWriter.WriteLine("using Repository.Interfaces;");
                streamWriter.WriteLine("using Repository.Context;");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"namespace Repository.Repositories");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine($"\tpublic class {_class}Repository : I{_class}Repository");
                streamWriter.WriteLine("\t{");
                streamWriter.WriteLine($"\t\tprivate readonly {_project}Context _context;");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"\t\tpublic {_class}Repository({_project}Context context)");
                streamWriter.WriteLine("\t\t\t=> _context = context;");
                streamWriter.WriteLine();
                streamWriter.WriteLine();

                //Exist
                streamWriter.WriteLine($"\t\tpublic async Task<bool> Exists{_class}(Expression<Func<{_dbSet}, bool>> expression)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\treturn await _context.{_property}");
                streamWriter.WriteLine("\t\t\t\t.AsNoTracking()");
                streamWriter.WriteLine("\t\t\t\t.AnyAsync(expression);");
                streamWriter.WriteLine("\t\t}");
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
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tawait _context.{_property}.AddAsync(entity);");
                streamWriter.WriteLine("\t\t\tawait _context.SaveChangesAsync();");
                streamWriter.WriteLine($"\t\t\treturn entity;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine();

                //Create
                streamWriter.WriteLine($"\t\tpublic async Task<List<{_dbSet}>> Create{_class}(IEnumerable<{_dbSet}> entities)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tawait _context.{_property}.AddRangeAsync(entities);");
                streamWriter.WriteLine("\t\t\tawait _context.SaveChangesAsync();");
                streamWriter.WriteLine($"\t\t\treturn entities.ToList();");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine();

                //Update
                streamWriter.WriteLine($"\t\tpublic async Task<{_dbSet}> Update{_class}({_dbSet} entity)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\t_context.{_property}.Update(entity);");
                streamWriter.WriteLine("\t\t\tawait _context.SaveChangesAsync();");
                streamWriter.WriteLine($"\t\t\treturn entity;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine();

                //Update
                streamWriter.WriteLine($"\t\tpublic async Task<List<{_dbSet}>> Update{_class}(IEnumerable<{_dbSet}> entities)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\t_context.{_property}.UpdateRange(entities);");
                streamWriter.WriteLine("\t\t\tawait _context.SaveChangesAsync();");
                streamWriter.WriteLine($"\t\t\treturn entities.ToList();");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine();

                //Delete
                streamWriter.WriteLine($"\t\tpublic async Task<int> Delete{_class}({_dbSet} entity)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\t_context.{_property}.Remove(entity);");
                streamWriter.WriteLine("\t\t\tint rows = await _context.SaveChangesAsync();");
                streamWriter.WriteLine("\t\t\t_context.ChangeTracker.Clear();");
                streamWriter.WriteLine("\t\t\treturn rows;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine();

                //Delete
                streamWriter.WriteLine($"\t\tpublic async Task<int> Delete{_class}(IEnumerable<{_dbSet}> entities)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\t_context.{_property}.RemoveRange(entities);");
                streamWriter.WriteLine("\t\t\tint rows = await _context.SaveChangesAsync();");
                streamWriter.WriteLine("\t\t\t_context.ChangeTracker.Clear();");
                streamWriter.WriteLine("\t\t\treturn rows;");
                streamWriter.WriteLine("\t\t}");

                //End Class
                streamWriter.WriteLine("\t}");
                streamWriter.Write("}");

                EndBuild(streamWriter, path);
            } 
        }


        protected void CreateDto()
        {
            string path = BeginBuild(BUILD.DTO, _dtoOutput, $"{_dto}.cs");

            if (path is not null)
            {
                using StreamWriter streamWriter = File.CreateText(path);
                streamWriter.WriteLine("namespace Microservices.DTOs");
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
                                new[] { "int", "float", "double", "decimal", "bool", "string", "byte[]", "DateTime", "DateOnly", "TimeSpan" }.Any(x => type.StartsWith(x))
                                ? "\t\t" : "\t\t//"
                            );

                            streamWriter.WriteLine(line + "\n");
                        }

                        break;
                    }
                }

                streamWriter.WriteLine("\t}");
                streamWriter.Write("}");

                EndBuild(streamWriter, path);
            }
        }


        protected void CreateMapper()
        {
            string path = BeginBuild(BUILD.Mapper, _mapperOutput, $"{_class}Mapper.cs");

            if (path is not null) 
            { 
                using StreamWriter streamWriter = File.CreateText(path);
                streamWriter.WriteLine("using Repository.Context;");
                streamWriter.WriteLine("using Microservices.DTOs;");
                streamWriter.WriteLine("using AutoMapper;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace Microservices.Mappers");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine($"\tpublic class {_class}Mapper : Profile");
                streamWriter.WriteLine("\t{");
                streamWriter.WriteLine($"\t\tpublic {_class}Mapper()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tCreateMap<{_dbSet}, {_dto}>()");
                streamWriter.WriteLine($"\t\t\t\t.AfterMap<{_class}Action>();");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"\t\t\tCreateMap<{_dto}, {_dbSet}>()");
                streamWriter.WriteLine($"\t\t\t\t.AfterMap<{_class}Action>();");
                streamWriter.WriteLine("\t\t}");
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
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("\t}");
                streamWriter.Write("}");

                EndBuild(streamWriter, path); 
            } 
        }


        protected void CreateIService()
        {
            string path = BeginBuild(BUILD.InterfaceService, _serviceInterfaceOutput, $"I{_class}Service.cs");
            
            if (path is not null) 
            { 
                using StreamWriter streamWriter = File.CreateText(path);
                streamWriter.WriteLine("using Microsoft.AspNetCore.JsonPatch;");
                streamWriter.WriteLine("using Microservices.DTOs;");
                streamWriter.WriteLine("using COERSystem;");

                streamWriter.WriteLine();
                streamWriter.WriteLine($"namespace Microservices.Interfaces");
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

                EndBuild(streamWriter, path);  
            } 
        }


        protected void CreateService()
        {
            string path = BeginBuild(BUILD.Service, _serviceOutput, $"{_class}Service.cs");

            if (path is not null) 
            { 
                string dto = _dto.FirstCharToLower();

                using StreamWriter streamWriter = File.CreateText(path);
                streamWriter.WriteLine("using Microsoft.AspNetCore.JsonPatch;");
                streamWriter.WriteLine("using Repository.Interfaces;");
                streamWriter.WriteLine("using Repository.Context;");
                streamWriter.WriteLine("using Microservices.Interfaces;");
                streamWriter.WriteLine("using Microservices.DTOs;");
                streamWriter.WriteLine("using AutoMapper;");
                streamWriter.WriteLine("using COERSystem;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace Microservices.Services");
                streamWriter.WriteLine("{");

                //Class
                streamWriter.WriteLine($"\tpublic class {_class}Service : I{_class}Service");
                streamWriter.WriteLine("\t{");
                streamWriter.WriteLine($"\t\tprivate readonly I{_class}Repository _repository;");
                streamWriter.WriteLine("\t\tprivate readonly IMapper _mapper;");
                streamWriter.WriteLine();

                //Constructor
                streamWriter.WriteLine($"\t\tpublic {_class}Service(");
                streamWriter.WriteLine($"\t\t\tI{_class}Repository repository,");
                streamWriter.WriteLine("\t\t\tIMapper mapper");
                streamWriter.WriteLine("\t\t) {");
                streamWriter.WriteLine("\t\t\t_repository = repository;");
                streamWriter.WriteLine("\t\t\t_mapper = mapper;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine();

                //GET ById 
                streamWriter.WriteLine($"\t\tpublic async Task<ResponseDTO<{_dto}>> Get{_class}ById(int {_variable}Id)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tResponseDTO<{_dto}> response = new();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\ttry");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine($"\t\t\t\t{_dbSet} entity = await _repository.Get{_class}By(x => x.Id == {_variable}Id);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\tif (entity is null)");
                streamWriter.WriteLine("\t\t\t\t\treturn response.NotFound();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Response");
                streamWriter.WriteLine($"\t\t\t\tresponse.Data = _mapper.Map<{_dto}>(entity);");
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
                streamWriter.WriteLine($"\t\t\t\tList<{_dbSet}> entities = await _repository.Get{_class}List(x => true);");
                streamWriter.WriteLine($"\t\t\t\tList<{_dto}> dtoList = _mapper.Map<List<{_dto}>>(entities);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Response");
                streamWriter.WriteLine("\t\t\t\tresponse.Data = dtoList.OrderBy(x => x.Name).ToList();");
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
                streamWriter.WriteLine("\t\t\t\t//Get");
                streamWriter.WriteLine($"\t\t\t\t{_dbSet} entity = await _repository.Get{_class}By(x => x.Id == {_variable}Id);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\tif (entity is null)");
                streamWriter.WriteLine("\t\t\t\t\treturn response.NotFound();");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\t//Delete");
                streamWriter.WriteLine("\t\t\t\tentity = Clean.NoNesting(entity);");
                streamWriter.WriteLine($"\t\t\t\tawait _repository.Delete{_class}(entity);");
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

                EndBuild(streamWriter, path);
            } 
        }


        protected void CreateController()
        {
            string path = BeginBuild(BUILD.Controller, _controllerOutput, $"{_class}Controller.cs");

            if (path is not null) 
            { 
                string dto = _dto.FirstCharToLower();

                using StreamWriter streamWriter = File.CreateText(path);
                streamWriter.WriteLine("using Microsoft.AspNetCore.JsonPatch;");
                streamWriter.WriteLine("using Microsoft.AspNetCore.Mvc;");
                streamWriter.WriteLine("using Microservices.Interfaces;");
                streamWriter.WriteLine("using Microservices.DTOs;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace API.Controllers");
                streamWriter.WriteLine("{");

                //Class 
                streamWriter.WriteLine("\t[ApiController]");
                streamWriter.WriteLine($"\t[Route(\"api/{_class}\")]");
                streamWriter.WriteLine($"\tpublic class {_class}Controller : ControllerBase");
                streamWriter.WriteLine("\t{"); 

                //Constructor
                streamWriter.WriteLine($"\t\tprivate readonly I{_class}Service _service;");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"\t\tpublic {_class}Controller(I{_class}Service service)");
                streamWriter.WriteLine("\t\t\t=> _service = service;");
                streamWriter.WriteLine();
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
                streamWriter.WriteLine("\t\t\treturn NoContent();");
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
                streamWriter.WriteLine("\t\t\treturn NoContent();");
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

                EndBuild(streamWriter, path); 
            } 
        }


        protected void CreateTests()
        {
            string path = BeginBuild(BUILD.Tests, _testOutput, $"{_class}Test.cs");
            
            if (path is not null) 
            {                 
                using StreamWriter streamWriter = File.CreateText(path);
                streamWriter.WriteLine("using Microservices.Interfaces;");
                streamWriter.WriteLine("using Microservices.Services;");
                streamWriter.WriteLine("using Repository.Interfaces; ");
                streamWriter.WriteLine("using Repository.Repositories;");
                streamWriter.WriteLine("using Setup;");
                streamWriter.WriteLine("using Xunit;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace Tests");
                streamWriter.WriteLine("{");

                //Class
                streamWriter.WriteLine($"\tpublic class {_class}Test : DBConnectionTests");
                streamWriter.WriteLine("\t{");
                streamWriter.WriteLine();

                //Constructor
                streamWriter.WriteLine($"\t\tprivate readonly I{_class}Repository _repository;");
                streamWriter.WriteLine($"\t\tprivate readonly I{_class}Service _service;");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"\t\tpublic {_class}Test()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\t_repository = new {_class}Repository(_context);");
                streamWriter.WriteLine($"\t\t\t_service = new {_class}Service(_repository, _mapper);");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
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

                EndBuild(streamWriter, path);  
            } 
        }


        protected void SetServiceCollection(string collection = null)
        {
            if (string.IsNullOrWhiteSpace(collection))
            {
                if (!string.IsNullOrWhiteSpace(_serviceCollectionPath))
                {
                    Console.Clear();
                    Console.Write(_consoleMessage);

                    if (_setRepositoryConfig)
                        SetServiceCollection(BUILD.Repository);

                    if (_setServiceConfig)
                        SetServiceCollection(BUILD.Service);
                }
            }

            else
            {
                try
                {
                    string path = $"{_serviceCollectionPath}/{(collection.Equals(BUILD.Service) ? "Microservice" : collection)}Collection.cs";

                    if (File.Exists(path))
                    {
                        Dictionary<double, string> dictionary = GetContentFile(path);

                        //Add Service
                        string service = $"{collection.FirstCharToLower()}.AddTransient<I{_class}{collection.FirstCharToUpper()}, {_class}{collection.FirstCharToUpper()}>();";

                        if (!dictionary.Any(x => x.Value.Trim().Contains(service)))
                        {
                            double keyRepository = dictionary.FirstOrDefault(x => x.Value.Contains($"return {collection.FirstCharToLower()};")).Key;
                            dictionary.Add(keyRepository - 1 + 0.1, $"\t\t\t{service}");

                            using StreamWriter textWriter = new(path);

                            foreach (double key in dictionary.Keys.OrderBy(x => x))
                                textWriter.WriteLine(dictionary[key]);

                            textWriter.Close();
                            textWriter.Dispose();

                            _consoleMessage += $"=> {service}\n";
                            _consoleMessage += $"{path}\n\n";
                        }

                        Console.Clear();
                        Console.Write(_consoleMessage);
                    }
                }

                catch (Exception ex)
                {
                    Alert(ex?.InnerException?.Message ?? ex.Message);
                }
            }
        } 


        private Dictionary<double, string> GetContentFile(string path)
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


        protected void CreateAllFiles()
            => _allFiles = Confirm("Create All Files");


        private void Alert(string message)
        {
            Console.Write("\n\n\n\n\n\n\t\t\t\t\t\t\t\t");
            Console.Write(message);
            Console.ReadKey();
        }


        protected bool Confirm(string message)
        {
            do
            {
                Console.Clear();
                Console.Write(_consoleMessage);
                Console.Write($"{message}: ");

                string answer = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(answer)) Alert("Wrong answer...\n\t\t\t\t\t\t\t\tConfirm('Y', 'YES', 'S', 'SI', 'T', 'TRUE', '1')\n\t\t\t\t\t\t\t\tRejec('N', 'NO', 'F', 'FALSE', '0')");

                else
                {
                    answer = answer.ToUpper().Trim();

                    if (answer.Equals("Y") || answer.Equals("YES")
                     || answer.Equals("S") || answer.Equals("SI")
                     || answer.Equals("T") || answer.Equals("TRUE")
                     || answer.Equals("1")) return true;

                    else if (answer.Equals("N") || answer.Equals("NO")
                          || answer.Equals("F") || answer.Equals("FALSE")
                          || answer.Equals("0")) return false;

                    else Alert("Wrong answer...\n\t\t\t\t\t\t\t\tConfirm('Y', 'YES', 'S', 'SI', 'T', 'TRUE', '1')\n\t\t\t\t\t\t\t\tRejec('N', 'NO', 'F', 'FALSE', '0')");
                }
            } while (true);
        } 


        private string BeginBuild(string build, string directoryPath, string fileName) 
        {
            if (!_allFiles && !Confirm($"Create {build}")) return null;

            else 
            {
                _consoleMessage += $"=> Creating {build}...".PadRight(50, ' ');

                Console.Clear();
                Console.Write(_consoleMessage);

                if (_setRepositoryConfig is false)
                    _setRepositoryConfig = build.Equals(BUILD.InterfaceRepository) || build.Equals(BUILD.Repository);

                if (_setServiceConfig is false)
                    _setServiceConfig = build.Equals(BUILD.InterfaceService) || build.Equals(BUILD.Service);
                 
                if (string.IsNullOrWhiteSpace(directoryPath))
                    directoryPath = "./Scaffold";

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                string path = $"{directoryPath}/{fileName}";

                if (File.Exists(path))
                {
                    _consoleMessage += $"Already exists\n{path}\n\n";
                    Console.WriteLine($"Already exists\n{path}\n\n");
                    return null;
                }

                else
                    return path;
            }
        }


        private void EndBuild(StreamWriter streamWriter, string path)
        {
            streamWriter.Close();
            streamWriter.Dispose();
            _consoleMessage += $"Created\n{path}\n\n";
            Console.WriteLine($"Created\n{path}\n\n");
        }
    } 
} 