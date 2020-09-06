using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainModuleContext.Context
{
    public  class MainModuleContext:DbContext
    {
        public MainModuleContext()
        {

        }
        public MainModuleContext(DbContextOptions<MainModuleContext>options):base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {


                optionsBuilder.UseSqlServer("Data Source=172.30.1.17;Initial Catalog=temp_newgiza;User ID=sa;Password=Wizard$1834");
            }
        }
    }
}
