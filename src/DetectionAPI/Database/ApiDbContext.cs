using DetectionAPI.Database.Entities;
using System.Data.Common;
using System.Data.Entity;

namespace DetectionAPI.Database
{
    /// <summary>
    /// Класс контекста базы данных, служит для подключения к ней и осуществления операций над ее данными
    /// </summary>
    public class ApiDbContext : DbContext
    {
        /// <summary>
        /// Конструктор по умолчанию для первоначальной инициализации таблиц
        /// </summary>
        static ApiDbContext()
        {
            System.Data.Entity.Database.SetInitializer(new ApiDbContextInitializer());
        }

        /// <summary>
        /// Конструктор по умолчанию для стандартного подключения
        /// </summary>
        public ApiDbContext() : base("DetectionApiDatabase")
        {

        }

        /// <summary>
        /// Конструктор с выбором подключения
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="contextOwnsConnection"></param>
        public ApiDbContext(DbConnection connection, bool contextOwnsConnection = true)
            : base(connection, contextOwnsConnection)
        {

        }

        /// <summary>
        /// Ссылка на таблицу Users
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Ссылка на табилцу Sessions
        /// </summary>
        public DbSet<Session> Sessions { get; set; }

        /// <summary>
        /// Ссылка на таблицу Images
        /// </summary>
        public DbSet<ImageInfo> Images { get; set; }

        /// <summary>
        /// Конфигурирует отображение таблиц базы данных в объектно-реляционную модель
        /// </summary>
        /// <param name="modelBuilder">Построитель объектно-реляционной модели базы данных</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Configurations
                .Add(User.CreateConfiguration());

            modelBuilder
                .Configurations
                .Add(Session.CreateConfiguration());

            modelBuilder
                .Configurations
                .Add(ImageInfo.CreateConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
