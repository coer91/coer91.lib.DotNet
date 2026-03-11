namespace coer91.NET
{
    public sealed class Scaffold : ScaffoldBuilder
    {
        public Scaffold SetProject(string name)
        {
            _project = name.ToPascalCase();
            return this;
        }


        public Scaffold SetContextPath(string path)
        {
            _contextPath = $"../{path}";
            return this;
        }


        public Scaffold SetDBContextName(string name)
        {
            _contextName = name;
            return this;
        }


        public Scaffold SetRepositoryInterfaceOutput(string path)
        {
            _repositoryInterfaceOutput = $"../{path}";
            return this;
        }


        public Scaffold SetRepositoryOutput(string path)
        {
            _repositoryOutput = $"../{path}";
            return this;
        }


        public Scaffold SetDtoOutput(string path)
        {
            _dtoOutput = $"../{path}";
            return this;
        }


        public Scaffold SetMapperOutput(string path)
        {
            _mapperOutput = $"../{path}";
            return this;
        }


        public Scaffold SetServiceInterfaceOutput(string path)
        {
            _serviceInterfaceOutput = $"../{path}";
            return this;
        }


        public Scaffold SetServiceOutput(string path)
        {
            _serviceOutput = $"../{path}";
            return this;
        }


        public Scaffold SetControllerOutput(string path)
        {
            _controllerOutput = $"../{path}";
            return this;
        }


        public Scaffold SetTestOutput(string path)
        {
            _testOutput = $"../{path}";
            return this;
        }


        public Scaffold SetServiceCollectionPath(string path)
        {
            _serviceCollectionPath = $"../{path}";
            return this;
        }


        public Scaffold SetXunitSetupPath(string path)
        {
            _xunitSetupPath = $"../{path}";
            return this;
        }


        public void Build()
        {
            string message = StartScaffold();

            if (string.IsNullOrWhiteSpace(message))
                do
                {
                    PrintHeader();
                    GetDbSet();
                    CreateAllFiles();
                    CreateIRepository();
                    CreateRepository();
                    CreateDto();
                    CreateMapper();
                    CreateIService();
                    CreateService();
                    CreateController();
                    CreateTests();
                    SetServiceCollection();

                } while (Confirm("Continue with more models"));

            else
                Console.WriteLine(message);

            Environment.Exit(0);
        }


        private string StartScaffold()
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(_project))
                message += "\n - The project name has not been set.";

            if (string.IsNullOrWhiteSpace(_contextPath))
                message += "\n - The context path has not been set.";

            if (string.IsNullOrWhiteSpace(_contextName))
                _contextName = $"{_project}Context";

            return message;
        }
    }
}