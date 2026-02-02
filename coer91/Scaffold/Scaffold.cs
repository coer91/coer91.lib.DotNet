namespace coer91
{
    public sealed class ScaffoldDDD : ScaffoldBuilder
    { 
        public ScaffoldDDD SetProject(string name)
        {
            _project = name.ToPascalCase();
            return this;
        }


        public ScaffoldDDD SetContextPath(string path)
        {
            _contextPath = path;
            return this;
        }


        public ScaffoldDDD SetDBContextName(string name)
        {
            _contextName = name;
            return this;
        }


        public ScaffoldDDD SetRepositoryInterfaceOutput(string path)
        {
            _repositoryInterfaceOutput = path;
            return this;
        }


        public ScaffoldDDD SetRepositoryOutput(string path)
        {
            _repositoryOutput = path;
            return this;
        }


        public ScaffoldDDD SetDtoOutput(string path)
        {
            _dtoOutput = path;
            return this;
        }


        public ScaffoldDDD SetMapperOutput(string path)
        {
            _mapperOutput = path;
            return this;
        }


        public ScaffoldDDD SetServiceInterfaceOutput(string path)
        {
            _serviceInterfaceOutput = path;
            return this;
        }


        public ScaffoldDDD SetServiceOutput(string path)
        {
            _serviceOutput = path;
            return this;
        }


        public ScaffoldDDD SetControllerOutput(string path)
        {
            _controllerOutput = path;
            return this;
        }


        public ScaffoldDDD SetTestOutput(string path)
        {
            _testOutput = path;
            return this;
        }


        public ScaffoldDDD SetServiceCollectionPath(string path)
        {
            _serviceCollectionPath = path;
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