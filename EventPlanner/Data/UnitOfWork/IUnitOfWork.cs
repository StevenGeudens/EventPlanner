using EventPlanner.Areas.Identity.Data;
using EventPlanner.Data.Repository;
using EventPlanner.Models;
using System;
using System.Threading.Tasks;

namespace EventPlanner.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Category> CategoryRepo { get; }
        IRepository<Collaboration> CollaborationRepo { get; }
        IRepository<Event> EventRepo { get; }
        IRepository<Favorite> FavoriteRepo { get; }
        IRepository<Review> ReviewRepo { get; }
        IRepository<Status> StatusRepo { get; }
        IRepository<CustomUser> UserRepo { get; }

        int Save();
        Task<int> SaveAsync();
    }
}
