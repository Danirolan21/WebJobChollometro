using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebJobChollometro.Data;
using WebJobChollometro.Repositories;

string connectionString = "Data Source=sqldani3213.database.windows.net;Initial Catalog=AZURETAJAMAR;Persist Security Info=True;User ID=adminsql;Password=*********;Encrypt=True;Trust Server Certificate=True";

var provider = new ServiceCollection()
    .AddTransient<RepositoryChollometro>()
    .AddDbContext<ChollometroContext>
    (options => options.UseSqlServer(connectionString))
    .BuildServiceProvider();

RepositoryChollometro repo =
    provider.GetService<RepositoryChollometro>();
//Console.WriteLine("Pulse Enter para Iniciar");
//Console.ReadLine();
await repo.PopulateChollosAzureAsync();
//Console.WriteLine("Proceso completado correctamente");
//Console.WriteLine("Enhorabuena!!!");