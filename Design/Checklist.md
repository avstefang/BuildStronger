# Checklist
## Ontwerpdocument
- [ ] Definition of Done opstellen
- [ ] Toelichting/samenvatting op de functionaliteit van je .NET MAUI app (in combinatie met de backend)
- [ ] User stories
- [ ] Use case diagram
- [ ] Ontwerp van je schermen met Wireframes
- [ ] Web API / Azure Cloud documentatie (met voorbeelden van requests)
- [ ] Toelichting authenticatie en autorisatie die gebruikt wordt
- [ ] Package diagram waarin ook de gekozen architectuur duidelijk wordt
- [ ] Deployment diagram
- [ ] klassendiagrammen van de relevante eigen ontworpen klassen
- [ ] Sequence diagrammen:
    - [ ] minimaal één waarin MVVM architectuur wordt toegelicht.
    - [ ] minimaal één waarin je interactie tussen de .NET MAUI app en de gebruikte backend illustreert.

## Blazor wasm applicatie
- [ ] Blazor beheer applicatie voor de medewerkers
- [ ] Bevat de volgende beheer elementen:
    - [ ] Informatie over de workouts
    - [ ] Informatie over de trainers
    - [ ] Informatie over de locaties
    - [ ] Informatie over het lesrooster
    - [ ] Nieuwe workouts (sport) toevoegen
    - [ ] Lessen per workout inplannen in rooster (CRUD)
        - [ ] Les start op specifieke tijd
        - [ ] Les heeft een bepaalde tijdsduur in minuten
        - [ ] Les heeft altijd een locatie én instructeur
        - [ ] Les moet eenmalig of repeterend in het rooster gezet kunnen worden (dag, week, maand, einddatum of aantal herhalingen (ING bankieren app))
    - [ ] Instructeurs kunnen veranderen en kan op nnb (nog niet bekend) komen te staan
    - [ ] Medewerkers hebben CRUD rechten
    - [ ] Workout verwijderen verwijderd ook alle gekoppelde lessen
    - [ ] Medewerkers kunnen instructeurs toevoegen met naam en foto
    - [ ] Medewerkers zien een volledig overzicht van de bezetting van lessen in het verleden, heden en toekomst
    - [ ] Medewerkers kunnen ledeninformatie zien en de status van het lidmaadschap
- [ ] Laat geïnteresseerden de volgende dingen zien:
    - [ ] Aanbod van lessen
    - [ ] Abonnement tarieven
    - [ ] Aanmelden voor nieuwe leden
    - [ ] Is een uithangbord
    - [ ] (.NET MAUI) App kan worden gedownload
    - [ ] Lid kan kiezen wanneer lidmaadschap in kan gaan
    - [ ] Lid krijgt email met daarin bevestiging van betaling en aanmelding
- [ ] Huidige abonnementen:
    - [ ] 2x per week sporten - €30,- per maand of €299,- per jaar
    - [ ] Onbeperkt - €55,- per maand of 549,- per jaar
- [ ] Ruimtes in de sportclub:
    - [ ] 3 ruimten voor groepslessen (zoals yoga, bodyshape, club power, xco, total body workout, …) Capaciteit zaal 1: 42 Capaciteit zaal 2: 32 Capaciteit zaal 3: 24
    - [ ] 1 buitenruimte (capaciteit 20 leden) voor outdoor bootcamp en boksen
    - [ ] 1 spinningruimte met 4 rijen van ieder 6 fietsen. Dit is de enige les waarbij bij het aanmelden een specifieke plek (fiets) gekozen kan worden.

## C# multiplatform Applicatie
- [ ] Een functioneel geteste en werkende .NET MAUI cross-platform applicatie
- [ ] Een eigen .NET backend inclusief persistentie die past bij de kwestie waarvoor een oplossing wordt gerealiseerd
- [ ] Meest recente (.NET) technologie wordt toegepast (in elk geval C# 14 en .NET 10)
- [ ] Broncode en GUI zijn Engelstalig en voor een internationale doelgroep
- [ ] Maak gebruik van sensoren
- [ ] Bevat de volgende elementen:
    - [ ] Aanmelden en aangemeldt blijven
    - [ ] Status abonnement inzien
    - [ ] Profiel bekijken
    - [ ] Profiel bewerken (foto uploaden, gebruikersnaam opgeven)
    - [ ] Abonnement toevoegen (vóór einde jaarabonnement kan opnieuw aangeschaft worden en krijgen daar 6 weken van tevoren een notificatie over)
    - [ ] Abonnement bekijken
    - [ ] Abonnement aanpassen (bijv. van 2x in de week naar onbeperkt)
    - [ ] Abonnement verlengen
    - [ ] Abonnement annuleren (jaarabonementen kunnen niet stopgezet/terug gedraait worden)
    - [ ] Reserveren van lessen
    - [ ] Abonneren op de wachtlijst als een les vol is
    - [ ] Fiets reserveren bij spinninglessen
    - [ ] Notificatie sturen als iemand zich af heeft gemeld en van de wachtlijst af bent gehaald
    - [ ] iDEAL/Wero betaling mag worden gesimuleerd, liefst met Stripe developer
    - [ ] Lesrooster kan meerdere dagen van tevoren vooruit worden bekeken, maar reserveren kan tot 1 week van tevoren
    - [ ] Per les kun je de aangemelde leden en gebruikersnaam en profielfoto zien
    - [ ] Les reserveren kan, mits je genoeg credits hebt
    - [ ] Spinningles kan je een specifieke plek reserveren
    - [ ] Je kan je aanmelden voor een wachtlijst en ook weer verwijderen
    - [ ] Afmelden voor een les kan tot maximaal 1 uur van tevoren en leden op de wachtlijst krijgen dan een notificatie dat de plek vrij is
    - [ ] Je wordt automatisch aangemeldt als aanwezig op basis van GEO of met RFID als GEO niet beschikbaar is, instructeur kan zien wie er wel en niet is
    - [ ] Een geschiedenis is in te zien van je historische lessen
- [ ] Bevat de volgende lessen:
    * Spinninglessen
    * Yoga
    * Bootcamp
    * Boksen
    * Bodyshape
    * Club power
    * XCO
    * Total Body Workout

### Unittesten en UI testen (alleen voor MAUI)
- [ ] Per functionaliteit een unittest
- [ ] 3 zinvolle verschillende unittesten
- [ ] 3 zinvolle verschillend ui / device running testen
- [ ] Opstellen acceptatietest(en) op basis van  Definition of Done
- [ ] Er is een projectbestand dat alle code en assets bevat
- [ ] Aantoonbaar gebruik van een ingerichte werkomgeving
- [ ] Een korte beschrijving met aanwijzingen hoe de applicatie geïnstalleerd en gebruikt moet worden.
- [ ] Een checklist of aan de Definition of Done is voldaan.

## Filmpje
- [ ] kort je .NET MAUI applicatie toelicht (doel, welke gebruikers)
- [ ] gerealiseerde user stories van je app demonstreert.
- [ ] toelicht welke keuzes je hebt gemaakt in je ontwerp en realisatie
- [ ] onderdelen van je realisatietraject waarop je trots bent


0. Scrijf alles op in Ontwerpdocument Markdown, maar laat de structuur even voor wat het is. Dat komt later wel.
1. Denk de SQL uit in Markdown en laat, als die (bijna) klaar is, Copilot ernaar kijken en de code genereren.
2. Probeer de API/Blazor applicatie van tevoren zoveel mogelijk uit te denken en laat eventueel Copilot daarna ernaar kijken.
3. Maak de API/Blazor combi, maar begin met de API.
4. Maak de Blazor applicatie en laat eventueel wat HTML code genereren.
5. Denk over hoe je het design wilt hebben van de C# applicatie.
6. Maak het design van de C# applicatie in WPF Multiplatform. Hang er nog geen logica achter, maar hou er al wel meteen rekening mee zodat het later makkelijker te implementeren is.
7. Denk de logica verder uit voor de backend van C#.
8. Ga elke stap nog na om alles recht te trekken op wat er tot nu toe is gemaakt. Zolang de API goed uit is gedacht, zou dit niet veel werk meer moeten zijn.
9. Als laatste, kijk naar het Ontwerpdocument. Er zou genoeg informatie moeten zijn door het klad gemaakt en de rest dat uit is gedacht en gemaakt om alles verder af te maken.