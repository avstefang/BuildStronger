**Question:**

Ok, I don't understand anything of the Onion architecture as everyone explains the model differently. Currently I have the following:

Onion volgt het volgende design:
Infrastructuur/Presentatie
           ↓
   Applicatie/Service
           ↓
        Domein (Entities eg Athlete/Workout etc., Exceptions, Repositories/Interfaces)

Infrastructure/Presentation is the layer where everything is implemented/the GUI lives. But Application I don't understand at all and I also don't understand why I need to create interfaces at the domain level and then almost the same interface at the application level. I currently have my project cut in the following parts:
- API (ASP .NET Web API)
- Web (Blazor)
- MApp (Multiplatform App) (MAUI)
- Tests

As I see, the API, Web and MApp all use the Presentation layer and share the Application and Domain logic between each other.

**Answer:**

You’re very close already. The confusion usually comes from people mixing together:

* Onion Architecture
* Clean Architecture
* DDD
* CQRS
* Hexagonal/Ports & Adapters

…and then renaming layers differently.

Your current understanding is mostly correct. The missing piece is:

> The Application layer is where *use cases/business workflows* live.

Not the core business rules themselves — those belong in the Domain.

---

# The simplest mental model

Think of the layers like this:

```text
(UI / HTTP / Database / External APIs)
        ↓
Application (Use cases)
        ↓
Domain (Business rules)
```

Or in your case:

```text
API / Blazor / MAUI
        ↓
Application
        ↓
Domain
```

That is already a valid Onion Architecture.

---

# What each layer actually does

## 1. Domain layer → “What the business IS”

This is the center.

Contains:

* Entities
* Value Objects
* Domain Services
* Domain Rules
* Domain Exceptions
* Repository contracts/interfaces

Examples:

```csharp
Athlete
Workout
TrainingPlan
```

Example rule:

```csharp
public class Workout
{
    public void Complete()
    {
        if (IsCompleted)
            throw new WorkoutAlreadyCompletedException();

        IsCompleted = true;
    }
}
```

Notice:

* No database
* No ASP.NET
* No MAUI
* No EF Core
* No HTTP
* No UI

The domain should survive if all frameworks disappear.

---

# 2. Application layer → “What the system DOES”

This is the part people explain terribly.

Application is:

* orchestration
* use cases
* workflows
* coordination

It answers:

> “What happens when the user does X?”

Examples:

* Create workout
* Complete workout
* Register athlete
* Get dashboard
* Send notification after workout completion

---

## Example

```csharp
public class CompleteWorkoutService
{
    private readonly IWorkoutRepository _repo;

    public async Task Execute(Guid workoutId)
    {
        var workout = await _repo.Get(workoutId);

        workout.Complete();

        await _repo.Save(workout);
    }
}
```

This is NOT domain logic.

The domain logic is:

```csharp
workout.Complete();
```

The application layer:

* loads data
* calls domain methods
* persists changes
* coordinates external stuff

---

# 3. Infrastructure layer → “How things are implemented”

Contains:

* EF Core
* SQL
* File storage
* Email sending
* External APIs
* Authentication implementation

Example:

```csharp
public class EfWorkoutRepository : IWorkoutRepository
{
}
```

The Application layer depends on interfaces.

Infrastructure implements them.

---

# 4. Presentation layer → “How users interact”

Contains:

* ASP.NET Controllers
* Blazor pages
* MAUI UI
* REST endpoints
* ViewModels

This layer:

* receives user input
* calls Application layer
* returns output

---

# Your current structure

You said:

* API (ASP.NET Web API)
* Web (Blazor)
* MApp (MAUI)
* Tests

That maps VERY naturally to Onion.

You likely want something like:

```text
/src
    /Domain
    /Application
    /Infrastructure
    /API
    /Web
    /MApp
/tests
```

Where:

* API/Web/MApp are presentation layers
* Application is shared
* Domain is shared
* Infrastructure is shared

That is exactly the intended setup.

---

# The BIG confusion: interfaces

You asked:

> why do I need interfaces in Domain and then almost the same interface in Application?

Usually:

> You DON’T.

This is where tutorials overengineer things.

---

# The only interfaces you commonly need

Most applications only need:

## Domain interfaces

Like:

```csharp
public interface IWorkoutRepository
{
    Task<Workout?> Get(Guid id);
    Task Save(Workout workout);
}
```

Why here?

Because:

* the domain/application needs persistence
* but should not know EF Core

Infrastructure implements it:

```csharp
public class WorkoutRepository : IWorkoutRepository
{
}
```

---

# You usually do NOT need:

```text
Application.Interfaces.Services
Application.Interfaces.Repositories
Domain.Interfaces.Repositories
```

That duplication is often unnecessary ceremony.

---

# Practical architecture for your app

For your fitness app example:

## Domain

```text
Entities
- Athlete
- Workout
- Exercise

Rules
- Workout cannot be completed twice
- Athlete max active plans
- etc

Interfaces
- IAthleteRepository
- IWorkoutRepository
```

---

## Application

```text
Use cases/services
- CreateWorkout
- CompleteWorkout
- GetDashboard
- RegisterAthlete

DTOs
Commands
Queries
Validators
```

This layer:

* orchestrates
* uses repositories
* calls domain methods

---

## Infrastructure

```text
EF Core
DbContext
Repositories
Authentication
File storage
Push notifications
```

---

## API / Web / MAUI

These call Application services.

Example:

```csharp
await _completeWorkout.Execute(id);
```

That’s it.

---

# The dependency direction is the important part

The key Onion rule:

```text
Outer layers depend on inner layers.
Inner layers NEVER depend on outer layers.
```

Meaning:

✅ API → Application
✅ Application → Domain
✅ Infrastructure → Domain/Application

But NEVER:

❌ Domain → EF Core
❌ Domain → ASP.NET
❌ Domain → MAUI
❌ Application → Blazor

---

# Why this architecture is useful

Because now:

* MAUI can reuse business logic
* Blazor can reuse business logic
* API can reuse business logic
* tests can test domain without database
* you can swap EF Core later
* UI frameworks can change

The business stays intact.

---

# Important: don’t overengineer

A lot of Onion/Clean tutorials become enterprise theater.

You probably do NOT need:

* 50 interfaces
* MediatR everywhere
* CQRS for every action
* repository per entity + service per entity + manager per entity
* DTO explosion

A clean simple structure is enough.

---

# A very good practical rule

## Domain

Contains:

> “business truth”

## Application

Contains:

> “business actions/use cases”

## Infrastructure

Contains:

> “technical implementation”

## Presentation

Contains:

> “UI/HTTP interaction”

---

# Concrete example flow

User presses “Complete Workout” in MAUI:

```text
MAUI button
    ↓
Application service
    ↓
Repository loads Workout
    ↓
Workout.Complete() (domain rule)
    ↓
Repository saves Workout
    ↓
UI updated
```

That’s Onion Architecture in practice.

And honestly:
if you understand this flow, you already understand more than most tutorials explain clearly.

**Question:**

I still don't fully understand. The domain includes basic interfaces, ENUMs, classes to create basic objects like 
public class Athlete
{
int id;
Role role
string firstName
string lastName
etc etc etc....

Application layer I still don't quite understand because I don't know what orchestration, workflows or coordination are and don't know how to translate use cases into code so it's quite abstract.

Infrastructure I didn't quite understand that this is a separate layer from Presentation, I thought that these were one and the same. But as I understand it, these make sure that if I setup my API, I just have to call the implementation I setup in the Infrastructure layer, which means the API frontend is kept very light.

Presentation is where the API, Web and App live. They are designed with the following architectural models:

Blazor Web API gaat Model, Controller, Service gebruiken
Blazor Web App gaat MVC gebruiken
MAUI app gaat MVVM gebruiken

**Answer:**

You’re getting stuck at exactly the point where most explanations become too abstract. So let’s remove the buzzwords entirely and map this to actual code.

---

# First: your understanding of Domain is correct

Your Domain layer is basically:

```csharp
public class Athlete
{
    public int Id { get; set; }
    public Role Role { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

plus:

* enums
* business rules
* exceptions
* repository interfaces

Example:

```csharp
public enum Role
{
    Athlete,
    Coach,
    Admin
}
```

and:

```csharp
public interface IAthleteRepository
{
    Task<Athlete?> GetById(int id);
    Task Add(Athlete athlete);
}
```

This is all correct.

---

# The REAL purpose of the Application layer

Forget the words:

* orchestration
* workflows
* coordination

Replace them with:

> “What should happen when the user presses a button?”

THAT is the Application layer.

---

# Concrete example

Suppose in your app a user clicks:

```text
"Register Athlete"
```

Now ask yourself:

What should happen?

Maybe:

1. Validate input
2. Create Athlete object
3. Save athlete to database
4. Send welcome email
5. Return response

THAT ENTIRE FLOW is the Application layer.

---

# Domain vs Application

This distinction is the key.

---

## Domain = WHAT an Athlete IS

```csharp
public class Athlete
{
    public string FirstName { get; private set; }

    public Athlete(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new Exception("Name required");

        FirstName = firstName;
    }
}
```

This is business truth.

---

## Application = WHAT the system DOES

```csharp
public class RegisterAthleteService
{
    private readonly IAthleteRepository _repo;

    public async Task Register(string firstName)
    {
        var athlete = new Athlete(firstName);

        await _repo.Add(athlete);
    }
}
```

This is:

* not UI
* not database
* not domain

It is:

> “what happens when user registers athlete”

THAT is Application.

---

# Another example: Complete Workout

---

## Domain

```csharp
public class Workout
{
    public bool IsCompleted { get; private set; }

    public void Complete()
    {
        if (IsCompleted)
            throw new Exception("Already completed");

        IsCompleted = true;
    }
}
```

Business rule:

> workout cannot be completed twice

---

## Application

```csharp
public class CompleteWorkoutService
{
    private readonly IWorkoutRepository _repo;

    public async Task Execute(int workoutId)
    {
        var workout = await _repo.GetById(workoutId);

        workout.Complete();

        await _repo.Save(workout);
    }
}
```

This says:

> when user clicks complete workout:

* load workout
* execute business rule
* save result

That is the Application layer.

---

# VERY IMPORTANT:

# Application layer is NOT technical

This is why it confuses people.

It is NOT:

* SQL
* HTTP
* Controllers
* UI
* EF Core

It is:

> user/business actions

---

# Infrastructure vs Presentation

You discovered the exact thing many beginners misunderstand.

They are NOT the same.

---

# Infrastructure = technical implementations

Examples:

* EF Core repositories
* Authentication implementation
* Email sender
* File storage
* JWT handling
* External API calls

Example:

```csharp
public class AthleteRepository : IAthleteRepository
{
    private readonly AppDbContext _db;

    public async Task Add(Athlete athlete)
    {
        _db.Athletes.Add(athlete);

        await _db.SaveChangesAsync();
    }
}
```

This is Infrastructure.

Because:

* EF Core
* SQL
* database implementation

---

# Presentation = how user communicates with app

Examples:

* API Controllers
* Blazor Pages
* MAUI ViewModels

Presentation receives input.

---

# Example full flow

User presses button in MAUI:

```text
MAUI View
    ↓
ViewModel
    ↓
Application Service
    ↓
Repository Interface
    ↓
Infrastructure Repository
    ↓
Database
```

---

# Your understanding here is VERY good:

> if I setup my API, I just have to call the implementation I setup in the Infrastructure layer

YES.

Exactly.

Your API should stay very thin/light.

Example controller:

```csharp
[ApiController]
public class AthleteController : ControllerBase
{
    private readonly RegisterAthleteService _service;

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        await _service.Register(dto.FirstName);

        return Ok();
    }
}
```

Notice:

* no SQL
* no business rules
* no EF Core

The controller just forwards the request.

That is GOOD architecture.

---

# Your frontend architectures are separate concepts

You mentioned:

* Web API → Controller/Service
* Blazor → MVC
* MAUI → MVVM

These are UI architectural patterns.

They are INSIDE the Presentation layer.

Meaning:

```text
Presentation Layer
├── API (Controller pattern)
├── Blazor (MVC)
└── MAUI (MVVM)
```

These do NOT replace Onion Architecture.

They exist INSIDE it.

---

# VERY important simplification

You probably only need:

```text
Domain
Application
Infrastructure
Presentation
```

That’s enough.

Don’t create:

* Services folder everywhere
* Repositories everywhere
* Managers
* Handlers
* Providers
* Engines
* Factories

unless you truly need them.

---

# The easiest way to think about Application

# Domain:

> “What rules exist?”

# Application:

> “What action is happening?”

Examples:

* Register athlete
* Login
* Complete workout
* Add exercise
* Generate report

Every button/menu/API endpoint usually maps to one Application action.

---

# Your architecture probably should look like this

```text
/src

    /Domain
        Athlete.cs
        Workout.cs
        Role.cs
        IAthleteRepository.cs

    /Application
        RegisterAthleteService.cs
        CompleteWorkoutService.cs

    /Infrastructure
        AppDbContext.cs
        AthleteRepository.cs

    /API
        Controllers/

    /Web
        Pages/

    /MApp
        Views/
        ViewModels/
```

This is already a clean Onion Architecture.

And honestly:
you understand way more than you think already.

**Question:**

But what does happen when you do: await _repo.Save(workout);? Because this layer does not have anything to do with the database yet. So if you call Save(), it doesn't save anything yet and has to be implemented in the layer above right? So I understand that it's validations and button presses etc. But when there needs to be a database query, it needs the next presentation layer right? Or is this as preparation for the presentation layer? So that it checks if all the fields are filled in for example and then when no exception is thrown, the SQL code is called to save the Athlete to the database. The Infrastructure is where I implement the authentication for the API, and any files that need to be saved to disk.

**Answer:**

YES — now you’re hitting the actual core of Dependency Inversion in Onion Architecture.

And your confusion is completely logical because this is the hardest conceptual jump.

You asked:

> If Application does `await _repo.Save(workout)`, but Application doesn’t know databases… how does anything actually get saved?

The answer is:

> The Infrastructure layer secretly provides the real implementation at runtime.

This is done through **Dependency Injection**.

---

# The most important thing to understand

When you write:

```csharp id="p8q0e2"
private readonly IWorkoutRepository _repo;
```

Application only knows:

> “something exists that can save workouts”

It does NOT know:

* SQL
* EF Core
* PostgreSQL
* MySQL
* files
* APIs

Just:

> “I can save workouts somehow”

---

# The interface is just a contract

Inside Domain:

```csharp id="z0sqhq"
public interface IWorkoutRepository
{
    Task Save(Workout workout);
}
```

This says:

> “Anyone implementing this interface must provide Save().”

But it contains NO implementation.

---

# Infrastructure provides the REAL implementation

Example:

```csharp id="5fsv1q"
public class WorkoutRepository : IWorkoutRepository
{
    private readonly AppDbContext _db;

    public async Task Save(Workout workout)
    {
        _db.Workouts.Update(workout);

        await _db.SaveChangesAsync();
    }
}
```

THIS is where SQL/database work happens.

This is Infrastructure.

---

# Then Dependency Injection connects everything

In ASP.NET:

```csharp id="4j5l2k"
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
```

This line means:

```text id="zwn5f2"
Whenever someone asks for IWorkoutRepository,
give them WorkoutRepository.
```

---

# So what ACTUALLY happens

When this runs:

```csharp id="dyo2v5"
await _repo.Save(workout);
```

the runtime secretly does:

```csharp id="tkp8t9"
await new WorkoutRepository().Save(workout);
```

because Dependency Injection connected them.

---

# VERY important architecture insight

Application does NOT call “the next layer”.

This is the key misunderstanding.

Instead:

```text id="6o6m4m"
Infrastructure plugs INTO Application.
```

NOT the other way around.

---

# Onion dependency direction

```text id="7upfkn"
Presentation → Application → Domain
                 ↑
          Infrastructure
```

Infrastructure depends on Application/Domain.

Application does NOT depend on Infrastructure.

This is the entire point of Onion.

---

# Why this is powerful

Because now you can swap implementations.

Today:

```text id="0s0dn6"
IWorkoutRepository → SQL Server repository
```

Tomorrow:

```text id="h7g9v7"
IWorkoutRepository → MongoDB repository
```

or:

```text id="q8lpr2"
IWorkoutRepository → InMemory repository for testing
```

Application code NEVER changes.

---

# VERY important:

# Validation placement

You asked:

> So this is preparation for the presentation layer? So that it checks fields etc?

Partly yes, but validation exists in MULTIPLE places.

---

# 1. Presentation validation (UI validation)

Example:

* required textbox
* email format
* password length

This is convenience validation.

Example in Blazor:

```csharp id="v3ab0w"
[Required]
public string FirstName { get; set; }
```

---

# 2. Application validation

Checks:

* request correctness
* permissions
* workflow rules

Example:

```csharp id="zz6q1u"
if (request.WorkoutId <= 0)
    throw new Exception();
```

---

# 3. Domain validation (MOST IMPORTANT)

True business rules.

Example:

```csharp id="lhc8h0"
if (IsCompleted)
    throw new WorkoutAlreadyCompletedException();
```

This rule MUST ALWAYS exist.

Even if:

* API changes
* UI changes
* MAUI changes

The business rule survives.

---

# Presentation layer should stay thin

You are absolutely correct.

A controller should mostly do this:

```csharp id="bmz5tv"
[HttpPost]
public async Task<IActionResult> CompleteWorkout(int id)
{
    await _service.Execute(id);

    return Ok();
}
```

That’s good architecture.

---

# Your understanding of Infrastructure is now correct

YES, Infrastructure contains:

* EF Core
* database implementations
* authentication implementation
* JWT generation
* file storage
* external APIs
* email sending
* disk access

Examples:

```text id="30t4u6"
Infrastructure
├── Persistence
├── Authentication
├── FileStorage
├── Email
└── ExternalServices
```

---

# One thing to correct

You said:

> when there needs to be a database query, it needs the next presentation layer right?

No.

Presentation should NEVER contain SQL/database code.

Presentation only:

* receives input
* displays output

The database access belongs ONLY in Infrastructure.

---

# Full real-world request flow

Suppose:

User clicks button in MAUI:

```text id="g4n5eq"
[MAUI View]
     ↓
[ViewModel]
     ↓
[Application Service]
     ↓
[IWorkoutRepository]
     ↓
[Infrastructure Repository]
     ↓
[EF Core]
     ↓
[SQL Database]
```

Then back up again.

---

# The hidden magic = Dependency Injection

Without DI, Onion Architecture feels impossible.

With DI, it suddenly makes sense.

Application says:

> “I need SOMETHING that can save workouts.”

Infrastructure says:

> “I implement that.”

ASP.NET connects them automatically.

---

# The easiest practical rule for you

## Domain

Pure business objects/rules

## Application

Use cases/actions

## Infrastructure

Technical implementations

## Presentation

UI/API interaction

That’s really all Onion Architecture is.

**Question:**

But doesn't the private readonly IWorkoutRepository _repo; defeat the purpose of the layer underneath can't know about the layer above? Because it seems to know all too well that the workout repository exists.... and ohhh, this is the Interface for WorkoutRepository right? So the WorkoutRepository isn't actually implemented yet here. But then it does actually need the actual implementation, not only the interface for it to actually work eventually right? Because IWorkoutRepository doesn't actually do anything yet. Ohhh, you mean to say that the dependency injection is making sure that the actual implementation does exist at this level instead of having only the not-implemented interface to work with. It's hard to create a full-blown project for school when you only know fragments and understand the basics because you've never extensively worked with them before...

**Answer:**

YES. You just made the conceptual leap.

This sentence:

> “Ohhh, this is the Interface for WorkoutRepository right?”

is the exact moment Onion Architecture starts making sense.

And this:

> “Dependency injection is making sure the actual implementation exists”

is also correct.

You are now understanding the architecture correctly.

---

# The crucial distinction

Application does NOT know about:

```csharp id="6xv5t2"
WorkoutRepository
```

It ONLY knows about:

```csharp id="3fj73x"
IWorkoutRepository
```

That distinction is EVERYTHING.

---

# Why this does NOT break Onion rules

You asked:

> Doesn't this defeat the purpose because the lower layer knows about the upper layer?

No, because:

```text id="53s6yu"
Application knows only the CONTRACT.
```

NOT the implementation.

---

# Think of it like a wall socket

Your phone charger knows:

```text id="u4uk6f"
"There is electricity available through this shape of connector."
```

It does NOT know:

* where electricity comes from
* coal
* solar
* nuclear
* wind

The interface is the socket shape.

The implementation is the power plant.

---

# In code terms

Application layer:

```csharp id="lqv20t"
public interface IWorkoutRepository
{
    Task Save(Workout workout);
}
```

This only says:

> “Something must exist that can save workouts.”

It does NOT say:

* SQL Server
* EF Core
* MongoDB
* File storage

Nothing technical.

---

# Infrastructure says:

```csharp id="1w2jup"
public class WorkoutRepository : IWorkoutRepository
{
    public async Task Save(Workout workout)
    {
        await _db.SaveChangesAsync();
    }
}
```

Infrastructure is volunteering:

```text id="l6zy3w"
"I can fulfill that contract."
```

---

# THIS is the dependency direction

Application owns the interface.

Infrastructure depends on Application.

NOT the other way around.

This is subtle but extremely important.

---

# Wrong direction (bad)

```text id="5gclo5"
Application
    ↓
Infrastructure
```

Example:

```csharp id="n8fhjc"
using Infrastructure.Persistence;
```

BAD.

Because now Application is tightly coupled to Infrastructure.

---

# Correct direction

```text id="nd4w8i"
Infrastructure
    ↓
Application
```

Infrastructure says:

```text id="9v3v0s"
"I implement your interface."
```

This keeps the core independent.

---

# You ALSO correctly realized:

> “The interface itself does nothing.”

YES.

Exactly.

An interface is ONLY a contract.

This:

```csharp id="0a2d4m"
public interface IWorkoutRepository
{
    Task Save(Workout workout);
}
```

contains ZERO logic.

No SQL.
No database.
No code execution.

By itself, it is useless.

---

# So how does it work?

Dependency Injection inserts the real implementation.

At runtime, ASP.NET does something conceptually like:

```csharp id="nqkq7w"
IWorkoutRepository repo = new WorkoutRepository();
```

and injects it into:

```csharp id="zbdb5t"
private readonly IWorkoutRepository _repo;
```

So when Application calls:

```csharp id="2cxmjc"
await _repo.Save(workout);
```

it ACTUALLY executes:

```csharp id="5ep85j"
WorkoutRepository.Save(workout)
```

behind the scenes.

---

# This is why Dependency Injection exists

Without DI, Onion Architecture becomes painful.

DI is what allows:

* abstraction in inner layers
* implementation in outer layers

without coupling them together.

---

# The thing you're struggling with is NORMAL

You said:

> “It's hard to create a full-blown project when you only know fragments.”

That is exactly how architecture feels at first.

Because architecture is:

* mostly invisible
* mostly abstract
* only understandable after seeing full flows

Most tutorials fail because they explain:

* theory
  before
* request flow

But software architecture only clicks when you trace:

* button click
  → service
  → repository
  → database

---

# The FULL mental model

Here is the whole thing simplified:

---

# 1. Domain defines needs

```csharp id="v75jol"
public interface IWorkoutRepository
{
    Task Save(Workout workout);
}
```

Meaning:

```text id="j7n3q9"
"I need a way to save workouts."
```

---

# 2. Application uses the need

```csharp id="39wb91"
await _repo.Save(workout);
```

Meaning:

```text id="yeb20q"
"Please save this workout."
```

Application still does NOT know HOW.

---

# 3. Infrastructure fulfills the need

```csharp id="f5w6n2"
public class WorkoutRepository : IWorkoutRepository
{
}
```

Meaning:

```text id="93ewxw"
"I know how to save workouts using SQL."
```

---

# 4. Dependency Injection connects them

```csharp id="4y0w03"
builder.Services.AddScoped<
    IWorkoutRepository,
    WorkoutRepository>();
```

Meaning:

```text id="zt22z4"
"When someone asks for IWorkoutRepository,
give them WorkoutRepository."
```

---

# 5. Presentation starts everything

Controller:

```csharp id="n8iw1d"
await _service.Execute(id);
```

And the chain begins.

---

# You are MUCH closer than you think

The fact that you now understand:

* interface vs implementation
* dependency direction
* DI purpose
* why Application does not know SQL

means you already passed the hardest conceptual barrier.

Most students fake understanding here for months.

You actually reasoned through it correctly.

**Question:**

I can make a general CRUD interface right? Because literally every entity needs to be able to do CRUD and build on top when needed. I did so in my last project, but there it was a bit messy, so I was wondering if that would work correctly in this setup. Because OOP is preventing reuse of the code, so CRUD is I think an excellent example of that.

**Answer:**

Yes — you absolutely *can* create a generic CRUD repository interface.

And for many school/business applications, that is completely reasonable.

Something like:

```csharp id="t8d5sw"
public interface IRepository<T>
{
    Task<T?> GetById(int id);

    Task<IEnumerable<T>> GetAll();

    Task Add(T entity);

    Task Update(T entity);

    Task Delete(T entity);
}
```

is very common.

Then:

```csharp id="qqqv45"
public interface IAthleteRepository : IRepository<Athlete>
{
}
```

This is perfectly valid Onion Architecture.

---

# BUT — there is an important nuance

You said:

> “Because literally every entity needs CRUD”

This is where architecture becomes practical instead of theoretical.

Technically:

* yes, many entities need CRUD
* but business operations are often NOT just CRUD

Example:

```text id="w0ys77"
Workout.Complete()
Athlete.AssignCoach()
TrainingPlan.Publish()
```

These are business actions.

Not generic CRUD anymore.

---

# So the common balance is:

## Generic repository for basic persistence

```csharp id="f9h37n"
IRepository<T>
```

for:

* Get
* Add
* Delete
* Update

---

## Specific repositories when needed

Example:

```csharp id="l2h57g"
public interface IWorkoutRepository
{
    Task<IEnumerable<Workout>> GetIncompleteWorkoutsForAthlete(int athleteId);
}
```

because generic CRUD cannot express business-specific queries well.

---

# Important:

# OOP is NOT preventing reuse

Actually:

> generic repositories ARE OOP reuse.

You’re using:

* abstraction
* inheritance
* polymorphism
* generic types

That IS object-oriented reuse.

---

# Why some people dislike generic repositories

You may see tutorials saying:

```text id="j84gpd"
"Generic repositories are bad!"
```

Usually because:

* they become too abstract
* they hide database capabilities
* they turn everything into CRUD-only thinking

But for:

* school projects
* medium business apps
* standard CRUD systems

they are often completely fine.

---

# The REAL danger is over-abstraction

THIS becomes messy:

```text id="x6o8i8"
IBaseRepository
IGenericRepository
IAsyncRepository
IReadOnlyRepository
IEntityRepository
ICrudRepository
```

with five inheritance layers.

That’s enterprise nonsense most of the time.

---

# A clean practical setup

This is a very reasonable structure for your project:

---

## Domain

```csharp id="0n6wfi"
public interface IRepository<T>
{
    Task<T?> GetById(int id);

    Task<IEnumerable<T>> GetAll();

    Task Add(T entity);

    Task Update(T entity);

    Task Delete(T entity);
}
```

---

## Specific repository if needed

```csharp id="kq3h9m"
public interface IWorkoutRepository : IRepository<Workout>
{
    Task<IEnumerable<Workout>>
        GetCompletedWorkoutsForAthlete(int athleteId);
}
```

---

## Infrastructure

```csharp id="y4px3k"
public class Repository<T> : IRepository<T>
{
    protected readonly AppDbContext _db;
}
```

and:

```csharp id="nsgg8d"
public class WorkoutRepository
    : Repository<Workout>,
      IWorkoutRepository
{
}
```

Very common.
Very clean.
Very acceptable.

---

# One VERY important thing

Do NOT put business logic into repositories.

Bad:

```csharp id="m25jqs"
repository.CompleteWorkout()
```

Good:

```csharp id="q31yow"
workout.Complete()
```

Then save it through repository.

Repositories should mainly:

* load entities
* save entities
* query entities

Business rules belong in Domain/Application.

---

# Another important insight

Many people confuse:

* CRUD architecture
  with
* Domain architecture

Simple apps:

```text id="44jr6w"
Create
Read
Update
Delete
```

Complex apps:

```text id="b5d2hn"
ApproveInvoice
PublishWorkout
CompleteTraining
AssignCoach
```

Those are workflows/use-cases.

Your app likely has BOTH.

That’s normal.

---

# My recommendation for your school project

Keep it simple.

You probably do NOT need:

* CQRS
* MediatR
* 25 repository patterns
* DDD aggregates everywhere

A clean structure like this is already very solid:

```text id="1x8k09"
Domain
├── Entities
├── Enums
├── Exceptions
└── IRepository<T>

Application
├── Services
└── UseCases

Infrastructure
├── EF Core
├── Repositories
└── Authentication

Presentation
├── API
├── Web
└── MAUI
```

That is more than enough for a strong architecture.

**Question:**

No, I'm not saying that business logic is not needed anymore then. But they are build on top of the CRUD. So essentially something like: public IWorkoutRepository : IRepository<Workout>
{
    Task<bool> SetWorkoutComplete(int id);
}

**Answer:**

YES — that is a very common and sensible approach.

You’re now thinking about the architecture correctly.

Your idea:

```csharp id="5l3f9u"
public interface IWorkoutRepository : IRepository<Workout>
{
    Task<bool> SetWorkoutComplete(int id);
}
```

is structurally valid.

But there is one important architectural refinement:

> `SetWorkoutComplete()` is probably business logic, not repository logic.

So most Onion/Clean Architecture approaches would prefer:

```csharp id="mv42qb"
var workout = await _repo.GetById(id);

workout.Complete();

await _repo.Update(workout);
```

instead of:

```csharp id="5f1h0s"
await _repo.SetWorkoutComplete(id);
```

---

# Why?

Because this version:

```csharp id="v30sqz"
SetWorkoutComplete()
```

starts pushing business behavior into the repository/database layer.

And then eventually repositories become:

```text id="rskn0y"
ApproveInvoice()
PublishWorkout()
BanUser()
CompleteWorkout()
```

which mixes:

* persistence
  with
* business actions

---

# The cleaner separation is:

## Repository

Handles:

```text id="tt67ho"
load/save/query data
```

---

## Domain Entity

Handles:

```text id="5q1d1z"
business behavior/rules
```

---

# So ideally

---

## Domain

```csharp id="t5c96q"
public class Workout
{
    public bool IsCompleted { get; private set; }

    public void Complete()
    {
        if (IsCompleted)
            throw new Exception("Already completed");

        IsCompleted = true;
    }
}
```

---

## Repository

```csharp id="7qkk4g"
public interface IWorkoutRepository
    : IRepository<Workout>
{
}
```

---

## Application

```csharp id="ifnzzf"
public async Task CompleteWorkout(int id)
{
    var workout = await _repo.GetById(id);

    workout.Complete();

    await _repo.Update(workout);
}
```

That is the “clean” Onion approach.

---

# HOWEVER

And this is important:

For a school project?

Your approach is NOT wrong.

This:

```csharp id="2f5vs2"
Task<bool> SetWorkoutComplete(int id);
```

is completely acceptable if:

* it keeps your code understandable
* your project remains maintainable
* you understand the tradeoff

Architecture is about tradeoffs, not religion.

---

# The practical middle ground

A very common real-world compromise is:

---

# Generic CRUD repository

```csharp id="7b5j5s"
IRepository<T>
```

for standard operations.

---

# Specialized query methods

Repositories often contain:

* filtering
* searching
* loading related data

Example:

```csharp id="g34u8o"
Task<IEnumerable<Workout>>
    GetIncompleteWorkoutsForAthlete(int athleteId);
```

That’s perfectly normal repository responsibility.

---

# Business actions stay in Domain/Application

Example:

```csharp id="32k3ui"
workout.Complete();
```

instead of:

```csharp id="2edcma"
repo.SetWorkoutComplete();
```

---

# Easy rule to remember

If the method sounds like:

```text id="xw7ymu"
"change business state"
```

it probably belongs in:

* Domain
  or
* Application

---

If the method sounds like:

```text id="9uw0fk"
"retrieve/store data"
```

it probably belongs in:

* Repository

---

# Examples

## Repository responsibilities

```csharp id="t4n27t"
GetById()
GetAll()
Search()
FindByEmail()
GetWorkoutsForAthlete()
Save()
Update()
Delete()
```

---

## Domain/Application responsibilities

```csharp id="ozd80w"
CompleteWorkout()
AssignCoach()
PublishTrainingPlan()
BanAthlete()
ApproveInvoice()
```

---

# Your instinct about CRUD is correct

You recognized something important:

> CRUD code repeats everywhere.

That’s exactly why generic repositories exist.

And using:

```csharp id="2fd7r5"
IRepository<T>
```

is absolutely reasonable in Onion Architecture.

**Question:**

public class Workout {}
I see that this is not an Interface. So that's also allowed. But if not an interface (Repository), what is this? And what does the folder need to be called?

**Answer:**

Ahhh, now I understand your question.

You mean this:

```csharp id="hmh43x"
public class Workout
{
    public bool IsCompleted { get; private set; }

    public void Complete()
    {
        if (IsCompleted)
            throw new Exception("Already completed");

        IsCompleted = true;
    }
}
```

and you’re asking:

> “If this is not a Repository and not an Interface, what IS it exactly?”

This is a:

# Domain Entity

(or simply: Entity)

---

# What is an Entity?

An Entity is:

* a business object
* with identity
* and behavior

Example:

```csharp id="uyg8si"
Workout
Athlete
Exercise
TrainingPlan
```

These are the actual “things” in your business domain.

---

# VERY important insight

A lot of beginners think entities are only:

```csharp id="pb6f9w"
public class Athlete
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

just data containers.

But in proper OOP / Domain Modeling:

> Entities ALSO contain behavior.

So this is GOOD:

```csharp id="4zyqg9"
workout.Complete();
```

instead of:

```csharp id="djixn6"
workout.IsCompleted = true;
```

because the entity protects its own rules.

---

# Folder naming

Usually:

```text id="jjlwmr"
Domain
├── Entities
├── Enums
├── Exceptions
├── Interfaces
└── ValueObjects
```

So:

```text id="pfzq4l"
/Domain/Entities/Workout.cs
```

is very standard.

---

# Difference between Entity vs Repository

---

# Entity

Represents:

> the business object itself

Example:

```csharp id="78m5zw"
Workout
Athlete
Exercise
```

Contains:

* data
* business behavior
* rules

---

# Repository

Represents:

> how entities are stored/retrieved

Example:

```csharp id="om58k7"
IWorkoutRepository
WorkoutRepository
```

Contains:

* database queries
* persistence logic

---

# Simplified analogy

Think of:

```text id="0j80n4"
Workout
```

as:

> the car

and:

```text id="zyzrt0"
WorkoutRepository
```

as:

> the garage/storage system

The garage is NOT the car.

---

# Important OOP concept you just discovered

You noticed:

> “Wait… methods can exist directly on entities?”

YES.

That is actually proper Object-Oriented Programming.

A LOT of modern codebases accidentally become:

```text id="7ul3dv"
objects = data only
services = all logic
```

which is called:

* Anemic Domain Model

Example of bad/anemic model:

```csharp id="f5pc0k"
public class Workout
{
    public bool IsCompleted { get; set; }
}
```

Then somewhere else:

```csharp id="69l8de"
public class WorkoutService
{
    public void CompleteWorkout(Workout workout)
    {
        if (workout.IsCompleted)
            throw new Exception();

        workout.IsCompleted = true;
    }
}
```

Notice:

* Workout itself protects nothing
* all logic is outside

That’s less object-oriented.

---

# Better model

```csharp id="3kq2b5"
public class Workout
{
    public bool IsCompleted { get; private set; }

    public void Complete()
    {
        if (IsCompleted)
            throw new Exception();

        IsCompleted = true;
    }
}
```

Now:

* Workout protects itself
* invalid states are harder
* rules stay centralized

THIS is why Domain Entities can contain methods.

---

# So your project structure becomes

```text id="m4knfp"
Domain
├── Entities
│   ├── Athlete.cs
│   ├── Workout.cs
│   └── Exercise.cs
│
├── Interfaces
│   ├── IRepository.cs
│   └── IWorkoutRepository.cs
│
├── Enums
│   └── Role.cs
│
└── Exceptions
```

This is very clean.

**Question:**

public bool IsCompleted { get; private set; }
But does this need to be private _isCompleted, or still public IsCompleted? Because now it's still allowed to be overwritten externally. So does there also need to be a method called UnComplete() or something stupid?

**Answer:**

Good catch — this is exactly the kind of question that leads into proper encapsulation in OOP.

This:

```csharp id="cc4jri"
public bool IsCompleted { get; private set; }
```

already prevents external modification.

Because:

* `public get`
  → everyone can READ it

* `private set`
  → only the `Workout` class itself can CHANGE it

So this is allowed:

```csharp id="1pzxv3"
bool done = workout.IsCompleted;
```

But this is NOT allowed outside the class:

```csharp id="n74mws"
workout.IsCompleted = true;
```

The compiler will reject it.

---

# Why this is good

It forces people to use:

```csharp id="u1c4xy"
workout.Complete();
```

instead of directly changing state.

That means:

* rules are enforced
* validation always runs
* invalid states become harder

---

# Should it instead be `_isCompleted`?

Usually BOTH exist conceptually:

```csharp id="2mjlwm"
private bool _isCompleted;

public bool IsCompleted => _isCompleted;
```

But in modern C#, auto-properties are cleaner:

```csharp id="xq6zw7"
public bool IsCompleted { get; private set; }
```

This is essentially shorthand for the above.

Most C# developers prefer the auto-property version unless extra logic is needed.

---

# About `UnComplete()`

You asked:

> “Do I now need an UnComplete() method?”

Only if the BUSINESS actually allows that.

That’s the key mindset shift.

---

# Domain entities model REAL business rules

Ask:

```text id="95b0b9"
Can a completed workout become incomplete again?
```

If:

* YES → add `UnComplete()`
* NO → don’t

Architecture follows business behavior.

Not technical symmetry.

---

# Example where it SHOULD exist

Imagine a draft article:

```text id="rx57a6"
Draft → Published → Draft again
```

Then:

```csharp id="1yzwsf"
Publish()
Unpublish()
```

makes sense.

---

# Example where it SHOULD NOT exist

A payment:

```text id="e7q5h2"
Paid
```

Usually payments are not “unpaid”.

Instead:

* Refund()
* Cancel()

Those are different business concepts.

---

# So for Workout

This is perfectly fine:

```csharp id="4q5x2s"
public class Workout
{
    public bool IsCompleted { get; private set; }

    public void Complete()
    {
        if (IsCompleted)
            throw new Exception("Already completed");

        IsCompleted = true;
    }
}
```

No `UnComplete()` needed unless your app genuinely requires it.

---

# The BIG OOP principle here

You are moving from:

```text id="z1vgzv"
objects as data containers
```

to:

```text id="9ynx0s"
objects protecting their own state
```

That is proper encapsulation.

And honestly, the fact that you’re asking these questions means you’re starting to understand the architectural reasoning instead of just copying patterns mechanically.