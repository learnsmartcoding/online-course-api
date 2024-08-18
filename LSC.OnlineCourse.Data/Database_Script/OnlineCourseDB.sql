IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'OnlineCourseDB')
BEGIN
    -- Create the database
    CREATE DATABASE OnlineCourseDB;
END
ELSE
BEGIN
   DROP DATABASE OnlineCourseDB;
END

Go
use OnlineCourseDB
go

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO


-- UserProfile Table
CREATE TABLE UserProfile (
    UserId INT IDENTITY(1,1),
    DisplayName NVARCHAR(100) NOT NULL CONSTRAINT DF_UserProfile_DisplayName DEFAULT 'Guest',
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    AdObjId NVARCHAR(128) NOT NULL,
    CONSTRAINT PK_UserProfile_UserId PRIMARY KEY (UserId)
);

-- Roles Table
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1),    
    RoleName NVARCHAR(50) NOT NULL,    
    CONSTRAINT PK_Roles_RoleId PRIMARY KEY (RoleId)
);

-- UserRole Table
CREATE TABLE UserRole (
    UserRoleId INT IDENTITY(1,1),
    RoleId INT NOT NULL,
    UserId INT NOT NULL,        
    CONSTRAINT PK_UserRole_UserRoleId PRIMARY KEY (UserRoleId),
    CONSTRAINT FK_UserRole_UserProfile FOREIGN KEY (UserId) REFERENCES UserProfile(UserId),
    CONSTRAINT FK_UserRole_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- CourseCategory Table
CREATE TABLE CourseCategory (
    CategoryId INT IDENTITY(1,1),
    CategoryName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(250),
    CONSTRAINT PK_CourseCategory_CategoryId PRIMARY KEY (CategoryId)
);

-- Instructor Table
CREATE TABLE Instructor (
    InstructorId INT IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Bio NVARCHAR(MAX),
	UserId INT NOT NULL,  
    CONSTRAINT PK_Instructor_InstructorId PRIMARY KEY (InstructorId),
	CONSTRAINT FK_Instructor_UserProfile FOREIGN KEY (UserId) REFERENCES UserProfile(UserId)
);

-- Course Table
CREATE TABLE Course (
    CourseId INT IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    CourseType NVARCHAR(10) NOT NULL CHECK (CourseType IN ('Online', 'Offline')),
    SeatsAvailable INT CHECK (SeatsAvailable >= 0),
    Duration DECIMAL(5, 2) NOT NULL, -- Duration in hours
    CategoryId INT NOT NULL,
    InstructorId INT NOT NULL,
    StartDate DATETIME, -- Applicable for Online courses
    EndDate DATETIME, -- Applicable for Online courses
    CONSTRAINT PK_Course_CourseId PRIMARY KEY (CourseId),
    CONSTRAINT FK_Course_CourseCategory FOREIGN KEY (CategoryId) REFERENCES CourseCategory(CategoryId),
    CONSTRAINT FK_Course_Instructor FOREIGN KEY (InstructorId) REFERENCES Instructor(InstructorId)
);

-- SessionDetails Table
CREATE TABLE SessionDetails (
    SessionId INT IDENTITY(1,1),
    CourseId INT NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    VideoUrl NVARCHAR(500),
    VideoOrder INT NOT NULL,
    CONSTRAINT PK_SessionDetails_SessionId PRIMARY KEY (SessionId),
    CONSTRAINT FK_SessionDetails_Course FOREIGN KEY (CourseId) REFERENCES Course(CourseId)
);

-- Enrollment Table
CREATE TABLE Enrollment (
    EnrollmentId INT IDENTITY(1,1),
    CourseId INT NOT NULL,
    UserId INT NOT NULL,
    EnrollmentDate DATETIME NOT NULL DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(20) NOT NULL CHECK (PaymentStatus IN ('Pending', 'Completed', 'Failed')),
    CONSTRAINT PK_Enrollment_EnrollmentId PRIMARY KEY (EnrollmentId),
    CONSTRAINT FK_Enrollment_Course FOREIGN KEY (CourseId) REFERENCES Course(CourseId),
    CONSTRAINT FK_Enrollment_UserProfile FOREIGN KEY (UserId) REFERENCES UserProfile(UserId)
);

-- Payment Table
CREATE TABLE Payment (
    PaymentId INT IDENTITY(1,1),
    EnrollmentId INT NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
    PaymentMethod NVARCHAR(50) NOT NULL,
    PaymentStatus NVARCHAR(20) NOT NULL CHECK (PaymentStatus IN ('Pending', 'Completed', 'Failed')),
    CONSTRAINT PK_Payment_PaymentId PRIMARY KEY (PaymentId),
    CONSTRAINT FK_Payment_Enrollment FOREIGN KEY (EnrollmentId) REFERENCES Enrollment(EnrollmentId)
);

-- Review Table
CREATE TABLE Review (
    ReviewId INT IDENTITY(1,1),
    CourseId INT NOT NULL,
    UserId INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comments NVARCHAR(MAX),
    ReviewDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Review_ReviewId PRIMARY KEY (ReviewId),
    CONSTRAINT FK_Review_Course FOREIGN KEY (CourseId) REFERENCES Course(CourseId),
    CONSTRAINT FK_Review_UserProfile FOREIGN KEY (UserId) REFERENCES UserProfile(UserId)
);


COMMIT;
GO

-- Sample data to start with --

INSERT INTO UserProfile (DisplayName, FirstName, LastName, Email, AdObjId)
VALUES 
('John Doe', 'John', 'Doe', 'john.doe@example.com', 'ad-obj-id-001'),
('Jane Smith', 'Jane', 'Smith', 'jane.smith@example.com', 'ad-obj-id-002'),
('Alice Johnson', 'Alice', 'Johnson', 'alice.johnson@example.com', 'ad-obj-id-003');

INSERT INTO Roles (RoleName)
VALUES 
('Admin'),
('Instructor'),
('Student');

INSERT INTO UserRole (RoleId, UserId)
VALUES 
(1, 1), -- John Doe as Admin
(2, 2), -- Jane Smith as Instructor
(3, 3); -- Alice Johnson as Student

INSERT INTO CourseCategory (CategoryName, Description)
VALUES 
('Programming', 'Courses related to software development and programming languages.'),
('Data Science', 'Courses covering data analysis, machine learning, and AI.'),
('Design', 'Courses related to graphic design, UX/UI, and creative fields.');

INSERT INTO Instructor (FirstName, LastName, Email, Bio, UserId)
VALUES 
('Jane', 'Smith', 'jane.smith@example.com', 'Experienced software engineer with 10 years in the industry.', 2);

INSERT INTO Course (Title, Description, Price, CourseType, SeatsAvailable, Duration, CategoryId, InstructorId, StartDate, EndDate)
VALUES 
('Angular Full Course', 'Comprehensive course covering Angular from basics to advanced topics.', 199.99, 'Online', 50, 40.5, 1, 1, '2024-09-01', '2024-09-30'),
('Introduction to Data Science', 'Learn the fundamentals of data science with hands-on examples.', 149.99, 'Offline', NULL, 30.0, 2, 1, NULL, NULL),
('Graphic Design Mastery', 'Master the art of graphic design with practical projects.', 129.99, 'Offline', NULL, 25.0, 3, 1, NULL, NULL);

INSERT INTO SessionDetails (CourseId, Title, Description, VideoUrl, VideoOrder)
VALUES 
(1, 'Introduction to Angular', 'Overview of Angular and its core concepts.', 'https://example.com/angular-intro', 1),
(1, 'Angular Components', 'Deep dive into Angular components.', 'https://example.com/angular-components', 2),
(1, 'Angular Services and Dependency Injection', 'Learn how to create and use services in Angular.', 'https://example.com/angular-services', 3),
(1, 'Routing in Angular', 'Understanding routing and navigation in Angular.', 'https://example.com/angular-routing', 4),
(2, 'Data Science Introduction', 'Introduction to data science and its applications.', 'https://example.com/data-science-intro', 1),
(2, 'Python for Data Science', 'Using Python for data analysis and visualization.', 'https://example.com/python-data-science', 2),
(2, 'Machine Learning Basics', 'Introduction to machine learning concepts.', 'https://example.com/ml-basics', 3),
(3, 'Introduction to Graphic Design', 'Overview of graphic design principles.', 'https://example.com/graphic-design-intro', 1),
(3, 'Typography and Layout', 'Learn the importance of typography and layout in design.', 'https://example.com/typography-layout', 2),
(3, 'Advanced Photoshop Techniques', 'Master advanced techniques in Adobe Photoshop.', 'https://example.com/photoshop-techniques', 3);

INSERT INTO Enrollment (CourseId, UserId, EnrollmentDate, PaymentStatus)
VALUES 
(1, 3, GETDATE(), 'Completed'),
(2, 3, GETDATE(), 'Pending'),
(3, 1, GETDATE(), 'Completed');

INSERT INTO Payment (EnrollmentId, Amount, PaymentDate, PaymentMethod, PaymentStatus)
VALUES 
(1, 199.99, GETDATE(), 'Credit Card', 'Completed'),
(2, 149.99, GETDATE(), 'Credit Card', 'Pending'),
(3, 129.99, GETDATE(), 'Credit Card', 'Completed');

INSERT INTO Review (CourseId, UserId, Rating, Comments, ReviewDate)
VALUES 
(1, 3, 5, 'Excellent course, highly recommended!', GETDATE()),
(2, 3, 4, 'Great content, but could use more examples.', GETDATE()),
(3, 1, 5, 'Loved the hands-on projects and practical examples.', GETDATE());


