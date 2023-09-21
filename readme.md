[![.github/workflows/publish-nuget-core.yml](https://github.com/daniel18acevedo/cq-unit-of-work/actions/workflows/publish-nuget-core.yml/badge.svg)](https://github.com/daniel18acevedo/cq-unit-of-work/actions/workflows/publish-nuget-core.yml)


This is a project to reduce the devoplment time when dealing with ORMs and basic repositories.

## Description of each package and goals:

### CQ.UnitOfWork

Is the package that has the core logic to develop a simple way of getting entity repositories, or specific type repositories in run time. This logic implements the abstractions define in **CQ.UnitOfWork.Abstractions**.

The idea of this separation is that only the project where dependency injection happens reference to this package. So only the projects interested in this functionallity should reference to **CQ.UnitOfWork.Abstractions**. This way the continues growing and updates only happens on the injection.

Here we are gonna find the extension **AddUnitOfWork** that injects the **UnitOfWorkService** in the collector to be used in other parts the solution.

---

### CQ.UnitOfWork.Abstractions

Is the package that defines the behaviour we are gonna be able to use in order to fetch the proper repository in run time. This package should be referenced by all projects which are interested in use a specific or entity repository.

Also exports an abstraction of the ORMs context to be used to perform basic common behaviour, such as, knowing if the connection to the database is stablished or not.

---

### CQ.UnitOfWork.EFCore

Is the package that has the core logic to deal with **EntityFramworkCore**. Here we are gonna find a basic efcore repository that solve us the dependencies to an **efcore context**, so we can focus only on developing specific business rules with **efcore**. Is not fully necessary to extend **EFCoreRepository**, only if we want to create new functions that use this specific ORM.

It also expose an abstraction of **DbContext** named **EFCoreContext**, which expose a simpler way of connecting to a database with this orm.

Here we are gonna find a series of extensions such as **AddEfCoreContext**, **AddEfCoreRepository**, which allow us to register the specific context, entity repositories or type repositories.

This package should be referenced by the project where dependency injection happend.

---

### CQ.UnitOfWork.EFCore.Abstractions

Is the package that defines the beahviour we are gonna be able to use for a specific **efcore repository**. This behaviour extends the behaviour of **CQ.UnitOfWork.Abstractions**.

This package should be referenced by the projects where we are interested in using efcore repositories.

---

### CQ.UnitOfWork.MongoDriver

Is the package that has the core logic to deal with **Mongo.Driver**. Here we are gonna find a basic **mongo-driver repository** that solve us the dependencies to an **mongo-driver context**, so we can focus only on developing specific business rules with **mongo-driver**. Is not fully necessary to extend **MongoDriverRepository**, only if we want to create new functions that use this specific ORM.

It also expose an abstraction of **IMongoDatabase** and **MongoClient** named **MongoDriverContext**, which expose a simpler way of connecting to a database with this orm.

Here we are gonna find a series of extensions such as **AddMongoContext**, **AddMongoRepository**, which allow us to register the specific context, entity repositories or type repositories.

This package should be referenced by the project where dependency injection happend.

---

## Register dependencies

### In project where dependency registration happens
In this project it should reference to **CQ.UnitOfWork**, and the specific ORM package to used. Once we reference to this package, we are gonna be able to register the **UnitOfWorkService** like this:
    
    //Default life cycle for UnitOfWork will be LifeCycle.SCOPED
    services.AddUnitOfWork();

    //Singleton life cycle for UnitOfWork
    services.AddUnitOfWork(LifeCycle.SINGLETON);

    //Transient life cycle for UnitOfWork
    services.AddUnitOfWork(LifeCycle.TRANSIENT);

### For EFCore
It should reference also to **CQ.UnitOfWork.EFCore**. Once we have reference to this package we are gonna be able to register the EFCore context and repositories for this ORM. 

    // For registering a efcore context we neet to implement a concrete context that heritence from EFCoreContext
    public class ConcreteContext : EFCoreContext
    {
      public DbSet<User> Users { get; set; }

      public DbSet<Book> Books { get; set; }
      
      public ConcreteContext(DbContextOptions options) : base(options) { }
    }
---
    // Then we can do the following
    services.AddEFCoreContext<ConcreteContext>(new EFCoreConfig{
      DatabaseConnection = new DatabaseConnection{
        ConnectionString = "your-connection-string",
        DatabaseName = "your-database-name"
      },
      DatabaseEngine = EFCoreDatabaseEngine.SQL //Default is SQL, another available option is SQL_LITE
    })

    //For registering entity repositories with SCOPED life time as default
    services.AddEfCoreRepository<User>();
    services.AddEfCoreRepository<Book>();

### For MongoDriver
It should reference also to **CQ.UnitOfWork.MongoDriver**. Once we have reference to this package we are gonna be able to register the MongoDriver context and repositories for this ORM. 

    // Then we can do the following
    services.AddMongoContext(new MongoDriverConfig{
      DatabaseConnection = new DatabaseConnection{
        ConnectionString = "your-connection-string",
        DatabaseName = "your-database-name"
      }
    })

    //For registering entity repositories with SCOPED life time as default
    services.AddMongoRepository<User>(); //The default collection name will be Users
    services.AddMongoRepository<Book>("books"); //The name of the collection will be books instead the default value Books

## Usage
### In projects where repositories are gonna be used
This projects should reference to **CQ.UnitOfWork.Abstractions**, and in case of necessary to have more ORM specific functions, reference to the proper package. For more EFCore functions, reference to **CQ.UnitOfWork.EFCore.Abstractiosn** and for mongo-driver specific functions, reference to **CQ.UnitOfWork.MongoDriver.Abstractions**.

    public class MyLogic
    {
      //Generic repository
      private readonly IRepository<User> _userRepository;

      //EFCore specific repository
      private readonly IEFCoreRepository<User> _userEfCoreRepository;

      //MongoDriver specific repository
      private readonly IMongoDriverRepository<User> _userMongoDriverRepository;

      public MyLogic(IUnitOfWork unitOfWork)
      {
        this._userRepository = unitOfWork.GetEntityRepository<User>();
        this._userEfCoreRepository = unitOfWork.GetRepository<IEFCoreRepository<User>>();
        this._userMongoDriverRepository = unitOfWork.GetRepository<IMongoDriverRepository<User>>();
      }
    }

Not necessary to use three repositories, this is a simple example to show how to call different repositories