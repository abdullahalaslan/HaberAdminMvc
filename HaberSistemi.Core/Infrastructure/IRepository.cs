using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HaberSistemi.Core.Infrastructure
{
   public interface IRepository<T> where T:class
    {
        IEnumerable<T> GetAll();   //Tüm datamızı cekecek.

        T GetById(int id);    //Tek bir nesne donecek.

       

        T Get(Expression<Func<T, bool>> expression);

        IQueryable<T> GetMany(Expression<Func<T, bool>> expression);

        void Insert(T obj);

        void Update(T obj);

        void Delete(int id);

        int Count();

        void Save();


    }
}
