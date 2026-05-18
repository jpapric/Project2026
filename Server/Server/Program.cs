using Server.Application.Interfaces;
using Server.Application.Services;
using Server.Infrastructure.Repository;
using System.Xml.Serialization;
using Server.Application.Interfaces;
using Server.Application.Services;
using Server.Infrastructure.Repository;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddScoped<IServerRepository, ServerRepository>();
            builder.Services.AddScoped<IServerService, ServerService>();
            
            //Dependency injection (DI)
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();
            app.Run();
        }
    }
}
