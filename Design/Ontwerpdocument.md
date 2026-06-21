# Ontwerpdocument
## Managementsamenvatting

## Inleiding
Twee jaar geleden is de eigenaar van sportclub Build Stronger begonnen voor zichzelf na meer dan 20 jaar ervaring met sportbegeleiding en fitness systemen. De sportclub is altijd medium druk en er hangt altijd een gezellige sfeer. De afgelopen twee jaar zijn er ook nog drie andere medewerkers aangenomen om samen een nog betere ervaring te kunnen bieden aan klanten. Ook is er in mei 2026 een uitbreiding gestart om aan de behoeften van hun klanten te kunnen voldoen.

Deze uitbreiding heeft als gevolg dat er nu teveel lessen en medewerkers zijn om op een efficiënte manier te kunnen ondersteunen in het Excel-document wat ze momenteel gebruiken. Build Stronger krijgt ook van klanten vaak de feedback dat ze moeilijk op internet te vinden zijn, ondanks dat ze wel Instagram en andere sociale media hebben.

Doordat Build Stronger een groeiend bedrijf is die moeilijk op de huidige manier kan blijven werken, hebben ze geconstateerd dat ze een webapplicatie én een applicatie die gebruikt kan worden op meerdere platformen (zoals Android, iOS, Windows) willen hebben.



## Scope & context



## User stories

### Account & Authenticatie

* Als gebruiker wil ik mij registreren zodat ik een account kan aanmaken en toegang krijg tot het systeem.
* Als gebruiker wil ik inloggen zodat ik mijn account en gegevens kan beheren.
* Als gebruiker wil ik ingelogd blijven zodat ik niet steeds opnieuw hoef in te loggen.

---

### Profielbeheer

* Als gebruiker wil ik mijn profiel bekijken zodat ik mijn gegevens kan controleren.
* Als gebruiker wil ik mijn profiel aanpassen zodat mijn gegevens up-to-date blijven.
* Als gebruiker wil ik een profielfoto uploaden zodat mijn account persoonlijker wordt.

---

### Abonnementen

* Als gebruiker wil ik een abonnement afsluiten zodat ik toegang krijg tot lessen.
* Als gebruiker wil ik mijn abonnement bekijken zodat ik inzicht heb in mijn status.
* Als gebruiker wil ik mijn abonnement aanpassen zodat ik kan upgraden of downgraden.
* Als gebruiker wil ik mijn abonnement verlengen zodat ik toegang behoud tot lessen.
* Als gebruiker wil ik mijn abonnement annuleren zodat ik geen gebruik meer maak van de dienst.

---

### Lessen & Reserveringen

* Als gebruiker wil ik het lesrooster bekijken zodat ik kan zien welke lessen beschikbaar zijn.
* Als gebruiker wil ik een les reserveren zodat ik kan deelnemen aan een training.
* Als gebruiker wil ik mij afmelden voor een les zodat iemand anders mijn plek kan gebruiken.
* Als gebruiker wil ik op een wachtlijst komen zodat ik alsnog kan deelnemen als er een plek vrijkomt.
* Als gebruiker wil ik een notificatie ontvangen wanneer ik van de wachtlijst afkom zodat ik weet dat ik kan deelnemen.
* Als gebruiker wil ik mijn lesgeschiedenis bekijken zodat ik inzicht heb in mijn activiteiten.

---

### Spinning

* Als gebruiker wil ik een specifieke fiets reserveren zodat ik mijn voorkeur plek kan kiezen.

---

### Betalingen

* Als gebruiker wil ik betalen voor een abonnement zodat mijn lidmaatschap geactiveerd wordt.
* Als gebruiker wil ik een bevestiging ontvangen na betaling zodat ik zeker weet dat mijn aankoop gelukt is.

---

### Medewerker functionaliteiten

* Als medewerker wil ik workouts beheren zodat het aanbod actueel blijft.
* Als medewerker wil ik lessen inplannen zodat het rooster gevuld is.
* Als medewerker wil ik instructeurs beheren zodat lessen correct toegewezen worden.
* Als medewerker wil ik ledeninformatie bekijken zodat ik inzicht heb in klanten.

---

### Instructeurs

* Als instructeur wil ik zien wie zich heeft aangemeld voor een les zodat ik weet wie aanwezig is.

---

### Notificaties

* Als gebruiker wil ik notificaties ontvangen zodat ik op de hoogte blijf van belangrijke wijzigingen (bijv. wachtlijst, abonnement).

## Definition of Done

### Algemeen

Een user story of functionaliteit wordt als “Done” beschouwd wanneer aan alle onderstaande criteria is voldaan.

---

### 1. Functionaliteit

* De functionaliteit is volledig geïmplementeerd volgens de bijbehorende user story en use case.
* Alle beschreven flows (inclusief alternatieve flows) werken correct.
* Alle precondities en postcondities uit de use cases worden correct afgehandeld.
* Edge cases (bijv. volle les, foutieve invoer, verlopen abonnement) zijn geïmplementeerd en getest.

---

### 2. Validatie & Business Rules

* Invoer wordt gevalideerd (bijv. verplichte velden, geldig e-mailadres, wachtwoordregels).
* Foutmeldingen zijn duidelijk en gebruiksvriendelijk.
* Business rules worden correct toegepast, zoals:

  * Alleen reserveren met actief abonnement
  * Niet reserveren binnen 1 uur voor aanvang
  * Wachtlijst-functionaliteit bij volle lessen
  * Betaling vereist voor activatie abonnement

---

### 3. Integratie (Frontend ↔ Backend)

* Alle API-calls werken correct en geven verwachte responses terug.
* Foutafhandeling is geïmplementeerd (bijv. netwerkfouten, server errors).
* Data wordt correct opgeslagen en opgehaald uit de backend (persistentie).

---

### 4. User Experience (UX/UI)

* De gebruiker kan alle flows succesvol doorlopen zonder fouten.
* Navigatie tussen schermen werkt logisch en consistent.
* De UI toont duidelijke feedback (bijv. bevestigingen, foutmeldingen, laadindicatoren).
* De applicatie crasht niet tijdens normaal gebruik.

---

### 5. Codekwaliteit

* Code compileert zonder errors of warnings.
* Code volgt de afgesproken architectuur (bijv. MVVM).
* Code is leesbaar, consistent en Engelstalig.
* Herbruikbare componenten en logica zijn correct toegepast.

---

### 6. Testing

* Voor elke belangrijke functionaliteit zijn unittesten aanwezig.
* Minimaal 3 verschillende unittesten zijn geïmplementeerd en slagen.
* UI/device tests zijn uitgevoerd (minimaal 3 scenario’s).
* Acceptatietesten zijn opgesteld op basis van deze Definition of Done.

---

### 7. Security & Authenticatie

* Gebruikers kunnen alleen acties uitvoeren waarvoor ze geautoriseerd zijn.
* Authenticatie werkt correct (login, registratie, sessiebeheer).
* Gevoelige data (zoals wachtwoorden) wordt veilig verwerkt.

---

### 8. Documentatie

* Relevante documentatie is bijgewerkt:

  * Use cases
  * Diagrammen (sequence, class, deployment, etc.)
  * API documentatie
* Installatie- en gebruiksinstructies zijn aanwezig.

---

### 9. Deployment & Techniek

* Applicatie is buildbaar en start zonder fouten.
* Werkt op de beoogde platformen (.NET MAUI).
* Gebruikt de vereiste technologieën (.NET 10, C# 14).
* Integraties (bijv. betalingen, notificaties) werken of zijn gesimuleerd.

---

### 10. Acceptatiecriteria (Use Case gebaseerd)

* Elke use case is aantoonbaar werkend in de applicatie.
* Zowel standaard flows als alternatieve flows zijn getest:

  * Registratie en login
  * Abonnement afsluiten, wijzigen, verlengen en annuleren
  * Les reserveren en wachtlijst
  * Fiets reserveren
  * Medewerkerfunctionaliteiten
* De verwachte postconditie van elke use case wordt bereikt.

---

### 11. Oplevering

* Alle functionaliteiten zijn aantoonbaar werkend.
* Er is een checklist ingevuld waaruit blijkt dat aan deze DoD is voldaan.
* De applicatie kan gedemonstreerd worden (filmpje).

## Architectuur & ontwerpkeuzes

## Diagrammen
### Sequence diagram
MVVM architectuur toelichten
Interactie laten zien tussen .NET MAUI app en gebruikte backend

### Deployment diagram
### Use case diagram
### Klassendiagram
### Package diagram

## Wireframes

## Documentatie

## Authenticatie en authorisatie
