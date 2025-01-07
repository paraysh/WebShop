using System.Data.Entity;

namespace WebShop.Models.Entity
{
    public partial class WebShopEntities
    {
        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}