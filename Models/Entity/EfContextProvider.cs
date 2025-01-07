using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebShop.Models.Entity
{
    public class EfContextProvider : IDbContextProvider
    {
        public EfContextProvider(WebShopEntities context)
        {
            Context = context;
        }
        public WebShopEntities Context { set; get; }
        public DbContextTransaction DbTransaction { set; get; }

        public void Commit()
        {
            DbTransaction.Commit();
        }

        public void Rollback()
        {
            DbTransaction.Rollback();
        }

        public void BeginTransaction()
        {
            DbTransaction = Context.Database.BeginTransaction();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}