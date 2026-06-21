# Architectuur

Blazor Web API gaat Model, Controller, Service gebruiken
Blazor Web App gaat MVC gebruiken
MAUI app gaat MVVM gebruiken


Onion gaat gebruikt worden voor de project solution. Zo kan het project op een efficiënte manier gemaakt worden zonder concerns door elkaar te halen. In elk geval worden de volgende projecten binnen de solution gemaakt:
- API (ASP .NET Web API)
- Web (Blazor)
- MApp (Multiplatform App) (MAUI)
- Tests
- Domein
- Application


https://marcoatschaefer.medium.com/onion-architecture-explained-building-maintainable-software-54996ff8e464

Onion volgt het volgende design:
       Presentatie (fontend, API, Web, App)
           ↓
     Infrastructuur (technische implementatie, Email, SQL, authenticatie (JWT), IO, externe API)
           ↓
   Applicatie/Service (wat moet er gebeuren als een gebruiker een interactie heeft met de gebruikersinterface/presentatie, services)
           ↓
        Domein (Entities eg Athlete/Workout etc., Exceptions, Repositories/Interfaces, ENUM)

## Project structure
```
BuildStronger
├─ API
│   ├ Controller
│   ├ Model
│   ├ Service
│   └ Resource (probably not needed, add when discovered it's needed)
├─ Application - !!!! does not implement any IO/SQL operations, only the Infrastructure layer can do this
│   ├ DTO (Data Transfer Object is an object that will be transferred between different layers, these are essentially the same as Entities, but differ in the fact that it's an application specific object and cannot exist in the real world without computers, it just contains data and no logic, !!! only use these once so not between use cases as this would couple them, transfer data between layers without exposing Domain entities directly, unlike Entities, DTOs contain no business behavior or identity rules, checkout Links:DTO below)
│   ├ Interface (any io/email/SQL operations that need to be defined but are not implemented yet, can also be under Domain, but I find this more logical because domain is only Business Logic and Application is already pre-stages of implementation)
│   └ Service (essentially use cases, difference between this and the Domain Business Rules is that Business Rules are rules that belong to the business itself and Services belong to the application, so user clicks button A, not Athlete must have a name, but Business Rules can be part of a Service, not the other way around, checkout Links:Application below)
├─ Domain - Business rules (rules that always remain true in every circumstance, also if computers would did not exist, so these can be real life business rules)
│   ├ Enum (self-explanatory, example Role)
│   ├ Entity (such as Athlete, can create objects from them)
│   ├ Exception (self explanatory, checkout Links:Exceptions below)
│   └ Value object (validate Email for example, the ones that need to be validated centrally, checkout Links:Value objects below)
├─ Infrastructure
│   ├ Auth (authentication + JWT for the API)
│   ├ External (external APIs/CLIs, basically anything that talks to the outside world, checkout Links:View)
│   └ Repository (the repositories can be used for IO purposes or to define SQL classes, when writing good general repositories, checkout Links:General repositories below, checkout Links:Repository below)
├─ MApp
│   ├ View
│   ├ ViewModel
│   ├ Model (optional, only for UI components, sometimes merged in ViewModel)
│   └ Resource
│       ├ Config
│       ├ Image
│       └ Language
├─ Test
│   ├ Unit - 3 of each for MApp
│   └ UI - 3 of each for MApp
└─ Web
    ├ Controller
    ├ View
    ├ Model (optional, only for UI components)
    └ Resource
        ├ Config
        ├ Image
        ├ Language
        └ Style
```


## Use cases
Systeem

Doel

Actoren
Ontwikkelaar, Klant, Eigenaar, Medewerker

### Gebruiker registreert zich
- **Samenvatting**: gebruiker is nog geen klant bij Build Stronger en gaat zich registreren. Als alle velden correct zijn ingevoerd, zal de gebruiker opgeslagen worden in het systeem.
- **Primaire actor**: gebruiker
- **Preconditie**: mag niet al een geregistreerd lid zijn, 
- **Trigger**: gebruiker probeert te registreren
- **Flow**: 
    1. Voer veld Voornaam in
    2. Voer veld Achternaam in
    3. Voer veld Email adres in
    4. Voer veld Wachtwoord in
    5. Gebruiker probeert zich te registreren    
    6. Systeem controleert de ingevoerde gegevens
    7. Gebruiker krijgt toegang tot het systeem
- **Postconditie**: gebruiker is opgeslagen in het systeem

### Gebruiker logt in
- **Samenvatting**: gebruiker probeert toegang te krijgen tot het systeem
- **Primaire actor**: gebruiker
- **Preconditie**: gebruiker heeft een account
- **Trigger**: gebruiker probeert in te loggen
- **Flow**: inloggen
    1. Voer veld Email adres in
    2. Voer veld Wachtwoord in
    3. Gebruiker probeert in te loggen
    4. Systeem controleert de ingevoerde gegevens
    5. Gebruiker krijgt toegang tot het systeem
- **Alternatieve flow 1**: heeft geen account of voert foutieve email of wachtwoord in
    5. Gebruiker wordt geweigerd door systeem en krijgt een foutmelding
    6. Gebruiker blijft op het inlogscherm
- **Postconditie**: gebruiker is ingelogd of heeft een melding ontvangen waarom het inloggen mislukt is

### Gebruiker sluit abonnement af
- **Samenvatting**: gebruiker heeft interesse om te sporten bij Build Stronger en koopt één van de vier abonnementen in
- **Primaire actor**: gebruiker
- **Preconditie**: gebruiker is ingelogd, beschikt over een betaalmethode
- **Trigger**: gegruiker wil een abonnement afsluiten
- **Flow**: 
    1. Navigeer naar Account-pagina
    2. Selecteer abonnement
    3. Kies één van de vier abonnementen
    4. Gebruiker gaat afrekeken
    5. Systeem geeft aan of de betaling geslaagd of gefaald is
- **Postconditie**: als betaling geslaagd is, heeft de gebruiker voor de duur van het abonnement toegang tot de lessen

### Gebruiker past abonnement aan
- **Samenvatting**: gebruiker wil up- of downgraden naar een ander abonnement
- **Primaire actor**: gebruiker
- **Preconditie**: gebruiker is ingelogd, heeft een actief abonnement, heeft een geldige Wero rekening
- **Trigger**: gebruiker selecteert Opslaan
- **Flow**: 
    1. Navigeer naar Account-pagina
    2. Selecteer Abonnement
    3. Kies voor het gewenste abonnement in de dropdown
    4. Sla de aanpassingen op
- **Postconditie**: de gebruiker heeft voor de duur van het nieuwe abonnement toegang tot de lessen

### Gebruiker verlengt abonnement
- **Samenvatting**: gebruiker heeft geen terugkerend abonnement en moet verlengen
- **Primaire actor**: gebruiker
- **Preconditie**: gebruiker is ingelogd, heeft een actief abonnement, beschikt over een betaalmethode
- **Trigger**: gebruiker slecteert de knop Verleng nu
- **Flow**: 
    1. Navigeer naar Account-pagina
    2. Selecteer Abonnement
    3. Gebruiker kiest voor verlengen
    4. Systeem verwerkt de betaling
    5. Systeem bevestigt de verlenging
- **Postconditie**: als betaling geslaagd is, heeft de gebruiker voor de duur van het abonnement toegang tot de lessen

### Gebruiker annulleert abonnement
- **Samenvatting**: gebruiker wil geen gebruik meer maken van Build Stronger en annulleert zijn bestaande abonnement
- **Primaire actor**: gebruiker
- **Preconditie**: gebruiker is ingelogd, heeft een actief abonnement
- **Trigger**: gebruiker kiest voor de opzegging
- **Flow**: 
    1. Navigeer naar Account-pagina
    2. Selecteer Abonnement
    3. Gebruiker kiest voor de opzegging van het abonnement
    4. Bevestig de opzegging
- **Postconditie**: er wordt geen geld van de rekening afgeschreven en gebruiker heeft geen toegang meer tot de lessen

### Gebruiker past profiel aan
- **Samenvatting**: gebruiker moet om bepaalde redenen informatie aanpassen in zijn profiel
- **Primaire actor**: gebruiker
- **Preconditie**: gebruiker is ingelogd
- **Trigger**: gebruiker heeft veranderingen aan het profiel gedaan en slaat op
- **Flow**: 
    1. Navigeer naar Account-pagina
    2. Selecteer het profiel
    3. Pas desgewenste informatie aan
    4. Sla het profiel op
- **Postconditie**: de nieuwe profielinformatie is opgeslagen

### Gebruiker reserveert een les
- **Samenvatting**: gebruiker wil zich aanmelden voor een les op een bepaalde datum en tijd
- **Primaire actor**: gebruiker
- **Preconditie**: gebruiker is ingelogd, heeft een actief abonnement
- **Trigger**: gebruiker kiest voor aanmelden
- **Flow**: succesvolle reservatie
    1. Navigeer naar Planning-pagina
    2. Selecteer de gewenste workout en tijdstip in lijst
    3. Gebruiker meldt zich aan voor de les
    4. Gebruiker krijgt bevestiging van de aanmelding
- **Alternatieve flow 1**: les is vol
    3. Les is vol wordt zichtbaar op de pagina getoont
    4. Gebruiker kan zich aanmelden voor de wachtlijst
- **Alternatieve flow 2**: meldt zich dag van tevoren aan
    3. Op de details pagina staat dat het niet meer mogelijk is om jezelf aan te melden voor deze les
    4. Gebruiker kan zichzelf niet meer aanmelden voor de les
- **Postconditie**: gebruiker staat op de lijst of wachtlijst voor de les en krijgt visuele bevestiging

### Plek komt vrij in les
- **Samenvatting**: gebruiker heeft zich aangemeld voor wachtlijst en een plek komt vrij
- **Primaire actor**: gebruiker
- **Preconditie**: gebruiker is ingelogd, heeft een actief abonnement
- **Trigger**: een andere gebruiker meldt zichzelf af waardoor er een plek vrijkomt
- **Flow**: 
    1. Gebruiker krijgt een notificatie dat hij is toegevoegd aan de les
    2. Op de Boekingen-pagina is zichtbaar dat gebruiker van de wachtlijst af is gehaald
- **Postconditie**: gebruiker is aan de les toegevoegd

### Gebruiker reserveert fiets
- **Samenvatting**: gebruiker wil een spinning les gaan volgen en reserveert een fiets met een plek in de ruimte
- **Primaire actor**: gebruiker
- **Preconditie**: gebruiker is ingelogd, heeft een actief abonnement, er moeten nog fietsen beschikbaar zijn
- **Trigger**: selecteert de spinningles
- **Flow**: 
    1. Navigeer naar Planning-pagina
    2. Selecteer gewenste workout en tijdstip in lijst
    3. Selecteer de gewenste plek voor de spinningles
    4. Gebruiker meldt zich aan voor de les
- **Postconditie**: gebruiker is aan de les toegevoegd

### Medewerker voegt instructeur toe
- **Samenvatting**: er is een nieuwe instructeur in dienst gekomen en moet toegevoegd worden aan het systeem
- **Primaire actor**: medewerker of instructeur
- **Preconditie**: gebruiker is ingelogd, is medewerker, instructeur of administrator
- **Trigger**: gebruiker voegt een instructeur toe
- **Flow**: 
    1. Navigeer naar het gebruikersoverzicht
    2. Selecteer het Toevoegen icoontje
    3. Voer de gegevens van de nieuwe instructeur in
    4. Selecteer het schuifje Is instructeur
    5. Instructeur wordt toegevoegd
- **Postconditie**: nieuwe gebruiker is toegevoegd aan het systeem met de rol van instructeur

### Medewerker voegt nieuwe sport toe
- **Samenvatting**: er gaat een nieuwe sport gegeven worden en moet aan het systeem toegevoegd worden
- **Primaire actor**: medewerker of instructeur
- **Preconditie**: gebruiker is ingelogd, is medewerker, instructeur of administrator
- **Trigger**: gebruiker voegt een sport toe
- **Flow**: 
    1. Navigeer naar het sportoverzicht
    2. Selecteer het Toevoegen icoontje
    3. Voer de gegevens van de nieuwe sport in
    4. Sport wordt toegevoegd
- **Postconditie**: de nieuwe sport is toegevoegd aan het systeem en kan nu gekozen worden door de instructeurs

## Value objects
- EmailAddress
- Money
- Duration
- Capacity
- DateRange
- SubscriptionPeriod
- LessonTime
- FullName

## Business rules
- Vanaf 1 week van tevoren kan een les niet meer geboekt worden
- Afmelden bij een les kan tot maximaal 1 uur van tevoren
- Lessen kunnen niet bestaan zonder een workout
- Lid kan kiezen wanneer lidmaadschap in kan gaan
- Er kan slechts één fiets per spinningles per lid worden gereserveerd
- Een spinningles kan niet méér plekken hebben dan er fietsen zijn, inclusief een voor de instructeur
- Er kunnen geen lessen worden geboekt zonder actief abonnement
- Een les kan niet over de maximale capaciteit heen gaan
- Les start op specifieke tijd en heeft bepaalde tijdsduur
- Les moet eenmalig of repeterend in het rooster gezet kunnen worden
- Een les kan tot het laatste moment gegeven worden door een andere instructeur

## Overige regels
- Medewerkers zien een volledig overzicht van de bezetting van lessen in het verleden, heden en toekomst
- Medewerkers kunnen ledeninformatie zien en de status van het lidmaadschap
- Gebruiker moet volledige controle hebben over abonnement en eigen profiel
- Overige functies van de app zijn gewoon toegankelijk zonder abonnement
- Medewerkers hebben beheer over hun leden en sporten
- Afmelden van een wachtlijst moet altijd kunnen
- Na een eerste keer inloggen, blijft de gebruiker ingelogd in de app
- Een gebruiker wordt automatisch op aanwezig gezet als die aankomt op locatie
- Leden kunnen per les zien wie er nog meer naar de les komen
- Een gebruiker komt automatisch op de wachtlijst te staan als de les vol is
- Als er een plek vrijkomt omdat iemand zichzelf heeft afgemeldt, gaat de eerst ingeschreven gebruiker op de aanmeldlijst
- Applicatie moet in elk geval bij inloggen of registreren een internet connectie hebben

## Exceptions

### Custom
- AthleteExistsException: athlete already exists in system
- AthleteNotFoundException: athlete is not found in the system
- LessonBookedException: the lesson is already full
- PaymentFailedException
- 

### Build in
- InvalidCredentialException: user logged in with invalid credentials

## Entities
- Athlete contains
  - Subscription
- Equipment contains
  - EquipmentRoom
- EquipmentRoom contains
  - Room
- EquipmentSpot contains
  - Lesson
  - EquipmentRoom
  - Reservation
- Instructor
- Lesson
  - Workout
  - Schedule
  - Instructor
  - Equipment
- Location
- Payment
  - Subscription
- Reservation
  - Athlete
  - Lesson
- Room
- Schedule
- Subscription contains
  - SubscriptionPlan
- SubscriptionPlan
- Workout
  - Room
  - Equipment

## DbContext
- Athlete contains: GET, UPDATE, DELETE, ADD
  - Subscription: GET
- Equipment: GET
- EquipmentRoom contains
  - Room
- EquipmentSpot: GET
  - Lesson
  - EquipmentRoom
- Instructor: GET, [role instructor] SET
- Lesson: GET, SET [role instructor], UPDATE [role instructor], DELETE [role instructor]
  - Workout
  - Schedule
  - Instructor
  - Equipment
  - Room
- Location: GET
- Payment: SET
  - Subscription
- Reservation: GET [AuthorizationDecision true], SET [AuthorizationDecision true], UPDATE [AuthorizationDecision true], DELETE [AuthorizationDecision true]
  - Athlete
  - Lesson
  - EquipmentSpot?
- Room: GET
- Workout: GET, SET [role instructor], UPDATE [role instructor], DELETE [role instructor]
  - Equipment

## Links
- Value objects: https://marcoatschaefer.medium.com/onion-architecture-explained-building-maintainable-software-54996ff8e464#2def, https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects
- Exceptions: https://learn.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-localized-exception-messages
- Application: https://marcoatschaefer.medium.com/onion-architecture-explained-building-maintainable-software-54996ff8e464#8e78
- DTO: https://marcoatschaefer.medium.com/onion-architecture-explained-building-maintainable-software-54996ff8e464#202a, https://medium.com/@20011002nimeth/understanding-data-transfer-objects-dtos-in-c-net-best-practices-examples-fe3e90238359
- Repository: https://marcoatschaefer.medium.com/onion-architecture-explained-building-maintainable-software-54996ff8e464#b80f
- General repositories: https://www.ben-morris.com/why-the-generic-repository-is-just-a-lazy-anti-pattern/
- View: https://marcoatschaefer.medium.com/onion-architecture-explained-building-maintainable-software-54996ff8e464#0b6e
- Use case: https://www.figma.com/nl-nl/resource-library/wat-is-een-use-case/
- Database: https://medium.com/@mcansener/seamless-database-integration-with-c-a-practical-guide-80ec8321f6f2
- JWT: https://medium.com/@sajadshafi/jwt-authentication-in-c-net-core-7-web-api-b825b3aee11d