//Bearbeiter: Yusuf Can Sönmez
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Tests.Utilities
{
    // Diese Klasse stellt eine gefälschte DbSet-Implementierung für Unit-Tests bereit.
    public class FakeDbSet<T> : DbSet<T>, IDbSet<T> where T : class
    {
        // Interne Liste zur Speicherung der Daten.
        List<T> _data;

        // Konstruktor, der die interne Liste initialisiert.
        public FakeDbSet()
        {
            _data = new List<T>();
        }

        // Methode zum Finden eines Elements anhand von Schlüsselwerten (nicht implementiert).
        public override T Find(params object[] keyValues)
        {
            throw new NotImplementedException("Derive from FakeDbSet<T> and override Find");
        }

        // Methode zum Hinzufügen eines Elements zur internen Liste.
        public override T Add(T item)
        {
            _data.Add(item);
            return item;
        }

        // Methode zum Entfernen eines Elements aus der internen Liste.
        public override T Remove(T item)
        {
            _data.Remove(item);
            return item;
        }

        // Methode zum Anhängen eines Elements (nicht implementiert).
        public override T Attach(T item)
        {
            return null;
        }

        // Methode zum Loslösen eines Elements aus der internen Liste.
        public T Detach(T item)
        {
            _data.Remove(item);
            return item;
        }

        // Methode zum Erstellen eines neuen Elements.
        public override T Create()
        {
            return Activator.CreateInstance<T>();
        }

        // Methode zum Erstellen eines neuen abgeleiteten Elements.
        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        // Eigenschaft, die die lokale Liste der Elemente zurückgibt.
        public List<T> Local
        {
            get { return _data; }
        }

        // Methode zum Hinzufügen einer Reihe von Elementen zur internen Liste.
        public override IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _data.AddRange(entities);
            return _data;
        }

        // Methode zum Entfernen einer Reihe von Elementen aus der internen Liste.
        public override IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            for (int i = entities.Count() - 1; i >= 0; i--)
            {
                T entity = entities.ElementAt(i);
                if (_data.Contains(entity))
                {
                    Remove(entity);
                }
            }

            return this;
        }

        // Implementierung der IQueryable-Schnittstelle.
        Type IQueryable.ElementType
        {
            get { return _data.AsQueryable().ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return _data.AsQueryable().Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _data.AsQueryable().Provider; }
        }

        // Implementierung der IEnumerable-Schnittstelle.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}