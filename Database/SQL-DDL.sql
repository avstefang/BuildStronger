-- SQL Server DDL generated from SQL.md
-- Assumptions:
-- - SQL Server syntax
-- - ENUM-like values are implemented with CHECK constraints

IF DB_ID(N'buildstronger') IS NULL
BEGIN
    EXEC('CREATE DATABASE buildstronger');
END;

USE buildstronger;

GO

DROP TABLE IF EXISTS equipmentSpot_reservation;
GO

DROP TABLE IF EXISTS reservation;
DROP TABLE IF EXISTS payment;
DROP TABLE IF EXISTS athlete_subscription;
DROP TABLE IF EXISTS schedule;
DROP TABLE IF EXISTS lesson;
DROP TABLE IF EXISTS workout_equipment;
DROP TABLE IF EXISTS subscription;
DROP TABLE IF EXISTS subscriptionPlan;
DROP TABLE IF EXISTS equipmentSpot;
DROP TABLE IF EXISTS equipmentRoom;
DROP TABLE IF EXISTS equipment;
DROP TABLE IF EXISTS workout;
DROP TABLE IF EXISTS room;
DROP TABLE IF EXISTS location;
DROP TABLE IF EXISTS athlete;

CREATE TABLE athlete (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_athlete_id DEFAULT (NEWID()) PRIMARY KEY,
    emailAddress NVARCHAR(255) NOT NULL UNIQUE,
    firstName NVARCHAR(100) NOT NULL,
    lastName NVARCHAR(100) NOT NULL,
    [password] NVARCHAR(255) NOT NULL,
    [role] NVARCHAR(30) NOT NULL CONSTRAINT DF_athlete_role DEFAULT ('User'),
    username NVARCHAR(100) NULL UNIQUE,
    photoPath NVARCHAR(500) NULL,
    CONSTRAINT CK_athlete_role CHECK ([role] IN ('User', 'Administrator', 'Employee', 'Instructor'))
);

CREATE TABLE location (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_location_id DEFAULT (NEWID()) PRIMARY KEY,
    name NVARCHAR(150) NOT NULL UNIQUE,
    address NVARCHAR(255) NOT NULL
);

CREATE TABLE room (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_room_id DEFAULT (NEWID()) PRIMARY KEY,
    name NVARCHAR(150) NOT NULL UNIQUE,
    capacity INT NOT NULL,
    locationId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_room_location FOREIGN KEY (locationId) REFERENCES location(id),
    CONSTRAINT CK_room_capacity CHECK (capacity > 0)
);

CREATE TABLE workout (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_workout_id DEFAULT (NEWID()) PRIMARY KEY,
    name NVARCHAR(150) NOT NULL UNIQUE,
    description NVARCHAR(MAX) NOT NULL,
    duration INT NOT NULL,
    CONSTRAINT CK_workout_duration CHECK (duration > 0)
);

CREATE TABLE equipment (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_equipment_id DEFAULT (NEWID()) PRIMARY KEY,
    name NVARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE subscriptionPlan (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_subscriptionPlan_id DEFAULT (NEWID()) PRIMARY KEY,
    name NVARCHAR(150) NOT NULL UNIQUE,
    price DECIMAL(10,2) NOT NULL,
    durationInMonths INT NOT NULL,
    weeklyCreditAmount INT NOT NULL,
    paymentMethod NVARCHAR(200) NOT NULL CONSTRAINT DF_subscriptionPlan_paymentMethod DEFAULT ('Wero'),
    paymentCurrency NVARCHAR(10) NOT NULL CONSTRAINT DF_subscriptionPlan_paymentCurrency DEFAULT ('eur'),
    CONSTRAINT CK_subscriptionPlan_price CHECK (price >= 0),
    CONSTRAINT CK_subscriptionPlan_durationInMonths CHECK (durationInMonths > 0),
    CONSTRAINT CK_subscriptionPlan_weeklyCreditAmount CHECK (weeklyCreditAmount >= 0),
    CONSTRAINT CK_subscriptionPlan_paymentCurrency CHECK (paymentCurrency IN ('eur'))
);

CREATE TABLE subscription (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_subscription_id DEFAULT (NEWID()) PRIMARY KEY,
    athleteId UNIQUEIDENTIFIER NULL,
    subscriptionPlanId UNIQUEIDENTIFIER NOT NULL,
    startDate DATE NOT NULL,
    autoRenew BIT NOT NULL CONSTRAINT DF_subscription_autoRenew DEFAULT (0),
    [status] NVARCHAR(20) NOT NULL CONSTRAINT DF_subscription_status DEFAULT ('Active'),
    CONSTRAINT FK_subscription_athlete FOREIGN KEY (athleteId) REFERENCES athlete(id) ON DELETE SET NULL,
    CONSTRAINT FK_subscription_subscriptionPlan FOREIGN KEY (subscriptionPlanId) REFERENCES subscriptionPlan(id),
    CONSTRAINT CK_subscription_status CHECK ([status] IN ('Active', 'Cancelled', 'Expired', 'WaitingActivation', 'Failed'))
);

CREATE TABLE equipmentRoom (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_equipmentRoom_id DEFAULT (NEWID()) PRIMARY KEY,
    roomId UNIQUEIDENTIFIER NOT NULL,
    [rowCount] INT NOT NULL,
    spotsPerRow INT NOT NULL,
    CONSTRAINT FK_equipmentRoom_room FOREIGN KEY (roomId) REFERENCES room(id),
    CONSTRAINT CK_equipmentRoom_rowCount CHECK ([rowCount] > 0),
    CONSTRAINT CK_equipmentRoom_spotsPerRow CHECK (spotsPerRow > 0)
);

CREATE TABLE equipmentSpot (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_equipmentSpot_id DEFAULT (NEWID()) PRIMARY KEY,
    equipmentId UNIQUEIDENTIFIER NOT NULL,
    equipmentRoomId UNIQUEIDENTIFIER NOT NULL,
    rowNumber INT NOT NULL,
    spotNumber INT NOT NULL,
    CONSTRAINT FK_equipmentSpot_equipment FOREIGN KEY (equipmentId) REFERENCES equipment(id),
    CONSTRAINT FK_equipmentSpot_equipmentRoom FOREIGN KEY (equipmentRoomId) REFERENCES equipmentRoom(id),
    CONSTRAINT UQ_equipmentSpot_room_position UNIQUE (equipmentRoomId, rowNumber, spotNumber),
    CONSTRAINT CK_equipmentSpot_rowNumber CHECK (rowNumber > 0),
    CONSTRAINT CK_equipmentSpot_spotNumber CHECK (spotNumber > 0)
);

CREATE TABLE lesson (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_lesson_id DEFAULT (NEWID()) PRIMARY KEY,
    workoutId UNIQUEIDENTIFIER NOT NULL,
    maxCapacity INT NOT NULL,
    instructorId UNIQUEIDENTIFIER NULL,
    roomId UNIQUEIDENTIFIER NOT NULL,
    customDuration INT NULL,
    CONSTRAINT FK_lesson_workout FOREIGN KEY (workoutId) REFERENCES workout(id) ON DELETE CASCADE,
    CONSTRAINT FK_lesson_instructor FOREIGN KEY (instructorId) REFERENCES athlete(id),
    CONSTRAINT FK_lesson_room FOREIGN KEY (roomId) REFERENCES room(id),
    CONSTRAINT CK_lesson_maxCapacity CHECK (maxCapacity > 0)
);

CREATE TABLE workout_equipment (
    workoutId UNIQUEIDENTIFIER NOT NULL,
    equipmentId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_workout_equipment PRIMARY KEY (workoutId, equipmentId),
    CONSTRAINT FK_workout_equipment_workout FOREIGN KEY (workoutId) REFERENCES workout(id) ON DELETE CASCADE,
    CONSTRAINT FK_workout_equipment_equipment FOREIGN KEY (equipmentId) REFERENCES equipment(id)
);

CREATE TABLE schedule (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_schedule_id DEFAULT (NEWID()) PRIMARY KEY,
    lessonId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    startTime TIME NOT NULL,
    startDay NVARCHAR(20) NOT NULL,
    repetitionCount INT NOT NULL CONSTRAINT DF_schedule_repetitionCount DEFAULT (0),
    repetitionEndDate DATE NULL,
    registerDate DATE NOT NULL,
    CONSTRAINT FK_schedule_lesson FOREIGN KEY (lessonId) REFERENCES lesson(id) ON DELETE CASCADE,
    CONSTRAINT CK_schedule_startDay CHECK (startDay IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday')),
    CONSTRAINT CK_schedule_repetitionCount CHECK (repetitionCount = -1 OR repetitionCount >= 0)
);

CREATE TABLE reservation (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_reservation_id DEFAULT (NEWID()) PRIMARY KEY,
    lessonId UNIQUEIDENTIFIER NOT NULL,
    athleteId UNIQUEIDENTIFIER NOT NULL,
    reservationDate DATE NOT NULL,
    reservedAt DATETIME2 NOT NULL CONSTRAINT DF_reservation_reservedAt DEFAULT (SYSDATETIME()),
    isCheckedIn BIT NOT NULL CONSTRAINT DF_reservation_isCheckedIn DEFAULT (0),
    [status] NVARCHAR(20) NOT NULL,
    CONSTRAINT FK_reservation_lesson FOREIGN KEY (lessonId) REFERENCES lesson(id) ON DELETE CASCADE,
    CONSTRAINT FK_reservation_athlete FOREIGN KEY (athleteId) REFERENCES athlete(id) ON DELETE CASCADE,
    CONSTRAINT UQ_reservation_lesson_athlete UNIQUE (lessonId, athleteId),
    CONSTRAINT CK_reservation_status CHECK ([status] IN ('Cancelled', 'Waitinglist', 'Accepted', 'CheckedIn', 'CheckedOut'))
);

CREATE TABLE payment (
    id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_payment_id DEFAULT (NEWID()) PRIMARY KEY,
    amount DECIMAL(10,2) NOT NULL,
    [status] NVARCHAR(20) NOT NULL CONSTRAINT DF_payment_status DEFAULT ('Pending'),
    payedAt DATETIME2 NOT NULL,
    method NVARCHAR(20) NOT NULL CONSTRAINT DF_payment_method DEFAULT ('Wero'),
    subscriptionId UNIQUEIDENTIFIER NOT NULL,
    processorId NVARCHAR(255) NOT NULL,
    currency NVARCHAR(10) NOT NULL CONSTRAINT DF_payment_currency DEFAULT ('eur'),
    CONSTRAINT FK_payment_subscription FOREIGN KEY (subscriptionId) REFERENCES subscription(id) ON DELETE CASCADE,
    CONSTRAINT CK_payment_status CHECK ([status] IN ('Pending', 'Succeeded', 'Failed', 'Refunded', 'Cancelled')),
    CONSTRAINT CK_payment_method CHECK (method IN ('Wero')),
    CONSTRAINT CK_payment_currency CHECK (currency IN ('eur')),
    CONSTRAINT CK_payment_amount CHECK (amount >= 0)
);

CREATE TABLE equipmentSpot_reservation (
    reservationId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    lessonId UNIQUEIDENTIFIER NOT NULL,
    athleteId UNIQUEIDENTIFIER NOT NULL,
    equipmentSpotId UNIQUEIDENTIFIER NOT NULL,
    reservedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_equipmentSpot_reservation_reservation FOREIGN KEY (reservationId) REFERENCES reservation(id) ON DELETE CASCADE,
    CONSTRAINT FK_equipmentSpot_reservation_lesson FOREIGN KEY (lessonId) REFERENCES lesson(id),
    CONSTRAINT FK_equipmentSpot_reservation_athlete FOREIGN KEY (athleteId) REFERENCES athlete(id),
    CONSTRAINT FK_equipmentSpot_reservation_equipmentSpot FOREIGN KEY (equipmentSpotId) REFERENCES equipmentSpot(id),
    CONSTRAINT UQ_equipmentSpot_reservation_lesson_athlete UNIQUE (lessonId, athleteId),
    CONSTRAINT UQ_equipmentSpot_reservation_lesson_spot UNIQUE (lessonId, equipmentSpotId),
    CONSTRAINT UQ_equipmentSpot_reservation_lesson_reservation UNIQUE (lessonId, reservationId)
);