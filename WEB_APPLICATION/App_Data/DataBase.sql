CREATE TABLE [User] (
    userId INT PRIMARY KEY IDENTITY(1,1),
    userName NVARCHAR(50) UNIQUE , 
    [password] NVARCHAR(50) , 
    role NVARCHAR(50) , 
    firstName NVARCHAR(50), 
    lastName NVARCHAR(50) , 
    accountCreationDate DATE   
);

CREATE TABLE [Session] (
    sessionId INT PRIMARY KEY IDENTITY(1,1), 
    userId INT FOREIGN KEY REFERENCES [User](userId) , 
    loginDate DATE , 
    loginTime TIME , 
    logoutTime TIME 
);

CREATE TABLE Course (
    courseId INT PRIMARY KEY IDENTITY(1,1) , 
    userId INT FOREIGN KEY REFERENCES [User](userId) , 
    courseDescription NVARCHAR(800)  , 
    courseName NVARCHAR(100) , 
    activeStatus BIT , 
    imageUrl NVARCHAR(255) 
);

CREATE TABLE Enrollment (
    enrollmentId INT PRIMARY KEY IDENTITY(1,1) , 
    userId INT FOREIGN KEY REFERENCES [User](userId) , 
    courseId INT FOREIGN KEY REFERENCES Course(courseId) , 
    completionRate FLOAT  , 
    enrollmentDate DATE , 
    activeStatus BIT  
);

CREATE TABLE Lesson (
    lessonId INT PRIMARY KEY IDENTITY(1,1), 
    courseId INT FOREIGN KEY REFERENCES Course(courseId) , 
    lessonTitle NVARCHAR(150) , 
    lessonContent NVARCHAR(MAX) 
); 

CREATE TABLE Assessment (
    assessmentId INT PRIMARY KEY IDENTITY(1,1), 
    lessonId INT  FOREIGN KEY REFERENCES Lesson(lessonId) , 
    attemptNumber INT , 
);

CREATE TABLE Question (
    questionId INT PRIMARY KEY IDENTITY(1,1) , 
    assessmentId INT FOREIGN KEY REFERENCES Assessment(assessmentId) , 
    questionType NVARCHAR(50) , 
    questionText NVARCHAR(MAX)   , 
    questionAnswer NVARCHAR(250) , 
    correctAnswer NVARCHAR(250)  
);

CREATE TABLE LessonCompletion (
    completionId INT PRIMARY KEY IDENTITY(1,1),
    userId       INT NOT NULL FOREIGN KEY REFERENCES [User](userId),
    lessonId     INT NOT NULL FOREIGN KEY REFERENCES Lesson(lessonId),
    courseId     INT NOT NULL FOREIGN KEY REFERENCES Course(courseId),
    completedAt  DATETIME DEFAULT GETDATE(),
    UNIQUE (userId, lessonId)
);

CREATE TABLE Forum (
    forumId  INT PRIMARY KEY  IDENTITY(1,1) , 
    courseId INT FOREIGN KEY REFERENCES Course(courseId) , 
    title NVARCHAR(100) , 
    postFlair NVARCHAR(250)   
);

CREATE TABLE Post(
    postId INT PRIMARY KEY IDENTITY(1,1) , 
    forumId INT FOREIGN KEY REFERENCES Forum(forumId) , 
    userId INT FOREIGN KEY REFERENCES [User](userId), 
    title NVARCHAR(100) , 
    textContent NVARCHAR(MAX) , 
    imageUrl NVARCHAR(255) , 
    postDate DATE , 
    postTime TIME 
);


CREATE TABLE Rating (
    ratingId INT PRIMARY KEY IDENTITY(1,1),
    courseId INT FOREIGN KEY REFERENCES Course(courseId),
    userId INT FOREIGN KEY REFERENCES [User](userId),
    score INT NOT NULL CHECK (score BETWEEN 1 AND 5),
    comment NVARCHAR(500),
    ratingDate DATE DEFAULT GETDATE()
);