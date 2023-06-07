using EventPlanner.Areas.Identity.Data;
using EventPlanner.Data.Repository;
using EventPlanner.Models;
using System.Threading.Tasks;

namespace EventPlanner.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        #region attributes
        private IRepository<Category> _categoryRepo;
        private IRepository<Collaboration> _collaborationRepo;
        private IRepository<Event> _eventsRepo;
        private IRepository<Favorite> _favoriteRepo;
        private IRepository<Review> _reviewRepo;
        private IRepository<Status> _statusRepo;
        private IRepository<CustomUser> _userRepo;
        #endregion

        protected EventPlannerContext Context;
        public UnitOfWork(EventPlannerContext ctx) { Context = ctx; }

        #region repositories
        public IRepository<Category> CategoryRepo
        {
            get 
            {
                if (_categoryRepo == null) _categoryRepo = new Repository<Category>(Context);
                return _categoryRepo;
            }
        }
        public IRepository<Collaboration> CollaborationRepo
        {
            get
            {
                if (_collaborationRepo == null) _collaborationRepo = new Repository<Collaboration>(Context);
                return _collaborationRepo;
            }
        }
        public IRepository<Event> EventRepo
        {
            get
            {
                if (_eventsRepo == null) _eventsRepo = new Repository<Event>(Context);
                return _eventsRepo;
            }
        }
        public IRepository<Favorite> FavoriteRepo
        {
            get
            {
                if (_favoriteRepo == null) _favoriteRepo = new Repository<Favorite>(Context);
                return _favoriteRepo;
            }
        }
        public IRepository<Review> ReviewRepo
        {
            get
            {
                if (_reviewRepo == null) _reviewRepo = new Repository<Review>(Context);
                return _reviewRepo;
            }
        }
        public IRepository<Status> StatusRepo
        {
            get
            {
                if (_statusRepo == null) _statusRepo = new Repository<Status>(Context);
                return _statusRepo;
            }
        }
        public IRepository<CustomUser> UserRepo
        {
            get
            {
                if (_userRepo == null) _userRepo = new Repository<CustomUser>(Context);
                return _userRepo;
            }
        }
        #endregion

        public void Dispose()
        {
            Context.Dispose();
        }

        public int Save()
        {
            return Context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return Context.SaveChangesAsync();
        }
    }
}
