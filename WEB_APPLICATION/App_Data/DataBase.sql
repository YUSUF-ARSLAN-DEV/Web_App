-- ============================================
-- DATABASE: EduNestDB
-- ============================================


-- ============================================
-- TABLE 1: User (No foreign dependencies)
-- ============================================
CREATE TABLE [dbo].[User] (
    [userId]              INT            IDENTITY (1, 1) NOT NULL,
    [userName]            NVARCHAR (50)  NULL,
    [password]            NVARCHAR (100) NULL,
    [role]                NVARCHAR (50)  NULL,
    [firstName]           NVARCHAR (50)  NULL,
    [lastName]            NVARCHAR (50)  NULL,
    [accountCreationDate] DATE           NULL,
    [activeStatus]        BIT            DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([userId] ASC),
    UNIQUE NONCLUSTERED ([userName] ASC)
);

-- ============================================
-- TABLE 2: Course (Depends on User)
-- ============================================
CREATE TABLE [dbo].[Course] (
    [courseId]          INT            IDENTITY (1, 1) NOT NULL,
    [userId]            INT            NULL,
    [courseDescription] NVARCHAR (800) NULL,
    [courseName]        NVARCHAR (100) NULL,
    [activeStatus]      BIT            NULL,
    [imageUrl]          NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([courseId] ASC),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId])
);

-- ============================================
-- TABLE 3: Lesson (Depends on Course)
-- ============================================
CREATE TABLE [dbo].[Lesson] (
    [lessonId]       INT            IDENTITY (1, 1) NOT NULL,
    [courseId]       INT            NULL,
    [lessonTitle]    NVARCHAR (150) NULL,
    [lessonContent]  NVARCHAR (MAX) NULL,
    [videoUrl]       NVARCHAR (500) NULL,
    [attachmentUrl]  NVARCHAR (500) NULL,
    [attachmentName] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([lessonId] ASC),
    FOREIGN KEY ([courseId]) REFERENCES [dbo].[Course] ([courseId])
);

-- ============================================
-- TABLE 4: Assessment (Depends on Lesson)
-- ============================================
CREATE TABLE [dbo].[Assessment] (
    [assessmentId]  INT IDENTITY (1, 1) NOT NULL,
    [lessonId]      INT NULL,
    [attemptNumber] INT NULL,
    PRIMARY KEY CLUSTERED ([assessmentId] ASC),
    FOREIGN KEY ([lessonId]) REFERENCES [dbo].[Lesson] ([lessonId])
);

-- ============================================
-- TABLE 5: Question (Depends on Assessment)
-- ============================================
CREATE TABLE [dbo].[Question] (
    [questionId]     INT            IDENTITY (1, 1) NOT NULL,
    [assessmentId]   INT            NULL,
    [questionType]   NVARCHAR (50)  NULL,
    [questionText]   NVARCHAR (MAX) NULL,
    [questionAnswer] NVARCHAR (250) NULL,
    [correctAnswer]  NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([questionId] ASC),
    FOREIGN KEY ([assessmentId]) REFERENCES [dbo].[Assessment] ([assessmentId])
);

-- ============================================
-- TABLE 6: Enrollment (Depends on User, Course)
-- ============================================
CREATE TABLE [dbo].[Enrollment] (
    [enrollmentId]   INT  IDENTITY (1, 1) NOT NULL,
    [userId]         INT  NULL,
    [courseId]       INT  NULL,
    [completionRate] INT  NULL,
    [enrollmentDate] DATE NULL,
    [activeStatus]   BIT  NULL,
    PRIMARY KEY CLUSTERED ([enrollmentId] ASC),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId]),
    FOREIGN KEY ([courseId]) REFERENCES [dbo].[Course] ([courseId])
);

-- ============================================
-- TABLE 7: Forum (Depends on Course)
-- ============================================
CREATE TABLE [dbo].[Forum] (
    [forumId]   INT            IDENTITY (1, 1) NOT NULL,
    [courseId]  INT            NULL,
    [title]     NVARCHAR (100) NULL,
    [postFlair] NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([forumId] ASC),
    FOREIGN KEY ([courseId]) REFERENCES [dbo].[Course] ([courseId])
);

-- ============================================
-- TABLE 8: Post (Depends on Forum, User)
-- ============================================
CREATE TABLE [dbo].[Post] (
    [postId]      INT            IDENTITY (1, 1) NOT NULL,
    [forumId]     INT            NULL,
    [userId]      INT            NULL,
    [title]       NVARCHAR (100) NULL,
    [textContent] NVARCHAR (MAX) NULL,
    [imageUrl]    NVARCHAR (255) NULL,
    [postDate]    DATE           NULL,
    [postTime]    TIME (7)       NULL,
    PRIMARY KEY CLUSTERED ([postId] ASC),
    FOREIGN KEY ([forumId]) REFERENCES [dbo].[Forum] ([forumId]),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId])
);

-- ============================================
-- TABLE 9: Rating (Depends on Course, User)
-- ============================================
CREATE TABLE [dbo].[Rating] (
    [ratingId]   INT            IDENTITY (1, 1) NOT NULL,
    [courseId]   INT            NULL,
    [userId]     INT            NULL,
    [score]      INT            NOT NULL,
    [comment]    NVARCHAR (500) NULL,
    [ratingDate] DATE           DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([ratingId] ASC),
    FOREIGN KEY ([courseId]) REFERENCES [dbo].[Course] ([courseId]),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId]),
    CHECK ([score]>=(1) AND [score]<=(5))
);

-- ============================================
-- TABLE 10: Session (Depends on User)
-- ============================================
CREATE TABLE [dbo].[Session] (
    [sessionId]  INT      IDENTITY (1, 1) NOT NULL,
    [userId]     INT      NULL,
    [loginDate]  DATE     NULL,
    [loginTime]  TIME (7) NULL,
    [logoutTime] TIME (7) NULL,
    PRIMARY KEY CLUSTERED ([sessionId] ASC),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId])
);

-- ============================================
-- TABLE 11: LessonCompletion (Depends on User, Lesson, Course)
-- ============================================
CREATE TABLE [dbo].[LessonCompletion] (
    [completionId] INT      IDENTITY (1, 1) NOT NULL,
    [userId]       INT      NOT NULL,
    [lessonId]     INT      NOT NULL,
    [courseId]     INT      NOT NULL,
    [completedAt]  DATETIME DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([completionId] ASC),
    UNIQUE NONCLUSTERED ([userId] ASC, [lessonId] ASC),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId]),
    FOREIGN KEY ([lessonId]) REFERENCES [dbo].[Lesson] ([lessonId]),
    FOREIGN KEY ([courseId]) REFERENCES [dbo].[Course] ([courseId])
);

-- ============================================
-- DONE!
-- ============================================

-- yusuf hamada
-- ============================================
--  Implementing a CASCAD so that when the parent record is deleted , all the child records will be automatically deleted as well. 
-- ============================================
-- we first find the  constraint name then run the SQL QUERY 
DECLARE @ConstraintName NVARCHAR(200)

SELECT TOP 1 @ConstraintName = fk.name
FROM sys.foreign_keys fk
INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
WHERE t.name = 'Question'

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Question DROP CONSTRAINT [' + @ConstraintName + ']')
    PRINT 'Dropped constraint: ' + @ConstraintName
END
ELSE
BEGIN
    PRINT 'No foreign key constraint found on Question table'
END

-- Add new CASCADE constraint
ALTER TABLE Question 
ADD CONSTRAINT FK_Question_Assessment_CASCADE
FOREIGN KEY (assessmentId) REFERENCES Assessment(assessmentId) 
ON DELETE CASCADE

PRINT 'Added CASCADE constraint successfully'

--  A new table that we are going to create for basically storing the comments on a post aka reddit style typa shiiii 
CREATE TABLE Comment (
    commentId INT IDENTITY(1,1) NOT NULL,
    postId INT NOT NULL,
    userId INT NOT NULL,
    commentText NVARCHAR(1000) NOT NULL,
    commentDate DATE NULL,
    commentTime TIME NULL,
    PRIMARY KEY (commentId),
    FOREIGN KEY (postId) REFERENCES Post(postId),
    FOREIGN KEY (userId) REFERENCES [User](userId)
)