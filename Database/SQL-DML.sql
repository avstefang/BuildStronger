-- SQL Server DML zaaddata
-- Aannames:
-- - Voer dit uit na SQL-DDL.sql op een schone database
-- - Dit script vult representatieve demogegevens in voor het fitnessmodel

SET NOCOUNT ON;

DECLARE
    @InstructorId UNIQUEIDENTIFIER,
    @Member1Id UNIQUEIDENTIFIER,
    @Member2Id UNIQUEIDENTIFIER,
    @Member3Id UNIQUEIDENTIFIER,
    @MainHallId UNIQUEIDENTIFIER,
    @OutdoorAreaId UNIQUEIDENTIFIER,
    @SpinningLocationId UNIQUEIDENTIFIER,
    @GroupRoom1Id UNIQUEIDENTIFIER,
    @GroupRoom2Id UNIQUEIDENTIFIER,
    @GroupRoom3Id UNIQUEIDENTIFIER,
    @OutdoorRoomId UNIQUEIDENTIFIER,
    @SpinningRoomId UNIQUEIDENTIFIER,
    @SpinningEquipmentRoomId UNIQUEIDENTIFIER,
    @BikeId UNIQUEIDENTIFIER,
    @MatId UNIQUEIDENTIFIER,
    @GlovesId UNIQUEIDENTIFIER,
    @YogaWorkoutId UNIQUEIDENTIFIER,
    @BootcampWorkoutId UNIQUEIDENTIFIER,
    @BoksenWorkoutId UNIQUEIDENTIFIER,
    @BodyshapeWorkoutId UNIQUEIDENTIFIER,
    @ClubPowerWorkoutId UNIQUEIDENTIFIER,
    @XcoWorkoutId UNIQUEIDENTIFIER,
    @TotalBodyWorkoutId UNIQUEIDENTIFIER,
    @SpinningWorkoutId UNIQUEIDENTIFIER,
    @Monthly2xPlanId UNIQUEIDENTIFIER,
    @Yearly2xPlanId UNIQUEIDENTIFIER,
    @MonthlyUnlimitedPlanId UNIQUEIDENTIFIER,
    @YearlyUnlimitedPlanId UNIQUEIDENTIFIER,
    @YogaLessonId UNIQUEIDENTIFIER,
    @SpinningLessonId UNIQUEIDENTIFIER,
    @BootcampLessonId UNIQUEIDENTIFIER,
    @BoksenLessonId UNIQUEIDENTIFIER,
    @BodyshapeLessonId UNIQUEIDENTIFIER,
    @ClubPowerLessonId UNIQUEIDENTIFIER,
    @XcoLessonId UNIQUEIDENTIFIER,
    @TotalBodyWorkoutLessonId UNIQUEIDENTIFIER,
    @YogaReservationId UNIQUEIDENTIFIER,
    @SpinningReservation1Id UNIQUEIDENTIFIER,
    @SpinningReservation2Id UNIQUEIDENTIFIER,
    @BootcampReservationId UNIQUEIDENTIFIER,
    @YogaSpotReservationId UNIQUEIDENTIFIER,
    @SpinningSpotReservation1Id UNIQUEIDENTIFIER,
    @SpinningSpotReservation2Id UNIQUEIDENTIFIER;

DECLARE @InstructorAthleteId UNIQUEIDENTIFIER = NEWID();
DECLARE @Member1AthleteId UNIQUEIDENTIFIER = NEWID();
DECLARE @Member2AthleteId UNIQUEIDENTIFIER = NEWID();
DECLARE @Member3AthleteId UNIQUEIDENTIFIER = NEWID();

INSERT INTO athlete (id, emailAddress, firstName, lastName, [password], [role], username, photoPath)
VALUES (@InstructorAthleteId, 'instructor@gym.example', 'Sanne', 'Jansen', 'hashed-password-1', 'Instructor', 'sannej', 'images/instructors/sanne-jansen.jpg');

INSERT INTO athlete (id, emailAddress, firstName, lastName, [password], [role], username, photoPath)
VALUES (@Member1AthleteId, 'member1@gym.example', 'Anna', 'de Vries', 'hashed-password-2', 'User', 'annav', 'images/members/anna-de-vries.jpg');

INSERT INTO athlete (id, emailAddress, firstName, lastName, [password], [role], username, photoPath)
VALUES (@Member2AthleteId, 'member2@gym.example', 'Bram', 'Peters', 'hashed-password-3', 'Employee', 'bramp', 'images/members/bram-peters.jpg');

INSERT INTO athlete (id, emailAddress, firstName, lastName, [password], [role], username, photoPath)
VALUES (@Member3AthleteId, 'member3@gym.example', 'Chloe', 'Bakker', 'hashed-password-4', 'Administrator', 'chloeb', 'images/members/chloe-bakker.jpg');

SELECT @InstructorId = id FROM athlete WHERE emailAddress = 'instructor@gym.example';
SELECT @Member1Id = id FROM athlete WHERE emailAddress = 'member1@gym.example';
SELECT @Member2Id = id FROM athlete WHERE emailAddress = 'member2@gym.example';
SELECT @Member3Id = id FROM athlete WHERE emailAddress = 'member3@gym.example';

DECLARE @HoofdlocatieId UNIQUEIDENTIFIER = NEWID();
DECLARE @BuitenlocatieId UNIQUEIDENTIFIER = NEWID();
DECLARE @SpinninglocatieId UNIQUEIDENTIFIER = NEWID();

INSERT INTO location (id, name, address) VALUES (@HoofdlocatieId, 'Hoofdlocatie', 'Sportslaan|1|1234 AB|Utrecht');
INSERT INTO location (id, name, address) VALUES (@BuitenlocatieId, 'Buitenlocatie', 'Sportslaan|1|1234 AB|Utrecht');
INSERT INTO location (id, name, address) VALUES (@SpinninglocatieId, 'Spinninglocatie', 'Sportslaan|1|1234 AB|Utrecht');

SELECT @MainHallId = id FROM location WHERE name = 'Hoofdlocatie';
SELECT @OutdoorAreaId = id FROM location WHERE name = 'Buitenlocatie';
SELECT @SpinningLocationId = id FROM location WHERE name = 'Spinninglocatie';

DECLARE @Zaal1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Zaal2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Zaal3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @BuitenruimteId UNIQUEIDENTIFIER = NEWID();
DECLARE @SpinningzaalId UNIQUEIDENTIFIER = NEWID();

INSERT INTO room (id, name, capacity, locationId) VALUES (@Zaal1Id, 'Zaal 1', 42, @MainHallId);
INSERT INTO room (id, name, capacity, locationId) VALUES (@Zaal2Id, 'Zaal 2', 32, @MainHallId);
INSERT INTO room (id, name, capacity, locationId) VALUES (@Zaal3Id, 'Zaal 3', 24, @MainHallId);
INSERT INTO room (id, name, capacity, locationId) VALUES (@BuitenruimteId, 'Buitenruimte', 20, @OutdoorAreaId);
INSERT INTO room (id, name, capacity, locationId) VALUES (@SpinningzaalId, 'Spinningzaal', 24, @SpinningLocationId);

SELECT @GroupRoom1Id = id FROM room WHERE name = 'Zaal 1';
SELECT @GroupRoom2Id = id FROM room WHERE name = 'Zaal 2';
SELECT @GroupRoom3Id = id FROM room WHERE name = 'Zaal 3';
SELECT @OutdoorRoomId = id FROM room WHERE name = 'Buitenruimte';
SELECT @SpinningRoomId = id FROM room WHERE name = 'Spinningzaal';

DECLARE @YogaWorkoutRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @BootcampWorkoutRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @BoksenWorkoutRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @BodyshapeWorkoutRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @ClubPowerWorkoutRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @XcoWorkoutRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @TotalBodyWorkoutRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @SpinningWorkoutRowId UNIQUEIDENTIFIER = NEWID();

INSERT INTO workout (id, name, description, duration) VALUES (@YogaWorkoutRowId, 'Yoga', 'Ontspannende les voor mobiliteit, balans en kracht.', 60);
INSERT INTO workout (id, name, description, duration) VALUES (@BootcampWorkoutRowId, 'Bootcamp', 'Buitenles met functionele training en conditieoefeningen.', 45);
INSERT INTO workout (id, name, description, duration) VALUES (@BoksenWorkoutRowId, 'Boksen', 'Techniek- en conditieles met boksoefeningen.', 60);
INSERT INTO workout (id, name, description, duration) VALUES (@BodyshapeWorkoutRowId, 'Bodyshape', 'Les voor het vormen en versterken van het hele lichaam.', 50);
INSERT INTO workout (id, name, description, duration) VALUES (@ClubPowerWorkoutRowId, 'Club power', 'Kracht- en uithoudingsles met stangen en gewichten.', 45);
INSERT INTO workout (id, name, description, duration) VALUES (@XcoWorkoutRowId, 'XCO', 'Groepsles met reactieve tubes voor kracht en cardio.', 45);
INSERT INTO workout (id, name, description, duration) VALUES (@TotalBodyWorkoutRowId, 'Total Body Workout', 'Gevarieerde les met kracht- en cardio-oefeningen.', 60);
INSERT INTO workout (id, name, description, duration) VALUES (@SpinningWorkoutRowId, 'Spinning', 'Indoor fietstraining op hoge intensiteit.', 45);

SELECT @YogaWorkoutId = id FROM workout WHERE name = 'Yoga';
SELECT @BootcampWorkoutId = id FROM workout WHERE name = 'Bootcamp';
SELECT @BoksenWorkoutId = id FROM workout WHERE name = 'Boksen';
SELECT @BodyshapeWorkoutId = id FROM workout WHERE name = 'Bodyshape';
SELECT @ClubPowerWorkoutId = id FROM workout WHERE name = 'Club power';
SELECT @XcoWorkoutId = id FROM workout WHERE name = 'XCO';
SELECT @TotalBodyWorkoutId = id FROM workout WHERE name = 'Total Body Workout';
SELECT @SpinningWorkoutId = id FROM workout WHERE name = 'Spinning';

DECLARE @Monthly2xPlanRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @Yearly2xPlanRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @MonthlyUnlimitedPlanRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @YearlyUnlimitedPlanRowId UNIQUEIDENTIFIER = NEWID();

INSERT INTO subscriptionPlan (id, name, price, durationInMonths, weeklyCreditAmount, paymentMethod, paymentCurrency)
VALUES (@Monthly2xPlanRowId, '2x per week - maandelijks', 30.00, 1, 2, 'Wero', 'eur');
INSERT INTO subscriptionPlan (id, name, price, durationInMonths, weeklyCreditAmount, paymentMethod, paymentCurrency)
VALUES (@Yearly2xPlanRowId, '2x per week - jaarlijks', 299.00, 12, 2, 'Wero', 'eur');
INSERT INTO subscriptionPlan (id, name, price, durationInMonths, weeklyCreditAmount, paymentMethod, paymentCurrency)
VALUES (@MonthlyUnlimitedPlanRowId, 'Onbeperkt - maandelijks', 55.00, 1, 999, 'Wero', 'eur');
INSERT INTO subscriptionPlan (id, name, price, durationInMonths, weeklyCreditAmount, paymentMethod, paymentCurrency)
VALUES (@YearlyUnlimitedPlanRowId, 'Onbeperkt - jaarlijks', 549.00, 12, 999, 'Wero', 'eur');

SELECT @Monthly2xPlanId = id FROM subscriptionPlan WHERE name = '2x per week - maandelijks';
SELECT @Yearly2xPlanId = id FROM subscriptionPlan WHERE name = '2x per week - jaarlijks';
SELECT @MonthlyUnlimitedPlanId = id FROM subscriptionPlan WHERE name = 'Onbeperkt - maandelijks';
SELECT @YearlyUnlimitedPlanId = id FROM subscriptionPlan WHERE name = 'Onbeperkt - jaarlijks';

DECLARE @BikeRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @MatRowId UNIQUEIDENTIFIER = NEWID();
DECLARE @GlovesRowId UNIQUEIDENTIFIER = NEWID();

INSERT INTO equipment (id, name) VALUES (@BikeRowId, 'Fiets');
INSERT INTO equipment (id, name) VALUES (@MatRowId, 'Yogamat');
INSERT INTO equipment (id, name) VALUES (@GlovesRowId, 'Bokshandschoenen');

SELECT @BikeId = id FROM equipment WHERE name = 'Fiets';
SELECT @MatId = id FROM equipment WHERE name = 'Yogamat';
SELECT @GlovesId = id FROM equipment WHERE name = 'Bokshandschoenen';

DECLARE @SpinningEquipmentRoomRowId UNIQUEIDENTIFIER = NEWID();

INSERT INTO equipmentRoom (id, roomId, [rowCount], spotsPerRow)
VALUES (@SpinningEquipmentRoomRowId, @SpinningRoomId, 4, 6);

SELECT @SpinningEquipmentRoomId = id FROM equipmentRoom WHERE roomId = @SpinningRoomId;

;WITH RowsCTE AS (
    SELECT 1 AS rowNumber UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4
), SpotsCTE AS (
    SELECT 1 AS spotNumber UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6
)
INSERT INTO equipmentSpot (equipmentId, equipmentRoomId, rowNumber, spotNumber)
SELECT @BikeId, @SpinningEquipmentRoomId, r.rowNumber, s.spotNumber
FROM RowsCTE r
CROSS JOIN SpotsCTE s;

INSERT INTO lesson (workoutId, maxCapacity, instructorId, roomId, customDuration)
VALUES (@YogaWorkoutId, 42, @InstructorId, @GroupRoom1Id, NULL);
SELECT @YogaLessonId = id FROM lesson WHERE workoutId = @YogaWorkoutId AND instructorId = @InstructorId;
INSERT INTO schedule (lessonId, startTime, startDay, repetitionCount, repetitionEndDate, registerDate)
VALUES (@YogaLessonId, '09:00', 'Monday', -1, NULL, '2026-05-01');

INSERT INTO lesson (workoutId, maxCapacity, instructorId, roomId, customDuration)
VALUES (@SpinningWorkoutId, 24, @InstructorId, @SpinningRoomId, NULL);
SELECT @SpinningLessonId = id FROM lesson WHERE workoutId = @SpinningWorkoutId AND instructorId = @InstructorId;
INSERT INTO schedule (lessonId, startTime, startDay, repetitionCount, repetitionEndDate, registerDate)
VALUES (@SpinningLessonId, '18:00', 'Tuesday', -1, NULL, '2026-05-01');

INSERT INTO lesson (workoutId, maxCapacity, instructorId, roomId, customDuration)
VALUES (@BootcampWorkoutId, 20, @InstructorId, @OutdoorRoomId, NULL);
SELECT @BootcampLessonId = id FROM lesson WHERE workoutId = @BootcampWorkoutId AND instructorId = @InstructorId;
INSERT INTO schedule (lessonId, startTime, startDay, repetitionCount, repetitionEndDate, registerDate)
VALUES (@BootcampLessonId, '19:00', 'Wednesday', -1, NULL, '2026-05-01');

INSERT INTO lesson (workoutId, maxCapacity, instructorId, roomId, customDuration)
VALUES (@BoksenWorkoutId, 20, @InstructorId, @OutdoorRoomId, NULL);
SELECT @BoksenLessonId = id FROM lesson WHERE workoutId = @BoksenWorkoutId AND instructorId = @InstructorId;
INSERT INTO schedule (lessonId, startTime, startDay, repetitionCount, repetitionEndDate, registerDate)
VALUES (@BoksenLessonId, '20:00', 'Thursday', -1, NULL, '2026-05-01');

INSERT INTO lesson (workoutId, maxCapacity, instructorId, roomId, customDuration)
VALUES (@BodyshapeWorkoutId, 32, @InstructorId, @GroupRoom2Id, NULL);
SELECT @BodyshapeLessonId = id FROM lesson WHERE workoutId = @BodyshapeWorkoutId AND instructorId = @InstructorId;
INSERT INTO schedule (lessonId, startTime, startDay, repetitionCount, repetitionEndDate, registerDate)
VALUES (@BodyshapeLessonId, '17:00', 'Friday', -1, NULL, '2026-05-01');

INSERT INTO lesson (workoutId, maxCapacity, instructorId, roomId, customDuration)
VALUES (@ClubPowerWorkoutId, 32, @InstructorId, @GroupRoom2Id, NULL);
SELECT @ClubPowerLessonId = id FROM lesson WHERE workoutId = @ClubPowerWorkoutId AND instructorId = @InstructorId;
INSERT INTO schedule (lessonId, startTime, startDay, repetitionCount, repetitionEndDate, registerDate)
VALUES (@ClubPowerLessonId, '18:00', 'Friday', -1, NULL, '2026-05-01');

INSERT INTO lesson (workoutId, maxCapacity, instructorId, roomId, customDuration)
VALUES (@XcoWorkoutId, 24, @InstructorId, @GroupRoom3Id, NULL);
SELECT @XcoLessonId = id FROM lesson WHERE workoutId = @XcoWorkoutId AND instructorId = @InstructorId;
INSERT INTO schedule (lessonId, startTime, startDay, repetitionCount, repetitionEndDate, registerDate)
VALUES (@XcoLessonId, '10:00', 'Saturday', -1, NULL, '2026-05-01');

INSERT INTO lesson (workoutId, maxCapacity, instructorId, roomId, customDuration)
VALUES (@TotalBodyWorkoutId, 24, @InstructorId, @GroupRoom3Id, NULL);
SELECT @TotalBodyWorkoutLessonId = id FROM lesson WHERE workoutId = @TotalBodyWorkoutId AND instructorId = @InstructorId;
INSERT INTO schedule (lessonId, startTime, startDay, repetitionCount, repetitionEndDate, registerDate)
VALUES (@TotalBodyWorkoutLessonId, '11:00', 'Sunday', -1, NULL, '2026-05-01');

INSERT INTO workout_equipment (workoutId, equipmentId) VALUES (@YogaWorkoutId, @MatId);
INSERT INTO workout_equipment (workoutId, equipmentId) VALUES (@SpinningWorkoutId, @BikeId);
INSERT INTO workout_equipment (workoutId, equipmentId) VALUES (@BoksenWorkoutId, @GlovesId);

INSERT INTO subscription (athleteId, subscriptionPlanId, startDate, autoRenew, [status])
VALUES (@Member1Id, @Monthly2xPlanId, '2026-05-01', 0, 'Active');
INSERT INTO subscription (athleteId, subscriptionPlanId, startDate, autoRenew, [status])
VALUES (@Member2Id, @YearlyUnlimitedPlanId, '2026-01-01', 1, 'Active');
INSERT INTO subscription (athleteId, subscriptionPlanId, startDate, autoRenew, [status])
VALUES (@Member3Id, @MonthlyUnlimitedPlanId, '2026-05-10', 0, 'Active');

DECLARE @Member1SubscriptionId UNIQUEIDENTIFIER = (SELECT id FROM subscription WHERE athleteId = @Member1Id);
DECLARE @Member2SubscriptionId UNIQUEIDENTIFIER = (SELECT id FROM subscription WHERE athleteId = @Member2Id);
DECLARE @Member3SubscriptionId UNIQUEIDENTIFIER = (SELECT id FROM subscription WHERE athleteId = @Member3Id);

INSERT INTO payment (amount, [status], payedAt, method, subscriptionId, processorId, currency)
VALUES (30.00, 'Succeeded', '2026-05-01T09:00:00', 'Wero', @Member1SubscriptionId, 'pi_demo_001', 'eur');

INSERT INTO payment (amount, [status], payedAt, method, subscriptionId, processorId, currency)
VALUES (549.00, 'Succeeded', '2026-01-01T10:00:00', 'Wero', @Member2SubscriptionId, 'pi_demo_002', 'eur');

INSERT INTO payment (amount, [status], payedAt, method, subscriptionId, processorId, currency)
VALUES (55.00, 'Pending', '2026-05-10T12:00:00', 'Wero', @Member3SubscriptionId, 'pi_demo_003', 'eur');

INSERT INTO reservation (lessonId, athleteId, reservationDate, reservedAt, isCheckedIn, [status])
VALUES (@YogaLessonId, @Member1Id, '2026-05-04', '2026-05-03T08:00:00', 1, 'CheckedIn');
SELECT @YogaReservationId = id FROM reservation WHERE lessonId = @YogaLessonId AND athleteId = @Member1Id;

INSERT INTO reservation (lessonId, athleteId, reservationDate, reservedAt, isCheckedIn, [status])
VALUES (@SpinningLessonId, @Member1Id, '2026-05-05', '2026-05-03T08:00:00', 0, 'Accepted');
SELECT @SpinningReservation1Id = id FROM reservation WHERE lessonId = @SpinningLessonId AND athleteId = @Member1Id;

INSERT INTO reservation (lessonId, athleteId, reservationDate, reservedAt, isCheckedIn, [status])
VALUES (@SpinningLessonId, @Member2Id, '2026-05-05', '2026-05-03T08:05:00', 0, 'Accepted');
SELECT @SpinningReservation2Id = id FROM reservation WHERE lessonId = @SpinningLessonId AND athleteId = @Member2Id;

INSERT INTO reservation (lessonId, athleteId, reservationDate, reservedAt, isCheckedIn, [status])
VALUES (@BootcampLessonId, @Member3Id, '2026-05-06', '2026-05-05T10:00:00', 0, 'Waitinglist');
SELECT @BootcampReservationId = id FROM reservation WHERE lessonId = @BootcampLessonId AND athleteId = @Member3Id;

INSERT INTO equipmentSpot_reservation (reservationId, lessonId, athleteId, equipmentSpotId, reservedAt)
SELECT @SpinningReservation1Id, @SpinningLessonId, @Member1Id, id, '2026-05-03T08:00:00'
FROM equipmentSpot
WHERE equipmentRoomId = @SpinningRoomId AND rowNumber = 1 AND spotNumber = 1;

INSERT INTO equipmentSpot_reservation (reservationId, lessonId, athleteId, equipmentSpotId, reservedAt)
SELECT @SpinningReservation2Id, @SpinningLessonId, @Member2Id, id, '2026-05-03T08:05:00'
FROM equipmentSpot
WHERE equipmentRoomId = @SpinningRoomId AND rowNumber = 1 AND spotNumber = 2;

-- Controle op vereisten
-- 1) De buitenlocatie moet Bootcamp en Boksen huisvesten
IF EXISTS (
    SELECT 1
    FROM workout w
    JOIN lesson l ON l.workoutId = w.id
    JOIN room r ON l.roomId = r.id
    WHERE w.name IN ('Bootcamp', 'Boksen')
      AND r.locationId <> @OutdoorAreaId
)
BEGIN
    RAISERROR('Gegevensvereiste geschonden: Bootcamp en Boksen moeten op de buitenlocatie staan', 16, 1);
END

-- 2) De hoofdlocatie moet drie groepsleszalen hebben met capaciteiten 42, 32 en 24
IF (SELECT COUNT(*) FROM room WHERE locationId = @MainHallId AND capacity IN (42, 32, 24)) <> 3
BEGIN
    RAISERROR('Gegevensvereiste geschonden: de hoofdlocatie moet drie groepsleszalen hebben met capaciteiten 42, 32 en 24', 16, 1);
END

-- 3) De buitenlocatie moet precies één ruimte met capaciteit 20 hebben
IF (SELECT COUNT(*) FROM room WHERE locationId = @OutdoorAreaId AND capacity = 20) <> 1
BEGIN
    RAISERROR('Gegevensvereiste geschonden: de buitenlocatie moet precies één ruimte met capaciteit 20 hebben', 16, 1);
END

-- 4) De spinninglocatie moet precies één ruimte met capaciteit 24 hebben
IF (SELECT COUNT(*) FROM room WHERE locationId = @SpinningLocationId AND capacity = 24) <> 1
BEGIN
    RAISERROR('Gegevensvereiste geschonden: de spinninglocatie moet precies één ruimte met capaciteit 24 hebben', 16, 1);
END

-- 5) De spinningruimte moet 4 rijen van 6 plekken hebben
IF NOT EXISTS (
    SELECT 1 FROM equipmentRoom er
    WHERE er.roomId = @SpinningRoomId AND er.[rowCount] = 4 AND er.spotsPerRow = 6
)
BEGIN
    RAISERROR('Gegevensvereiste geschonden: de spinningruimte moet 4 rijen van 6 plekken hebben', 16, 1);
END

-- 6) Alleen spinninglessen mogen equipmentSpot_reservation-records hebben
IF EXISTS (
    SELECT 1
    FROM equipmentSpot_reservation esr
    JOIN lesson l ON esr.lessonId = l.id
    JOIN workout w ON l.workoutId = w.id
    WHERE w.name <> 'Spinning'
)
BEGIN
    RAISERROR('Gegevensvereiste geschonden: alleen spinninglessen mogen equipmentSpot_reservation-records hebben', 16, 1);
END
