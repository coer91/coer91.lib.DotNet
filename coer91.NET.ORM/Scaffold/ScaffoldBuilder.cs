using coer91.NET.Files; 

namespace coer91.NET.ORM
{
    public abstract class ScaffoldBuilder : ScaffoldWritter
    {         
        private bool _sync = false;
        private bool _scaffold = false;
        private bool _allFiles = false;
        private bool _setDto = false;
        private bool _setAutoMapper = false;
        private bool _setRepositoryConfig = false;
        private bool _setServiceConfig = false;
        private bool _setTestConfig = false;

        private string _consoleMessage; 
        protected ScaffoldProfile[] _sqlServerProfileList = [];


        //STEP 1
        protected void PrintHeader()
        {
            Console.Clear();
            _consoleMessage = $"{"".PadRight(50, '*')}\n";
            _consoleMessage += $"{"".PadLeft(15, '*')}{"",-6}Scaffold{"",-6}{"".PadRight(15, '*')}\n";
            _consoleMessage += $"{"".PadRight(50, '*')}\n";
            Console.WriteLine(_consoleMessage);
        } 


        //STEP 2
        protected void SelectDatabase()
        {
            if (_scaffold) 
            {
                _consoleMessage += $"\nDatabase: {_database}";
                return;
            }
            
            string[] databaseList = [];

            //SQL SERVER
            for (int i = 0; i < _sqlServerProfileList.Length; i++)
            { 
                if (string.IsNullOrWhiteSpace(_sqlServerProfileList[i].ConnectionString))
                    continue;

                string connection = _sqlServerProfileList[i].ConnectionString.Trim();

                if (connection.Contains(';') && connection.Contains("Server" , StringComparison.OrdinalIgnoreCase) && connection.Contains("Catalog", StringComparison.OrdinalIgnoreCase)) {                    
                    string server   = connection.Split(";").FirstOrDefault(x => x.Contains("Server",  StringComparison.OrdinalIgnoreCase)).Split("=")[1].Trim();
                    string database = connection.Split(";").FirstOrDefault(x => x.Contains("Catalog", StringComparison.OrdinalIgnoreCase)).Split("=")[1].Trim();
                    databaseList    = [.. databaseList.Append($"{database} [SQL SERVER][{server}]")];
                }

                else  
                    databaseList = [.. databaseList.Append($"{connection} [SQL SERVER]")];                 
            }
            
            //ONE PROFILE
            if (databaseList.Length == 1)
            {
                _database = databaseList.FirstOrDefault();
                _profile = _sqlServerProfileList[0];
            }

            else while (true)
            {
                Console.Clear();
                Console.WriteLine(_consoleMessage);

                for (int i = 1; i <= databaseList.Length; i++)
                    Console.WriteLine($"  {i}) {databaseList[i - 1]}");

                Console.Write($"\nSelect a Database: ");

                string option = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(option))                    
                    continue;

                if (int.TryParse(option, out int _option))
                {
                    try
                    {
                        _database = databaseList[_option - 1];

                        //SQL SERVER
                        _profile = _sqlServerProfileList[_option - 1];

                    }
                    catch
                    {
                        _database = null;
                    }
                }

                if (!string.IsNullOrWhiteSpace(_database)) break;
            } 

            _consoleMessage += $"\nDatabase: {_database}";
        }


        //STEP 3
        protected async Task SyncDatabase()
        {
            if (_sync) 
            {
                _consoleMessage += " sync";
                return;
            }

            if (_scaffold) return;

            Console.Clear();
            Console.WriteLine(_consoleMessage);

            //Clean Data
            if (string.IsNullOrWhiteSpace(_profile.StartupProject))
                _profile.StartupProject = "API";

            if (string.IsNullOrWhiteSpace(_profile.Project))
                _profile.Project = "Repositories";

            if (string.IsNullOrWhiteSpace(_profile.ContextOutput))
                _profile.ContextOutput = "Database";

            if (string.IsNullOrWhiteSpace(_profile.ContextNamespace))
                _profile.ContextNamespace = $"{_profile.Project}.{_profile.ContextOutput}";

            if (string.IsNullOrWhiteSpace(_profile.ContextName))
            {
                string database = _profile.ConnectionString.Contains(';')
                    ? _profile.ConnectionString.Split(";").FirstOrDefault(x => x.Contains("Catalog", StringComparison.OrdinalIgnoreCase))?.Split("=")[1].Trim()
                    : _profile.ConnectionString;

                if (string.IsNullOrWhiteSpace(database))
                    throw new ArgumentNullException("Context name not provided");

                _profile.ContextName = $"{database}Context";
            }

            if (Confirm("\nDo you want to sync the database"))
            {
                //SQL SERVER 
                await ScaffoldSync.SQLServerSync(_profile);

                Console.Clear();
                _consoleMessage += " sync";
                Console.WriteLine(_consoleMessage);
                _sync = true;
            }
        }


        //STEP 4
        protected void GetDbSet()
        {
            if (_scaffold || Confirm("\nDo you want to scaffold a DbSet"))
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine(_consoleMessage);
                    Console.Write($"DbSet: ");

                    string dbSet = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(dbSet))
                        continue;

                    dbSet = dbSet.ToPascalCase();

                    if (LoadDbSet(dbSet))
                    {
                        _dbSet = dbSet;
                        _consoleMessage += $"\nDbSet: {_dbSet}\n";
                        _allFiles = Confirm("Create All Files");

                        Console.Clear();
                        Console.WriteLine(_consoleMessage);
                        break;
                    } 
                }

                _scaffold = true;
            }   
            
            else Environment.Exit(0);
        }


        //STEP 5
        private bool LoadDbSet(string dbSet)
        {
            try
            {
                //SQL SERVER                
                string path = $"{_rootPath}/{_profile.Project}/{_profile.ContextOutput}/{_profile.ContextName}.cs";

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


                Alert($"DbSet<{dbSet}> Not Found...");
            }

            catch (Exception ex)
            {
                Alert(ex?.InnerException?.Message ?? ex.Message);
            }

            return false;
        }


        //STEP 6
        protected async Task CreateRepository()
        {
            _setRepositoryConfig = false;
            if (!_allFiles && !Confirm($"\nCreate Repository")) return;
            Console.Clear(); 
            Console.WriteLine(_consoleMessage);

            //Interface 
            Console.Write($"=> Creating Repository Interface...".PadRight(50, ' '));            
            string directoryPath = FilesManagement.CreateDirectory($"{_rootPath}/{_profile.OutputFiles.IRepositoryOutput}");
            string path = $"{directoryPath}/I{_class}Repository.cs";

            if (File.Exists(path)) 
            {
                Console.Clear();
                _consoleMessage += $"\nRepository Interface already exists\n{path}\n";
                Console.WriteLine(_consoleMessage);
            } 

            else
            { 
                await WriteRepositoryInterface(path);

                Console.Clear();
                _consoleMessage += $"\nRepository Interface Created\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }


            //Repository
            Console.Write($"=> Creating Repository...".PadRight(50, ' '));
            directoryPath = FilesManagement.CreateDirectory($"{_rootPath}/{_profile.OutputFiles.RepositoryOutput}");
            path = $"{directoryPath}/{_class}Repository.cs";

            if (File.Exists(path))
            {
                Console.Clear();
                _consoleMessage += $"\nRepository already exists\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            else
            { 
                await WriteRepository(path);

                Console.Clear();
                _consoleMessage += $"\nRepository Created\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            _setRepositoryConfig = true;
        }


        //STEP 7
        protected async Task CreateDto()
        {
            _setDto = false;
            if (!_allFiles && !Confirm($"\nCreate DTO")) return;
            Console.Clear();
            Console.WriteLine(_consoleMessage);
            Console.Write($"=> Creating DTO...".PadRight(50, ' '));

            string directoryPath = FilesManagement.CreateDirectory($"{_rootPath}/{_profile.OutputFiles.DtoOutput}");
            string path = $"{directoryPath}/{_dto}.cs";

            if (File.Exists(path))
            {
                Console.Clear();
                _consoleMessage += $"\nDTO already exists\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            else
            {
                await WriteDTO(path);

                Console.Clear();
                _consoleMessage += $"\nDTO Created\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            _setDto = true;
        } 


        //STEP 8
        protected async Task CreateMapper()
        {
            _setAutoMapper = false;
            if (!_allFiles && !Confirm($"\nCreate AutoMapper")) return;
            Console.Clear();
            Console.WriteLine(_consoleMessage);
            Console.Write($"=> Creating AutoMapper...".PadRight(50, ' '));

            string directoryPath = FilesManagement.CreateDirectory($"{_rootPath}/{_profile.OutputFiles.AutoMapperOutput}");
            string path = $"{directoryPath}/{_class}Mapper.cs";

            if (File.Exists(path))
            {
                Console.Clear();
                _consoleMessage += $"\nAutoMapper already exists\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            else
            {
                bool actionMapper = Confirm($"\nAdd ActionMapper");
                await WriteAutomapper(path, actionMapper);

                Console.Clear();
                _consoleMessage += $"\nAutoMapper Created\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            _setAutoMapper = true;
        }


        //STEP 9
        protected async Task CreateService()
        {
            _setServiceConfig = false;
            if (!_allFiles && !Confirm($"\nCreate Service")) return;
            Console.Clear();
            Console.WriteLine(_consoleMessage);

            //Interface 
            Console.Write($"=> Creating Service Interface...".PadRight(50, ' '));
            string directoryPath = FilesManagement.CreateDirectory($"{_rootPath}/{_profile.OutputFiles.IServiceOutput}");
            string path = $"{directoryPath}/I{_class}Service.cs";

            if (File.Exists(path))
            {
                Console.Clear();
                _consoleMessage += $"\nService Interface already exists\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            else
            {
                await WriteServiceInterface(path);

                Console.Clear();
                _consoleMessage += $"\nService Interface Created\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }


            //Service
            Console.Write($"=> Creating Service...".PadRight(50, ' '));
            directoryPath = FilesManagement.CreateDirectory($"{_rootPath}/{_profile.OutputFiles.ServiceOutput}");
            path = $"{directoryPath}/{_class}Service.cs";

            if (File.Exists(path))
            {
                Console.Clear();
                _consoleMessage += $"\nService already exists\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            else
            {
                bool addLogic = _setDto && _setAutoMapper && Confirm($"\nAdd Logic to Service");
                await WriteService(path, addLogic);

                Console.Clear();
                _consoleMessage += $"\nService Created\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            _setServiceConfig = true;
        }


        //STEP 10
        protected async Task CreateController()
        {
            if (!_allFiles && !Confirm($"\nCreate Controller")) return;
            Console.Clear();
            Console.WriteLine(_consoleMessage);
            Console.Write($"=> Creating Controller...".PadRight(50, ' '));
             
            string directoryPath = FilesManagement.CreateDirectory($"{_rootPath}/{_profile.OutputFiles.ControllerOutput}");
            string path = $"{directoryPath}/{_class}Controller.cs";

            if (File.Exists(path))
            {
                Console.Clear();
                _consoleMessage += $"\nController already exists\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            else
            {
                await WriteController(path);

                Console.Clear();
                _consoleMessage += $"\nController Created\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }
        }


        //Step 11
        protected async Task CreateTests()
        {
            _setTestConfig = false;

            if (string.IsNullOrWhiteSpace(_profile.OutputFiles.TestOutput)) return;
            if (!_allFiles && !Confirm($"\nCreate Tests")) return;
            Console.Clear();
            Console.WriteLine(_consoleMessage);
            Console.Write($"=> Creating Tests...".PadRight(50, ' '));

            string directoryPath = FilesManagement.CreateDirectory($"{_rootPath}/{_profile.OutputFiles.TestOutput}");
            string path = $"{directoryPath}/{_class}Test.cs";

            if (File.Exists(path))
            {
                Console.Clear();
                _consoleMessage += $"\nTests already exists\n{path}\n";
                Console.WriteLine(_consoleMessage);
            }

            else
            {
                await WriteTests(path);

                Console.Clear();
                _consoleMessage += $"\nTests Created\n{path}\n";
                Console.WriteLine(_consoleMessage);

            }

            _setTestConfig = true;
        }


        //Step 12
        protected async Task SetServiceCollection(string setupPath = null, string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(setupPath))
            {
                 Console.Clear();
                 Console.Write(_consoleMessage);

                if (_setRepositoryConfig)
                    await SetServiceCollection($"{_rootPath}/{_profile.OutputFiles.RepositorySetup}", "repository");

                if (_setServiceConfig)
                    await SetServiceCollection($"{_rootPath}/{_profile.OutputFiles.ServiceSetup}", "service");

                if (_setTestConfig)
                    await SetServiceCollection($"{_rootPath}/{_profile.OutputFiles.TestSetup}", "test");                
            }

            else
            {
                try
                {
                    if (prefix.Equals("test"))
                    { 
                        string path = $"{setupPath}/{_class}Injection.cs";

                        if (!File.Exists(path))
                        {
                            await WriteTestInjection(path);

                            Console.Clear();
                            _consoleMessage += $"\nTest Injection Created\n{path}\n";
                            Console.WriteLine(_consoleMessage);
                        } 
                    }

                    else
                    { 
                        if (!setupPath.EndsWith(".cs"))
                            setupPath = $"{setupPath}.cs";

                        if (File.Exists(setupPath))
                        {
                            Dictionary<double, string> dictionary = GetContentFile(setupPath);

                            //Add Service
                            string service = $"{prefix}.AddTransient<I{_class}{prefix.FirstCharToUpper()}, {_class}{prefix.FirstCharToUpper()}>();";

                            if (!dictionary.Any(x => x.Value.Trim().Contains(service)))
                            {
                                double keyRepository = dictionary.FirstOrDefault(x => x.Value.Contains($"return {prefix};")).Key;
                                dictionary.Add(keyRepository - 1 + 0.1, $"\t\t\t{service}");

                                using StreamWriter textWriter = new(setupPath);

                                foreach (double key in dictionary.Keys.OrderBy(x => x))
                                    textWriter.WriteLine(dictionary[key]);

                                textWriter.Close();
                                textWriter.Dispose();

                                _consoleMessage += $"\n=> {service}\n";
                                _consoleMessage += $"{setupPath}\n\n";
                            }

                            Console.Clear();
                            Console.Write(_consoleMessage);
                        }

                        else Alert($"{setupPath} Not Found");                    
                    }
                }

                catch (Exception ex)
                {
                    Alert(ex?.InnerException?.Message ?? ex.Message);
                }
            }
        }        


        private static void Alert(string message)
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
    }
}