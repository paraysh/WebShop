using System.Data.Entity;

namespace WebShop.Models.Entity
{
    public interface IDbContextProvider
    {
        WebShopEntities Context { get; set; }
        DbContextTransaction DbTransaction { get; set; }
        void Commit();
        void Rollback();
        void BeginTransaction();
        void SaveChanges();
    }
}
