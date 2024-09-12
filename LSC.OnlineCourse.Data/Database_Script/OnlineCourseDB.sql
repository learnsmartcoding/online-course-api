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


-- SmartApp Table
CREATE TABLE SmartApp (
    SmartAppId INT IDENTITY(1,1),    
    AppName NVARCHAR(50) NOT NULL,    
    CONSTRAINT PK_SmartApp_SmartAppId PRIMARY KEY (SmartAppId)
);

-- UserRole Table
CREATE TABLE UserRole (
    UserRoleId INT IDENTITY(1,1),
    RoleId INT NOT NULL,
    UserId INT NOT NULL,    
	SmartAppId INT NOT NULL,
    CONSTRAINT PK_UserRole_UserRoleId PRIMARY KEY (UserRoleId),
    CONSTRAINT FK_UserRole_UserProfile FOREIGN KEY (UserId) REFERENCES UserProfile(UserId),
    CONSTRAINT FK_UserRole_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
	CONSTRAINT FK_UserRole_SmartApp FOREIGN KEY (SmartAppId) REFERENCES SmartApp(SmartAppId)
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


INSERT INTO SmartApp (AppName)
VALUES 
('Online_Course'),
('Expense_Tracker');

DECLARE @appId int = (select SmartAppId from SmartApp where AppName= 'Online_Course')
INSERT INTO UserRole (RoleId, UserId, SmartAppId)
VALUES 
(1, 1,@appId), -- John Doe as Admin
(2, 2,@appId), -- Jane Smith as Instructor
(3, 3, @appId); -- Alice Johnson as Student


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


--Dataset 2
INSERT INTO UserProfile (DisplayName, FirstName, LastName, Email, AdObjId)
VALUES 
('Michael Brown', 'Michael', 'Brown', 'michael.brown@example.com', 'ad-obj-id-004'),
('Laura White', 'Laura', 'White', 'laura.white@example.com', 'ad-obj-id-005'),
('David Green', 'David', 'Green', 'david.green@example.com', 'ad-obj-id-006');

INSERT INTO Roles (RoleName)
VALUES 
('Admin'),
('Instructor'),
('Student');

INSERT INTO UserRole (RoleId, UserId, SmartAppId)
VALUES 
(1, 4,@appId), -- Michael Brown as Admin
(2, 5,@appId), -- Laura White as Instructor
(3, 6,@appId); -- David Green as Student

INSERT INTO CourseCategory (CategoryName, Description)
VALUES 
('Web Development', 'Courses focusing on front-end and back-end web development.'),
('Cybersecurity', 'Courses covering security practices and ethical hacking.'),
('Project Management', 'Courses on managing projects, teams, and resources effectively.');

INSERT INTO Instructor (FirstName, LastName, Email, Bio, UserId)
VALUES 
('Laura', 'White', 'laura.white@example.com', 'Certified web developer and instructor with 8 years of experience.', 5);

INSERT INTO Course (Title, Description, Price, CourseType, SeatsAvailable, Duration, CategoryId, InstructorId, StartDate, EndDate)
VALUES 
('Full Stack Web Development', 'Learn to build web applications from scratch using modern technologies.', 249.99, 'Online', 40, 50.0, 4, 2, '2024-10-01', '2024-11-30'),
('Ethical Hacking Basics', 'Introduction to ethical hacking and cybersecurity fundamentals.', 199.99, 'Online', 35, 40.0, 5, 2, '2024-10-01', '2024-11-30'),
('Agile Project Management', 'Master the principles of Agile and Scrum.', 179.99, 'Offline', NULL, 30.0, 6, 2, NULL, NULL);

INSERT INTO SessionDetails (CourseId, Title, Description, VideoUrl, VideoOrder)
VALUES 
(4, 'Introduction to Web Development', 'Overview of web development and technologies.', 'https://example.com/web-development-intro', 1),
(4, 'Building APIs with Node.js', 'Learn to create APIs using Node.js.', 'https://example.com/nodejs-apis', 2),
(4, 'Frontend Development with React', 'Learn React for building user interfaces.', 'https://example.com/react-frontend', 3),
(5, 'Introduction to Cybersecurity', 'Overview of cybersecurity concepts and practices.', 'https://example.com/cybersecurity-intro', 1),
(5, 'Network Security Fundamentals', 'Learn about securing network infrastructure.', 'https://example.com/network-security', 2),
(5, 'Ethical Hacking Techniques', 'Introduction to ethical hacking tools and techniques.', 'https://example.com/ethical-hacking', 3),
(6, 'Introduction to Agile', 'Overview of Agile project management.', 'https://example.com/agile-intro', 1),
(6, 'Scrum Framework', 'Learn about the Scrum framework and roles.', 'https://example.com/scrum-framework', 2),
(6, 'Agile Tools and Techniques', 'Tools and techniques for Agile project management.', 'https://example.com/agile-tools', 3);

INSERT INTO Enrollment (CourseId, UserId, EnrollmentDate, PaymentStatus)
VALUES 
(4, 6, GETDATE(), 'Completed'),
(5, 6, GETDATE(), 'Pending'),
(6, 4, GETDATE(), 'Completed');

INSERT INTO Payment (EnrollmentId, Amount, PaymentDate, PaymentMethod, PaymentStatus)
VALUES 
(1, 249.99, GETDATE(), 'Credit Card', 'Completed'),
(2, 199.99, GETDATE(), 'Credit Card', 'Pending'),
(3, 179.99, GETDATE(), 'Credit Card', 'Completed');

INSERT INTO Review (CourseId, UserId, Rating, Comments, ReviewDate)
VALUES 
(4, 6, 5, 'Fantastic course with great content!', GETDATE()),
(5, 6, 4, 'Very informative but could have more hands-on examples.', GETDATE()),
(6, 4, 5, 'Great for understanding Agile principles.', GETDATE());


-- Dataset 3
INSERT INTO UserProfile (DisplayName, FirstName, LastName, Email, AdObjId)
VALUES 
('Emma Wilson', 'Emma', 'Wilson', 'emma.wilson@example.com', 'ad-obj-id-007'),
('Chris Taylor', 'Chris', 'Taylor', 'chris.taylor@example.com', 'ad-obj-id-008'),
('Sophia Davis', 'Sophia', 'Davis', 'sophia.davis@example.com', 'ad-obj-id-009');

INSERT INTO Roles (RoleName)
VALUES 
('Admin'),
('Instructor'),
('Student');

INSERT INTO UserRole (RoleId, UserId, SmartAppId)
VALUES 
(1, 7,@appId), -- Emma Wilson as Admin
(2, 8,@appId), -- Chris Taylor as Instructor
(3, 9,@appId); -- Sophia Davis as Student

INSERT INTO CourseCategory (CategoryName, Description)
VALUES 
('Marketing', 'Courses focused on digital marketing, SEO, and social media marketing.'),
('Finance', 'Courses covering investment, personal finance, and accounting.'),
('Health & Fitness', 'Courses on physical fitness, mental well-being, and nutrition.');

INSERT INTO Instructor (FirstName, LastName, Email, Bio, UserId)
VALUES 
('Chris', 'Taylor', 'chris.taylor@example.com', 'Marketing expert with over 15 years of experience in the industry.', 8);

INSERT INTO Course (Title, Description, Price, CourseType, SeatsAvailable, Duration, CategoryId, InstructorId, StartDate, EndDate)
VALUES 
('Digital Marketing Mastery', 'Become an expert in digital marketing with this comprehensive course.', 219.99, 'Online', 30, 35.0, 7, 3, '2024-11-01', '2024-12-01'),
('Personal Finance Essentials', 'Learn how to manage your finances and make smart investments.', 179.99, 'Offline', NULL, 25.0, 8, 3, NULL, NULL),
('Yoga for Beginners', 'Start your journey to physical and mental wellness with yoga.', 99.99, 'Online', 20, 15.0, 9, 3, '2024-11-01', '2024-11-15');

INSERT INTO SessionDetails (CourseId, Title, Description, VideoUrl, VideoOrder)
VALUES 
(7, 'Introduction to Digital Marketing', 'Learn the basics of digital marketing.', 'https://example.com/digital-marketing-intro', 1),
(7, 'SEO Best Practices', 'Optimize your content for search engines.', 'https://example.com/seo-best-practices', 2),
(7, 'Social Media Marketing', 'Leverage social media for brand growth.', 'https://example.com/social-media-marketing', 3),
(8, 'Basics of Personal Finance', 'Understanding the fundamentals of personal finance.', 'https://example.com/personal-finance-basics', 1),
(8, 'Investment Strategies', 'Learn different strategies for smart investing.', 'https://example.com/investment-strategies', 2),
(8, 'Budgeting and Saving', 'Tips for effective budgeting and saving.', 'https://example.com/budgeting-saving', 3),
(9, 'Introduction to Yoga', 'Basics of yoga and its benefits.', 'https://example.com/yoga-intro', 1),
(9, 'Beginner Yoga Poses', 'Learn simple yoga poses for beginners.', 'https://example.com/beginner-yoga-poses', 2),
(9, 'Meditation and Relaxation', 'Incorporate meditation into your daily routine.', 'https://example.com/meditation-relaxation', 3);

INSERT INTO Enrollment (CourseId, UserId, EnrollmentDate, PaymentStatus)
VALUES 
(7, 9, GETDATE(), 'Completed'),
(8, 9, GETDATE(), 'Pending'),
(9, 7, GETDATE(), 'Completed');

INSERT INTO Payment (EnrollmentId, Amount, PaymentDate, PaymentMethod, PaymentStatus)
VALUES 
(1, 219.99, GETDATE(), 'Credit Card', 'Completed'),
(2, 179.99, GETDATE(), 'Credit Card', 'Pending'),
(3, 99.99, GETDATE(), 'Credit Card', 'Completed');

INSERT INTO Review (CourseId, UserId, Rating, Comments, ReviewDate)
VALUES 
(7, 9, 5, 'Highly recommend for anyone wanting to learn digital marketing!', GETDATE()),
(8, 9, 4, 'Very helpful, but could include more advanced topics.', GETDATE()),
(9, 7, 5, 'Great introduction to yoga, very relaxing.', GETDATE());


--Dataset for courses
-- Course 1: Full Stack Web Development
INSERT INTO Course (Title, Description, Price, CourseType, SeatsAvailable, Duration, CategoryId, InstructorId, StartDate, EndDate)
VALUES 
('Full Stack Web Development', 'Learn to build complete web applications using HTML, CSS, JavaScript, and backend technologies.', 249.99, 'Online', 75, 60.0, 1, 1, '2024-10-01', '2024-12-01');

---- Sessions for Course 1
--INSERT INTO SessionDetails (CourseId, Title, Description, VideoUrl, VideoOrder)
--VALUES 
--((SELECT CourseId FROM Course WHERE Title = 'Full Stack Web Development'), 'Introduction to Web Development', 'Overview of web development and technologies involved.', 'https://example.com/fullstack-intro', 1),
--((SELECT CourseId FROM Course WHERE Title = 'Full Stack Web Development'), 'Frontend Development Basics', 'Introduction to HTML, CSS, and JavaScript.', 'https://example.com/fullstack-frontend', 2),
--((SELECT CourseId FROM Course WHERE Title = 'Full Stack Web Development'), 'Backend Development Basics', 'Introduction to server-side programming and databases.', 'https://example.com/fullstack-backend', 3),
--((SELECT CourseId FROM Course WHERE Title = 'Full Stack Web Development'), 'Deploying Web Applications', 'How to deploy web applications to the cloud.', 'https://example.com/fullstack-deployment', 4);

-- Course 2: Advanced Data Science Techniques
INSERT INTO Course (Title, Description, Price, CourseType, SeatsAvailable, Duration, CategoryId, InstructorId, StartDate, EndDate)
VALUES 
('Advanced Data Science Techniques', 'Dive deep into advanced techniques in data science, including deep learning and AI.', 299.99, 'Online', 50, 45.0, 2, 1, '2024-11-01', '2024-12-31');

-- Sessions for Course 2
INSERT INTO SessionDetails (CourseId, Title, Description, VideoUrl, VideoOrder)
VALUES 
((SELECT CourseId FROM Course WHERE Title = 'Advanced Data Science Techniques'), 'Deep Learning Overview', 'Introduction to deep learning and neural networks.', 'https://example.com/ds-deeplearning', 1),
((SELECT CourseId FROM Course WHERE Title = 'Advanced Data Science Techniques'), 'Convolutional Neural Networks', 'Understanding CNNs for image processing.', 'https://example.com/ds-cnn', 2),
((SELECT CourseId FROM Course WHERE Title = 'Advanced Data Science Techniques'), 'Natural Language Processing', 'NLP techniques and applications.', 'https://example.com/ds-nlp', 3),
((SELECT CourseId FROM Course WHERE Title = 'Advanced Data Science Techniques'), 'AI in Production', 'Deploying AI models in real-world applications.', 'https://example.com/ds-ai-production', 4);

-- Course 3: UI/UX Design Fundamentals
INSERT INTO Course (Title, Description, Price, CourseType, SeatsAvailable, Duration, CategoryId, InstructorId, StartDate, EndDate)
VALUES 
('UI/UX Design Fundamentals', 'Learn the essentials of user interface and user experience design.', 179.99, 'Offline', NULL, 30.0, 3, 1, '2024-10-15', '2024-11-15');

-- Sessions for Course 3
INSERT INTO SessionDetails (CourseId, Title, Description, VideoUrl, VideoOrder)
VALUES 
((SELECT CourseId FROM Course WHERE Title = 'UI/UX Design Fundamentals'), 'Introduction to UI/UX', 'Overview of UI/UX design principles and importance.', 'https://example.com/uiux-intro', 1),
((SELECT CourseId FROM Course WHERE Title = 'UI/UX Design Fundamentals'), 'Wireframing and Prototyping', 'Creating wireframes and prototypes for designs.', 'https://example.com/uiux-wireframing', 2),
((SELECT CourseId FROM Course WHERE Title = 'UI/UX Design Fundamentals'), 'Design Systems and Style Guides', 'Establishing design systems and style guides for consistency.', 'https://example.com/uiux-designsystems', 3),
((SELECT CourseId FROM Course WHERE Title = 'UI/UX Design Fundamentals'), 'Usability Testing', 'Methods and tools for usability testing.', 'https://example.com/uiux-usability', 4);

-- Course 4: Python for Data Analysis
INSERT INTO Course (Title, Description, Price, CourseType, SeatsAvailable, Duration, CategoryId, InstructorId, StartDate, EndDate)
VALUES 
('Python for Data Analysis', 'Learn how to use Python for data analysis, visualization, and statistical modeling.', 159.99, 'Online', 60, 35.0, 2, 1, '2024-09-15', '2024-10-15');

-- Sessions for Course 4
INSERT INTO SessionDetails (CourseId, Title, Description, VideoUrl, VideoOrder)
VALUES 
((SELECT CourseId FROM Course WHERE Title = 'Python for Data Analysis'), 'Introduction to Python', 'Getting started with Python programming.', 'https://example.com/python-intro', 1),
((SELECT CourseId FROM Course WHERE Title = 'Python for Data Analysis'), 'Data Manipulation with Pandas', 'Using Pandas for data manipulation and cleaning.', 'https://example.com/python-pandas', 2),
((SELECT CourseId FROM Course WHERE Title = 'Python for Data Analysis'), 'Data Visualization with Matplotlib', 'Creating visualizations with Matplotlib and Seaborn.', 'https://example.com/python-visualization', 3),
((SELECT CourseId FROM Course WHERE Title = 'Python for Data Analysis'), 'Statistical Modeling', 'Applying statistical models using Python.', 'https://example.com/python-statistics', 4);

-- Course 5: Digital Marketing Masterclass
INSERT INTO Course (Title, Description, Price, CourseType, SeatsAvailable, Duration, CategoryId, InstructorId, StartDate, EndDate)
VALUES 
('Digital Marketing Masterclass', 'Comprehensive guide to mastering digital marketing strategies.', 199.99, 'Online', 40, 25.0, 3, 1, '2024-12-01', '2024-12-31');

-- Sessions for Course 5
INSERT INTO SessionDetails (CourseId, Title, Description, VideoUrl, VideoOrder)
VALUES 
((SELECT CourseId FROM Course WHERE Title = 'Digital Marketing Masterclass'), 'Introduction to Digital Marketing', 'Overview of digital marketing and its components.', 'https://example.com/dm-intro', 1),
((SELECT CourseId FROM Course WHERE Title = 'Digital Marketing Masterclass'), 'SEO Strategies', 'Optimizing content for search engines.', 'https://example.com/dm-seo', 2),
((SELECT CourseId FROM Course WHERE Title = 'Digital Marketing Masterclass'), 'Social Media Marketing', 'Using social media platforms for marketing.', 'https://example.com/dm-socialmedia', 3),
((SELECT CourseId FROM Course WHERE Title = 'Digital Marketing Masterclass'), 'Email Marketing Campaigns', 'Designing and executing email marketing campaigns.', 'https://example.com/dm-email', 4);


update SessionDetails set VideoUrl = 'https://www.youtube.com/watch?v=70q8PanGbnw'
where CourseId=1

update SessionDetails set 
VideoUrl = 'https://www.youtube.com/watch?v=fqI0ToX1nDQ'
where CourseId<>1


---- These below designed after Episode 15 ------

  CREATE TABLE VideoRequest (
    VideoRequestId INT IDENTITY(1,1),    
    UserId INT NOT NULL, 
	Topic NVARCHAR(50) NOT NULL, 
	SubTopic NVARCHAR(50) NOT NULL, 
    RequestStatus NVARCHAR(50) NOT NULL, 
	ShortTitle NVARCHAR(200) NOT NULL, 
	RequestDescription NVARCHAR(4000) NOT NULL, 
	Response NVARCHAR(4000) NULL, 
	VideoUrls NVARCHAR(2000) NULL, 
    CONSTRAINT PK_VideoRequest_VideoRequestId PRIMARY KEY (VideoRequestId),
    CONSTRAINT FK_VideoRequest_UserProfile FOREIGN KEY (UserId) REFERENCES UserProfile(UserId)
);


-- below is used to trigger a azure function when a table's record modified or added
--https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-azure-sql-trigger?tabs=isolated-process%2Cpython-v2%2Cportal&pivots=programming-language-csharp
Go
ALTER DATABASE OnlineCourseDB
SET CHANGE_TRACKING = ON
(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON);

ALTER TABLE [dbo].[VideoRequest]
ENABLE CHANGE_TRACKING;
-- ends here