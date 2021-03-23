using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreCodeCamp
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<CampContext>();
      services.AddScoped<ICampRepository, CampRepository>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly()); //After adding Auto mapper from nuget 

      services.AddControllers();//no view support , pure conyrollers only

            //for swagger implementaion
          //  services.AddSwaggerGen();

            //can also modify the open api look by adding further details showing version, desc, license, terms of service
            services.AddSwaggerGen(o =>
                o.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Code Camp Core API",
                    Description = "Asp .net core WebApi ",
                    TermsOfService = new Uri("https://paima.com"), //can mention any url specific
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Parminder",
                        Email = "parmindergrewal0791@gmail.com",
                        Url = new Uri("https://paima.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense
                    {
                        Name = "Open License",
                         Url = new Uri("https://paima.com")
                    }
                }
            ));

            services.AddCors(options => options.AddDefaultPolicy(
                builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .WithOrigins("http://localhost:4200")));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
            //for swagger implementaion add below 2 middlewares
            app.UseSwagger();

            app.UseCors(); //before routing , before authorisation

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Camp API V1");
            }); //run the project and hit http://localhost:6600/swagger/v1/swagger.json can see the open api json
            //http://localhost:6600/swagger/index.html provides the swagger 

            if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(cfg =>
      {
        cfg.MapControllers();//no mapping views
      });
    }
  }
}

/* Post man download, redirectHttp false, 2 pane enable it
 * 
 * dotnet ef database update 
 * https://database.guide/how-to-install-sql-server-on-a-mac/
 * https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio?view=sql-server-ver15
 * https://github.com/microsoft/azuredatastudio/issues/447
 * 
 * 
 * once u change the data in respository any db data or add or add any new table and want to upate the database
 * 
 * u must 1st create a migration: dotnet ef migrations add 'initial-data'   //anyname
 * then for db update : dotnet ef database update
 * 
 * if want to remove migration(always current migration will be removed): dotnet ef migrations remove 
 * if want to drop databse use the query as 
 * 
alter database codeCampdb set single_user with rollback immediate
drop database codeCampdb
 */
