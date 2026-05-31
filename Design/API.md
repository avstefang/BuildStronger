# API uitdenken

## GET

Omschrijving: gebruik om data op te halen  en moet idempotent zijn - READ

- athlete
    - remainingCreditAmount
    - photo
- workout
- instructor - get the athlete and only return when tested true on isInstructor
- location
- lesson
    - getCapacity
- schedule - returns the lesson and the schedule from the database
- subscription
- waitinglist
- payment
- reservation
- equipment
- equipmentRoom
- equipmentSpot
- equipmentSpotReservation
- room

## POST

Omschrijving: gebruik om nieuwe onderliggende resources aan te maken - NEW

- athlete
    - photo
- workout
- instructor
- location
- schedule - only allow schedule (for now, as lesson creating a single lesson is not needed), this way you can create a new lesson on the schedule and will be split to both the lesson and schedule table in the database (first create the lesson, then create the schedule itself with the just created lessonId)
- subscription
- waitinglist
- payment
- reservation
- equipment
- equipmentRoom
- equipmentSpot
- equipmentSpotReservation
- room

## PUT

Omschrijving: gebruik om al bestaande resources te updaten of vervangen - UPDATE/REPLACE

- athlete
    - photo
- workout
- instructor
- location
- lesson
- schedule
- subscription
- waitinglist
- payment
- reservation
- equipment
- equipmentRoom
- equipmentSpot
- equipmentSpotReservation
- room

## DELETE

Omschrijving: gebruik om een bestaande resource te verwijdern - DELETE

- athlete
    - photo
- workout
- instructor
- location
- lesson
- schedule
- subscription
- waitinglist
- payment
- reservation
- equipment
- equipmentRoom
- equipmentSpot
- equipmentSpotReservation
- room