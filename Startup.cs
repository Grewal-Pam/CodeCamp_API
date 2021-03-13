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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
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
