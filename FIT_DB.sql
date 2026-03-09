USE [fit];
GO

CREATE TABLE [users] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [uuid] UNIQUEIDENTIFIER UNIQUE DEFAULT NEWID(), -- Dùng UNIQUEIDENTIFIER tối ưu hơn cho uuid
  [email] NVARCHAR(255) UNIQUE NOT NULL,
  [password] NVARCHAR(255) NOT NULL,
  [created_at] DATETIME2 DEFAULT GETDATE(),
  [updated_at] DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE [faculties] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [name] NVARCHAR(255) NOT NULL,
  [address] NVARCHAR(255),
  [created_at] DATETIME2 DEFAULT GETDATE(),
  [updated_at] DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE [instructors] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [name] NVARCHAR(255) NOT NULL,
  [email] NVARCHAR(255) UNIQUE,
  [title] NVARCHAR(100),
  [faculty_id] INT,
  [created_at] DATETIME2 DEFAULT GETDATE(),
  [updated_at] DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE [majors] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [faculty_id] INT,
  [name] NVARCHAR(255) NOT NULL,
  [degree_level] VARCHAR(20) CHECK ([degree_level] IN ('bachelor', 'master', 'phd')),
  [required_credits] INT,
  [created_at] DATETIME2 DEFAULT GETDATE(),
  [updated_at] DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE [intakes] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [code] VARCHAR(50) NOT NULL,
  [year] INT,
  [created_at] DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE [curriculums] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [major_id] INT,
  [intake_id] INT,
  [total_credits] INT,
  [created_at] DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE [courses] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [code] VARCHAR(50) UNIQUE NOT NULL,
  [name_vi] NVARCHAR(255),
  [name_en] NVARCHAR(255),
  [credit] INT,
  [lecture_hours] INT,
  [self_study_hours] INT,
  [description] NVARCHAR(MAX),
  [created_at] DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE [course_prerequisites] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [course_id] INT,
  [prerequisite_course_id] INT
);
GO

CREATE TABLE [curriculum_courses] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [curriculum_id] INT,
  [course_id] INT,
  [semester] INT,
  [order] INT,
  [is_required] BIT,
  [group] VARCHAR(20) CHECK ([group] IN ('general', 'school', 'major', 'base', 'knowledge', 'advance', 'intern')),
  [created_at] DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE [syllabi] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [course_id] INT,
  [version] VARCHAR(50),
  [language] VARCHAR(5) CHECK ([language] IN ('vi', 'en')),
  [description] NVARCHAR(MAX),
  [adjustment_date] DATE,
  [approved_by] NVARCHAR(255),
  [created_at] DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE [syllabus_instructors] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [syllabus_id] INT,
  [instructor_id] INT
);
GO

CREATE TABLE [learning_resources] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [syllabus_id] INT,
  [type] VARCHAR(20) CHECK ([type] IN ('textbook', 'reference', 'software')),
  [title] NVARCHAR(255),
  [author] NVARCHAR(255),
  [publisher] NVARCHAR(255),
  [year] INT
);
GO

CREATE TABLE [course_goals] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [syllabus_id] INT,
  [code] VARCHAR(50),
  [description] NVARCHAR(MAX)
);
GO

CREATE TABLE [course_learning_outcomes] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [goal_id] INT,
  [code] VARCHAR(50),
  [description] NVARCHAR(MAX),
  [level] VARCHAR(5) CHECK ([level] IN ('I', 'II', 'III'))
);
GO

CREATE TABLE [assessments] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [syllabus_id] INT,
  [name] NVARCHAR(255),
  [type] VARCHAR(30) CHECK ([type] IN ('attendance', 'group_assignment', 'individual_assignment', 'final_exam')),
  [weight] INT
);
GO

CREATE TABLE [assessment_clo] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [assessment_id] INT,
  [clo_id] INT
);
GO

CREATE TABLE [lesson_weeks] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [syllabus_id] INT,
  [week] INT,
  [type] VARCHAR(20) CHECK ([type] IN ('lecture', 'seminar', 'exam')),
  [title] NVARCHAR(255),
  [content] NVARCHAR(MAX)
);
GO

CREATE TABLE [lesson_resources] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [lesson_id] INT,
  [resource_id] INT
);
GO

CREATE TABLE [lesson_clo] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [lesson_id] INT,
  [clo_id] INT
);
GO

CREATE TABLE [rubrics] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [assessment_id] INT,
  [name] NVARCHAR(255)
);
GO

CREATE TABLE [rubric_criteria] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [rubric_id] INT,
  [name] NVARCHAR(255),
  [weight] INT
);
GO

CREATE TABLE [rubric_levels] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [criteria_id] INT,
  [level_name] NVARCHAR(255),
  [min_score] FLOAT,
  [max_score] FLOAT,
  [description] NVARCHAR(MAX)
);
GO

CREATE TABLE [exam_matrix] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [syllabus_id] INT,
  [lecture_number] INT,
  [remember_questions] INT,
  [understand_questions] INT,
  [apply_questions] INT
);
GO

CREATE TABLE [course_policies] (
  [id] INT IDENTITY(1,1) PRIMARY KEY,
  [syllabus_id] INT,
  [title] NVARCHAR(255),
  [content] NVARCHAR(MAX)
);
GO

-- =============================================
-- KHỞI TẠO FOREIGN KEYS (CÓ ĐẶT TÊN RÕ RÀNG)
-- =============================================

ALTER TABLE [instructors] ADD CONSTRAINT [FK_instructors_faculties] FOREIGN KEY ([faculty_id]) REFERENCES [faculties] ([id]);
GO

ALTER TABLE [majors] ADD CONSTRAINT [FK_majors_faculties] FOREIGN KEY ([faculty_id]) REFERENCES [faculties] ([id]);
GO

ALTER TABLE [curriculums] ADD CONSTRAINT [FK_curriculums_majors] FOREIGN KEY ([major_id]) REFERENCES [majors] ([id]);
GO

ALTER TABLE [curriculums] ADD CONSTRAINT [FK_curriculums_intakes] FOREIGN KEY ([intake_id]) REFERENCES [intakes] ([id]);
GO

ALTER TABLE [course_prerequisites] ADD CONSTRAINT [FK_prereq_course] FOREIGN KEY ([course_id]) REFERENCES [courses] ([id]);
GO

ALTER TABLE [course_prerequisites] ADD CONSTRAINT [FK_prereq_required_course] FOREIGN KEY ([prerequisite_course_id]) REFERENCES [courses] ([id]);
GO

ALTER TABLE [curriculum_courses] ADD CONSTRAINT [FK_curr_courses_curriculum] FOREIGN KEY ([curriculum_id]) REFERENCES [curriculums] ([id]);
GO

ALTER TABLE [curriculum_courses] ADD CONSTRAINT [FK_curr_courses_course] FOREIGN KEY ([course_id]) REFERENCES [courses] ([id]);
GO

ALTER TABLE [syllabi] ADD CONSTRAINT [FK_syllabi_courses] FOREIGN KEY ([course_id]) REFERENCES [courses] ([id]);
GO

ALTER TABLE [syllabus_instructors] ADD CONSTRAINT [FK_syll_inst_syllabi] FOREIGN KEY ([syllabus_id]) REFERENCES [syllabi] ([id]);
GO

ALTER TABLE [syllabus_instructors] ADD CONSTRAINT [FK_syll_inst_instructors] FOREIGN KEY ([instructor_id]) REFERENCES [instructors] ([id]);
GO

ALTER TABLE [learning_resources] ADD CONSTRAINT [FK_resources_syllabi] FOREIGN KEY ([syllabus_id]) REFERENCES [syllabi] ([id]);
GO

ALTER TABLE [course_goals] ADD CONSTRAINT [FK_goals_syllabi] FOREIGN KEY ([syllabus_id]) REFERENCES [syllabi] ([id]);
GO

ALTER TABLE [course_learning_outcomes] ADD CONSTRAINT [FK_clo_goals] FOREIGN KEY ([goal_id]) REFERENCES [course_goals] ([id]);
GO

ALTER TABLE [assessments] ADD CONSTRAINT [FK_assessments_syllabi] FOREIGN KEY ([syllabus_id]) REFERENCES [syllabi] ([id]);
GO

ALTER TABLE [assessment_clo] ADD CONSTRAINT [FK_assess_clo_assessment] FOREIGN KEY ([assessment_id]) REFERENCES [assessments] ([id]);
GO

ALTER TABLE [assessment_clo] ADD CONSTRAINT [FK_assess_clo_clo] FOREIGN KEY ([clo_id]) REFERENCES [course_learning_outcomes] ([id]);
GO

ALTER TABLE [lesson_weeks] ADD CONSTRAINT [FK_lesson_syllabi] FOREIGN KEY ([syllabus_id]) REFERENCES [syllabi] ([id]);
GO

ALTER TABLE [lesson_resources] ADD CONSTRAINT [FK_lesson_res_lesson] FOREIGN KEY ([lesson_id]) REFERENCES [lesson_weeks] ([id]);
GO

ALTER TABLE [lesson_resources] ADD CONSTRAINT [FK_lesson_res_resource] FOREIGN KEY ([resource_id]) REFERENCES [learning_resources] ([id]);
GO

ALTER TABLE [lesson_clo] ADD CONSTRAINT [FK_lesson_clo_lesson] FOREIGN KEY ([lesson_id]) REFERENCES [lesson_weeks] ([id]);
GO

ALTER TABLE [lesson_clo] ADD CONSTRAINT [FK_lesson_clo_clo] FOREIGN KEY ([clo_id]) REFERENCES [course_learning_outcomes] ([id]);
GO

ALTER TABLE [rubrics] ADD CONSTRAINT [FK_rubrics_assessments] FOREIGN KEY ([assessment_id]) REFERENCES [assessments] ([id]);
GO

ALTER TABLE [rubric_criteria] ADD CONSTRAINT [FK_criteria_rubrics] FOREIGN KEY ([rubric_id]) REFERENCES [rubrics] ([id]);
GO

ALTER TABLE [rubric_levels] ADD CONSTRAINT [FK_levels_criteria] FOREIGN KEY ([criteria_id]) REFERENCES [rubric_criteria] ([id]);
GO

ALTER TABLE [exam_matrix] ADD CONSTRAINT [FK_matrix_syllabi] FOREIGN KEY ([syllabus_id]) REFERENCES [syllabi] ([id]);
GO

ALTER TABLE [course_policies] ADD CONSTRAINT [FK_policies_syllabi] FOREIGN KEY ([syllabus_id]) REFERENCES [syllabi] ([id]);
GO