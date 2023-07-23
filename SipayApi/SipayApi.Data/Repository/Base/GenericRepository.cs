using Microsoft.EntityFrameworkCore;
using SipayApi.Base;
using System.Linq.Expressions;
using static Dapper.SqlMapper;

namespace SipayApi.Data.Repository;

public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : BaseModel
{
    private readonly SimDbContext dbContext;
    public GenericRepository(SimDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // Bu metot, değişikliklerin veritabanına kalıcı olarak kaydedilmesi için DbContext'teki SaveChanges() metodunu çağırır.
    public void Save()
    {
        dbContext.SaveChanges();
    }

    // Bu metot, belirli bir varlık(entity) örneğini veritabanından silmek için kullanılır.
    public void Delete(Entity entity)
    {
        dbContext.Set<Entity>().Remove(entity);
    }

    // Bu metot, belirli bir kimlik(id) değerine sahip varlığı veritabanından silmek için kullanılır.
    public void DeleteById(int id)
    {
        var entity = dbContext.Set<Entity>().Find(id);
        Delete(entity);
    }

    // Bu metot, tüm varlık örneklerini veritabanından almak için kullanılır.
    public List<Entity> GetAll()
    {
        return dbContext.Set<Entity>().ToList();
    }

    // Bu metot, tüm varlık örneklerini bir IQueryable nesnesi olarak veritabanından almak için kullanılır.
    public IQueryable<Entity> GetAllAsQueryable()
    {
        return dbContext.Set<Entity>().AsQueryable();
    }

    // Bu metot, belirli bir kimlik (id) değerine sahip varlığı veritabanından almak için kullanılır.
    public Entity GetById(int id)
    {
        var entity = dbContext.Set<Entity>().Find(id);
        return entity;
    }

    //  Bu metot, yeni bir varlık örneğini veritabanına eklemek için kullanılır. Ayrıca, InsertDate ve InsertUser gibi bazı alanlar için varsayılan değerler atanır.
    public void Insert(Entity entity)
    {
        entity.InsertDate = DateTime.UtcNow;
        entity.InsertUser = "simadmin@pay.com";
        dbContext.Set<Entity>().Add(entity);
    }

    // Bu metot, bir varlık örneğinin veritabanındaki karşılığını güncellemek için kullanılır.
    public void Update(Entity entity)
    {
        dbContext.Set<Entity>().Update(entity);
    }

    // Bu metot, belirli bir Lambda ifadesi ile veritabanındaki varlık örneklerini filtrelemek için kullanılır. Filtreleme işlemi,
    // Expression<Func<Entity, bool>> tipindeki bir parametre ile sağlanır.

    public IEnumerable<Entity> Where(Expression<Func<Entity, bool>> expression)
    {
        return dbContext.Set<Entity>().Where(expression).AsQueryable();
    }

    //public List<Entity> Where(Expression<Func<Entity, bool>> filter)
    //{
    //    return dbContext.Set<Entity>().Where(filter).ToList();
    //}
}