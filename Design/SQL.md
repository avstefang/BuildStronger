# SQL database
* athlete
    - id: GUID, NOT NULL, PK
    - emailAddress: NOT NULL, Unique
    - firstName: NOT NULL
    - lastName: NOT NULL
    - password: NOT NULL
    - role: ENUM[(default)User, Administrator, Employee, Instructor, NNB_Instructor], DEFAULT FALSE
    - username: empty, Unique
    - photoPath: empty
* lesson
    - id: GUID, NOT NULL, Increment, Unique, PK
    - workoutId: NOT NULL, FK
    - customDuration: NULLABLE, INT
    - maxCapacity: NOT NULL, INT,
    - instructorId: (create unknown instructor with id 1 later on), FK, INT
* schedule
    - id: GUID, NOT NULL, Increment
    - startTime: NOT NULL, TIME
    - lessonId: GUID, NOT NULL, FK, Unique
    - startDay: NOT NULL, ENUM[Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday]
    - repetitionCount: DEFAULT 0 (never), -1 is no end, above 0 is setting specific repetitions
    - repetitionEndDate: NULLABLE, DATE
    - registeredAt: NOT NULL, DATE
* workout
    - id: GUID, NOT NULL, Increment, Unique, PK
    - name: NOT NULL, Unique
    - description: NOT NULL, STRING
    - duration: NOT NULL
    - roomId: NOT NULL, FK, INT
* workout_equipment
    - workoutId: GUID, NOT NULL, FK
    - equipmentId: GUID, NOT NULL, FK
    - Unique(workoutId, equipmentId)
* reservation
    - lessonId: FK, NOT NULL
    - athleteId: GUID, FK, NOT NULL
    - reservationDate: DATE, NOT NULL
    - status: ENUM[Cancelled, Waitinglist, Accepted, CheckedIn, CheckedOut]
    - reservedAt: NOT NULL, DATETIME
    - Unique(lessonId, athleteId)
* subscription
    - id: GUID, NOT NULL, Increment, Unique, PK
    - athleteId: GUID, NOT NULL, FK
    - subscriptionPlanId: GUID, NOT NULL, FK
    - startDate: DATE, NOT NULL
    - autoRenew: DEFAULT FALSE, BOOL
    - status: DEFAULT FALSE, ENUM[Active, Cancelled, Expired]
* subscriptionPlan
    - id: GUID, NOT NULL, Increment, Unique, PK
    - name: NOT NULL, Unique
    - price: NOT NULL, DECIMAL
    - durationInMonths: INT, NOT NULL
    - monthlyCreditAmount: NOT NULL, INT
    - paymentMethod: NOT NULL, ENUM, Default Wero (others to come later)
    - paymentCurrency: NOT NULL, ENUM default eur
* payment
    - id: GUID, NOT NULL, Increment, Unique, PK
    - subscriptionId: GUID, NOT NULL, FK
    - amount: DECIMAL, NOT NULL
    - status: NOT NULL, ENUM[Pending, Succeeded, Failed, Refunded, Cancelled]
    - payedAt: DATETIME2, NOT NULL
    - method: NOT NULL, ENUM, Default Wero (others to come later)
    - processorId: string, NOT NULL
    - currency: NOT NULL, ENUM default eur
* equipment
    - id: GUID, NOT NULL, Increment, Unique, PK
    - name: NOT NULL, Unique
* equipmentRoom
    - id: GUID, NOT NULL, Increment, Unique, PK
    - roomId: GUID, NOT NULL, FK
    - rowCount: NOT NULL, INT
    - spotsPerRow: NOT NULL, INT
* equipmentSpot
    - id: GUID, NOT NULL, Increment, Unique, PK
    - equipmentId: GUID, NOT NULL, FK
    - equipmentRoomId: GUID, NOT NULL, FK
    - rowNumber: NOT NULL, INT
    - spotNumber: NOT NULL, INT
    - Unique(equipmentRoomId, rowNumber, spotNumber)
* equipmentSpot_reservation
    - lessonId: GUID, NOT NULL, FK
    - athleteId: GUID, NOT NULL, FK
    - equipmentSpotId: GUID, NOT NULL, FK
    - reservationId: GUID, NOT NULL, FK
    - Unique(lessonId, athleteId)
    - Unique(lessonId, equipmentSpotId)
    - Unique(lessonId, reservationId)
* location
    - id: GUID, NOT NULL, Increment, Unique, PK
    - name: NOT NULL, Unique
    - address: NOT NULL
* room
    - id: GUID, NOT NULL, Increment, Unique, PK
    - name: NOT NULL, Unique
    - capacity: INT, NOT NULL
    - locationId: GUID, NOT NULL, FK

## Bereken in app of API (niet via database)
* Aangemeldt blijven
* Status abonnement inzien ?????
* 6 weken van tevoren notificatie sturen verlengen sportabonnement
* Berekenen van de wachtlijst als er iemand afgehaald moet worden en aan de les toegevoegd moet worden
* Laten zien van aangemelde leden, gebruikersnaam en profielfoto
* Overblijvende credits berekenen aan de hand van lessen gedaan die week
* Reserveren tot één week van tevoren
* Afmelden max 1 uur van tevoren en sturen notificatie leden wachtlijst
* Toevoegen en verwijderen wachtlijst
* Automatisch TRUE zetten van isCheckedIn op basis van GEO locatie
* Historische lessen
* Spinningles koppelen aan een reservering per fiets/plek in de API
* Kan maar één spot voor de les/equipment reserveren