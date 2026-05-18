using System.Xml.Serialization;
//using VIdeoStoreApp.Application.Interfaces;
//using VIdeoStoreApp.Application.Services;
//using VIdeoStoreApp.Infrastructure.Repository;

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

        

            //builder.Services.AddScoped<IVideoStoreRepository, VideoStoreRepository>();
            //builder.Services.AddScoped<IVideoStoreService, VideoStoreService>();

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
