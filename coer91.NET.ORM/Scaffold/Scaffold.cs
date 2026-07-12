namespace coer91.NET.ORM
{
    public sealed class Scaffold : ScaffoldBuilder
    {
        public Scaffold SQLServerProfile(ScaffoldProfile profile)
        {
            _sqlServerProfileList = [.. _sqlServerProfileList.Append(profile)];
            return this;
        }

        public async Task Build()
        { 
            do
            {
                PrintHeader();
                SelectDatabase();
                await SyncDatabase(); 
                GetDbSet();
                await CreateRepository();
                await CreateDto();
                await CreateMapper();
                await CreateService();
                await CreateController();
                await CreateTests();
                await SetServiceCollection();

            } while (Confirm("\nContinue with more models"));                       

            Environment.Exit(0);
        } 
    }
}