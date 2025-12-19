using Domain.Entities;
using Domain.enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data
{
    public static class EducationDbContextModelCreating
    {
        public static void Apply(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.HasPostgresEnum(schema: "public", name: "exam_status", ["Submitted", "Graded", "Pending"]);
            modelBuilder.HasPostgresEnum(schema: "public", name: "payment_status", ["Pending", "Completed", "Failed"]);
            modelBuilder.HasPostgresEnum("public", "UsageCategory", ["ProfilePicture", "Thumbnail"]);

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("users");
                b.HasKey(x => x.Id).HasName("users_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.FullName).HasColumnName("full_name").IsRequired();
                b.Property(x => x.Ssn).HasColumnName("ssn").IsRequired();
                b.Property(x => x.PhoneNumber).HasColumnName("phone_number").IsRequired();
                b.Property(x => x.GmailExternal).HasColumnName("gmail_external");
                b.Property(x => x.PersonalPictureUrl).HasColumnName("personal_picture_url");
                b.Property(x => x.DateOfBirth).HasColumnName("date_of_birth");
                b.Property(x => x.Gender).HasColumnName("gender").IsRequired();
                b.Property(x => x.LocationMaps).HasColumnName("location_maps");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasIndex(x => x.Ssn).IsUnique();
                b.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<Student>(b =>
            {
                b.ToTable("students");
                b.HasKey(x => x.UserId).HasName("students_pkey");
                b.Property(x => x.UserId).HasColumnName("user_id");
                b.Property(x => x.DeviceId).HasColumnName("device_id");
                b.Property(x => x.TriedScreenshot).HasColumnName("tried_screenshot");
                b.Property(x => x.EducationYearId).HasColumnName("education_year_id").IsRequired();
                b.Property(x => x.ParentPhoneNumber).HasColumnName("parent_phone_number").IsRequired();
                b.HasOne(x => x.EducationYear).WithMany(e => e.Students).HasForeignKey(s => s.EducationYearId).IsRequired();
                b.HasOne(x => x.User).WithOne(x => x.Student).HasForeignKey<Student>(x => x.UserId).HasConstraintName("students_user_id_fkey");
            });

            modelBuilder.Entity<EducationYear>(b =>
            {
                b.ToTable("education_years");
                b.HasKey(x => x.Id).HasName("education_years_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.EducationYearName).HasColumnName("year_name").IsRequired();
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<Instructor>(b =>
            {
                b.ToTable("instructors");
                b.HasKey(x => x.UserId).HasName("instructors_pkey");
                b.Property(x => x.UserId).HasColumnName("user_id");
                b.HasOne(x => x.User).WithOne(x => x.Instructor).HasForeignKey<Instructor>(x => x.UserId).HasConstraintName("instructors_user_id_fkey");
            });

            modelBuilder.Entity<Course>(b =>
            {
                b.ToTable("courses");
                b.HasKey(x => x.Id).HasName("courses_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("name").IsRequired();
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.Price).HasColumnName("price");
                b.Property(x => x.PictureUrl).HasColumnName("picture_url");
                b.Property(x => x.IntroVideoUrl).HasColumnName("intro_video_url");
                b.Property(x => x.NumberOfSections).HasColumnName("number_of_sections");
                b.Property(x => x.NumberOfVideos).HasColumnName("number_of_videos");
                b.Property(x => x.NumberOfExams).HasColumnName("number_of_exams");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<Section>(b =>
            {
                b.ToTable("sections");
                b.HasKey(x => x.Id).HasName("sections_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("name").IsRequired();
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.Price).HasColumnName("price");
                b.Property(x => x.NumberOfVideos).HasColumnName("number_of_videos");
                b.Property(x => x.NumberOfExams).HasColumnName("number_of_exams");
                b.Property(x => x.Rating).HasColumnName("rating");
                b.Property(x => x.CourseId).HasColumnName("course_id");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Course).WithMany(x => x.Sections).HasForeignKey(x => x.CourseId).HasConstraintName("sections_course_id_fkey");
            });

            modelBuilder.Entity<Video>(b =>
            {
                b.ToTable("videos");
                b.HasKey(x => x.Id).HasName("videos_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("name").IsRequired();
                b.Property(x => x.VideoUrl).HasColumnName("video_url").IsRequired();
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.SectionId).HasColumnName("section_id");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Section).WithMany(x => x.Videos).HasForeignKey(x => x.SectionId).HasConstraintName("videos_section_id_fkey");
            });

            modelBuilder.Entity<Sheet>(b =>
            {
                b.ToTable("sheets");
                b.HasKey(x => x.Id).HasName("sheets_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("name").IsRequired();
                b.Property(x => x.SheetUrl).HasColumnName("sheet_url");
                b.Property(x => x.SectionId).HasColumnName("section_id");
                b.Property(x => x.VideoId).HasColumnName("video_id");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Section).WithMany(x => x.Sheets).HasForeignKey(x => x.SectionId).HasConstraintName("sheets_section_id_fkey");
                b.HasOne(x => x.Video).WithMany().HasForeignKey(x => x.VideoId).HasConstraintName("sheets_video_id_fkey");

            });

            modelBuilder.Entity<Exam>(b =>
            {
                b.ToTable("exams");
                b.HasKey(x => x.Id).HasName("exams_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("name").IsRequired();
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.StartTime).HasColumnName("start_time");
                b.Property(x => x.EndTime).HasColumnName("end_time");
                b.Property(x => x.DurationInMinutes).HasColumnName("duration");
                b.Property(x => x.PassMarkPercentage).HasColumnName("pass_mark_percentage");
                b.Property(x => x.ExamType).HasColumnName("exam_type").HasConversion<EnumToStringConverter<ExamType>>();
                b.Property(x => x.TotalMark).HasColumnName("total_mark");
                b.Property(x => x.CourseId).HasColumnName("course_id");
                b.Property(x => x.SectionId).HasColumnName("section_id");
                b.Property(x => x.NumberOfQuestions).HasColumnName("number_of_questions");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.Property(x => x.IsRandomized).HasColumnName("is_randomized");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasMany(x => x.ExamResults).WithOne(r => r.Exam).HasForeignKey(r => r.ExamId);
                b.HasMany(x => x.ExamQuestions).WithOne(r => r.Exam).HasForeignKey(r => r.ExamId);
                b.HasMany(x => x.InstructorExams).WithOne(r => r.Exam).HasForeignKey(r => r.ExamId);
                b.HasMany(x => x.StudentExams).WithOne(r => r.Exam).HasForeignKey(r => r.ExamId);
                b.HasOne(x => x.Course).WithMany(x => x.Exams).HasForeignKey(x => x.CourseId).HasConstraintName("exams_course_id_fkey");
                b.HasOne(x => x.Section).WithMany(x => x.Exams).HasForeignKey(x => x.SectionId).HasConstraintName("exams_section_id_fkey");

            });

            modelBuilder.Entity<Question>(b =>
            {
                b.ToTable("questions");
                b.HasKey(x => x.Id).HasName("questions_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.QuestionString).HasColumnName("question_string").IsRequired();
                b.Property(x => x.SectionId).HasColumnName("section_id").IsRequired();
                b.Property(x => x.CourseId).HasColumnName("course_id").IsRequired();
                b.Property(x => x.QuestionImageUrl).HasColumnName("question_image_url");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<ExamBank>(b =>
            {
                b.ToTable("exam_bank", "public");
                b.HasKey(x => new { x.QuestionId, x.ExamId }).HasName("exam_bank_pkey");
                b.Property(eq => eq.ExamId)
                       .HasColumnName("exam_id")
                       .IsRequired();

                b.Property(eq => eq.QuestionId)
                       .HasColumnName("question_id")
                       .IsRequired();

                b.Property(x => x.QuestionMark).HasColumnName("question_mark");
            });

            modelBuilder.Entity<Answer>(b =>
            {
                b.ToTable("answers");
                b.HasKey(x => x.Id).HasName("answers_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.QuestionId).HasColumnName("question_id");
                b.Property(x => x.AnswerText).HasColumnName("answer_text").IsRequired();
                b.Property(x => x.IsCorrect).HasColumnName("is_correct");
                b.Property(x => x.Explanation).HasColumnName("explanation");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Question).WithMany(x => x.Answers).HasForeignKey(x => x.QuestionId).HasConstraintName("answers_question_id_fkey");
            });

            modelBuilder.Entity<StudentCourse>(b =>
            {
                b.ToTable("student_courses");
                b.HasKey(x => new { x.StudentId, x.CourseId }).HasName("student_courses_pkey");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.CourseId).HasColumnName("course_id");
                b.Property(x => x.EnrolledAt).HasColumnName("enrolled_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.Property(x => x.NumberOfCourseVideosWatched).HasColumnName("number_of_course_videos_watched");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.StudentCourses).HasForeignKey(x => x.StudentId).HasConstraintName("student_courses_student_id_fkey");
                b.HasOne(x => x.Course).WithMany(x => x.StudentCourses).HasForeignKey(x => x.CourseId).HasConstraintName("student_courses_course_id_fkey");
            });

            modelBuilder.Entity<StudentSection>(b =>
            {
                b.ToTable("student_sections");
                b.HasKey(x => new { x.StudentId, x.SectionId }).HasName("student_sections_pkey");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.SectionId).HasColumnName("section_id");
                b.Property(x => x.EnrolledAt).HasColumnName("enrolled_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.Property(x => x.NumberOfSectionVideosWatched).HasColumnName("number_of_section_videos_watched");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.StudentSections).HasForeignKey(x => x.StudentId).HasConstraintName("student_sections_student_id_fkey");
                b.HasOne(x => x.Section).WithMany(x => x.StudentSections).HasForeignKey(x => x.SectionId).HasConstraintName("student_sections_section_id_fkey");
            });

            modelBuilder.Entity<StudentVideo>(b =>
            {
                b.ToTable("student_videos");
                b.HasKey(x => new { x.StudentId, x.VideoId }).HasName("student_videos_pkey");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.VideoId).HasColumnName("video_id");
                b.Property(x => x.WatchedAt).HasColumnName("watched_at");
                b.Property(x => x.IsWatched).HasColumnName("is_watched");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.StudentVideos).HasForeignKey(x => x.StudentId).HasConstraintName("student_videos_student_id_fkey");
                b.HasOne(x => x.Video).WithMany(x => x.StudentVideos).HasForeignKey(x => x.VideoId).HasConstraintName("student_videos_video_id_fkey");
            });

            modelBuilder.Entity<StudentSheet>(b =>
            {
                b.ToTable("student_sheets");
                b.HasKey(x => new { x.StudentId, x.SheetId }).HasName("student_sheets_pkey");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.SheetId).HasColumnName("sheet_id");
                b.Property(x => x.ViewedAt).HasColumnName("viewed_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.StudentSheets).HasForeignKey(x => x.StudentId).HasConstraintName("student_sheets_student_id_fkey");
                b.HasOne(x => x.Sheet).WithMany(x => x.StudentSheets).HasForeignKey(x => x.SheetId).HasConstraintName("student_sheets_sheet_id_fkey");
            });

            modelBuilder.Entity<StudentExam>(b =>
            {
                b.ToTable("student_exams");
                b.HasKey(x => new { x.StudentId, x.ExamId }).HasName("student_exams_pkey");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.ExamId).HasColumnName("exam_id");
                b.Property(x => x.TakenAt).HasColumnName("taken_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.StudentExams).HasForeignKey(x => x.StudentId).HasConstraintName("student_exams_student_id_fkey");
                b.HasOne(x => x.Exam).WithMany(x => x.StudentExams).HasForeignKey(x => x.ExamId).HasConstraintName("student_exams_exam_id_fkey");
            });

            modelBuilder.Entity<InstructorCourse>(b =>
            {
                b.ToTable("instructor_courses");
                b.HasKey(x => new { x.InstructorId, x.CourseId }).HasName("instructor_courses_pkey");
                b.Property(x => x.InstructorId).HasColumnName("instructor_id");
                b.Property(x => x.CourseId).HasColumnName("course_id");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Instructor).WithMany(x => x.InstructorCourses).HasForeignKey(x => x.InstructorId).HasConstraintName("instructor_courses_instructor_id_fkey");
                b.HasOne(x => x.Course).WithMany(x => x.InstructorCourses).HasForeignKey(x => x.CourseId).HasConstraintName("instructor_courses_course_id_fkey");
            });

            modelBuilder.Entity<InstructorSection>(b =>
            {
                b.ToTable("instructor_sections");
                b.HasKey(x => new { x.InstructorId, x.SectionId }).HasName("instructor_sections_pkey");
                b.Property(x => x.InstructorId).HasColumnName("instructor_id");
                b.Property(x => x.SectionId).HasColumnName("section_id");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Instructor).WithMany(x => x.InstructorSections).HasForeignKey(x => x.InstructorId).HasConstraintName("instructor_sections_instructor_id_fkey");
                b.HasOne(x => x.Section).WithMany(x => x.InstructorSections).HasForeignKey(x => x.SectionId).HasConstraintName("instructor_sections_section_id_fkey");
            });

            modelBuilder.Entity<InstructorExam>(b =>
            {
                b.ToTable("instructor_exams");
                b.HasKey(x => new { x.InstructorId, x.ExamId }).HasName("instructor_exams_pkey");
                b.Property(x => x.InstructorId).HasColumnName("instructor_id");
                b.Property(x => x.ExamId).HasColumnName("exam_id");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Instructor).WithMany(x => x.InstructorExams).HasForeignKey(x => x.InstructorId).HasConstraintName("instructor_exams_instructor_id_fkey");
                b.HasOne(x => x.Exam).WithMany(x => x.InstructorExams).HasForeignKey(x => x.ExamId).HasConstraintName("instructor_exams_exam_id_fkey");
            });

            modelBuilder.Entity<VideoSheet>(b =>
            {
                b.ToTable("video_sheets");
                b.HasKey(x => new { x.VideoId, x.SheetId }).HasName("video_sheets_pkey");
                b.Property(x => x.VideoId).HasColumnName("video_id");
                b.Property(x => x.SheetId).HasColumnName("sheet_id");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Video).WithMany(x => x.VideoSheets).HasForeignKey(x => x.VideoId).HasConstraintName("video_sheets_video_id_fkey");
                b.HasOne(x => x.Sheet).WithMany(x => x.VideoSheets).HasForeignKey(x => x.SheetId).HasConstraintName("video_sheets_sheet_id_fkey");
            });

            modelBuilder.Entity<CourseReview>(b =>
            {
                b.ToTable("course_reviews");
                b.HasKey(x => x.Id).HasName("course_reviews_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.EntityId).HasColumnName("course_id");
                b.Property(x => x.StarRating).HasColumnName("star_rating");
                b.Property(x => x.Comment).HasColumnName("comment");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.CourseReviews).HasForeignKey(x => x.StudentId).HasConstraintName("course_reviews_student_id_fkey");
                b.HasOne(x => x.Course).WithMany(x => x.CourseReviews).HasForeignKey(x => x.EntityId).HasConstraintName("course_reviews_course_id_fkey");
            });

            modelBuilder.Entity<SectionReview>(b =>
            {
                b.ToTable("section_reviews");
                b.HasKey(x => x.Id).HasName("section_reviews_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.EntityId).HasColumnName("section_id");
                b.Property(x => x.StarRating).HasColumnName("star_rating");
                b.Property(x => x.Comment).HasColumnName("comment");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.SectionReviews).HasForeignKey(x => x.StudentId).HasConstraintName("section_reviews_student_id_fkey");
                b.HasOne(x => x.Section).WithMany(x => x.SectionReviews).HasForeignKey(x => x.EntityId).HasConstraintName("section_reviews_section_id_fkey");
            });

            modelBuilder.Entity<VideoReview>(b =>
            {
                b.ToTable("video_reviews");
                b.HasKey(x => x.Id).HasName("video_reviews_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.EntityId).HasColumnName("video_id");
                b.Property(x => x.StarRating).HasColumnName("star_rating");
                b.Property(x => x.Comment).HasColumnName("comment");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.VideoReviews).HasForeignKey(x => x.StudentId).HasConstraintName("video_reviews_student_id_fkey");
                b.HasOne(x => x.Video).WithMany(x => x.VideoReviews).HasForeignKey(x => x.EntityId).HasConstraintName("video_reviews_video_id_fkey");
            });

            modelBuilder.Entity<InstructorReview>(b =>
            {
                b.ToTable("instructor_reviews");
                b.HasKey(x => x.Id).HasName("instructor_reviews_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.EntityId).HasColumnName("instructor_id");
                b.Property(x => x.StarRating).HasColumnName("star_rating");
                b.Property(x => x.Comment).HasColumnName("comment");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.InstructorReviews).HasForeignKey(x => x.StudentId).HasConstraintName("instructor_reviews_student_id_fkey");
                b.HasOne(x => x.Instructor).WithMany(x => x.InstructorReviews).HasForeignKey(x => x.EntityId).HasConstraintName("instructor_reviews_instructor_id_fkey");
            });

            modelBuilder.Entity<ChatRoom>(b =>
            {
                b.ToTable("chat_rooms");
                b.HasKey(x => x.Id).HasName("chat_rooms_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.User1Id).HasColumnName("user1_id");
                b.Property(x => x.User2Id).HasColumnName("user2_id");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.User1).WithMany().HasForeignKey(x => x.User1Id).HasConstraintName("chat_rooms_user1_id_fkey");
                b.HasOne(x => x.User2).WithMany().HasForeignKey(x => x.User2Id).HasConstraintName("chat_rooms_user2_id_fkey");
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.ToTable("chat_messages");
                b.HasKey(x => x.Id).HasName("chat_messages_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ChatRoomId).HasColumnName("chat_room_id");
                b.Property(x => x.SenderId).HasColumnName("sender_id");
                b.Property(x => x.MessageText).HasColumnName("message_text").IsRequired();
                b.Property(x => x.Comment).HasColumnName("comment");
                b.Property(x => x.StarRating).HasColumnName("star_rating");
                b.Property(x => x.SentAt).HasColumnName("sent_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.ChatRoom).WithMany(x => x.Messages).HasForeignKey(x => x.ChatRoomId).HasConstraintName("chat_messages_chat_room_id_fkey");
                b.HasOne(x => x.Sender).WithMany().HasForeignKey(x => x.SenderId).HasConstraintName("chat_messages_sender_id_fkey");
            });

            modelBuilder.Entity<Payment>(b =>
            {
                b.ToTable("payments");
                b.HasKey(x => x.Id).HasName("payments_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.Status).HasColumnName("status");
                b.Property(x => x.CourseId).HasColumnName("course_id");
                b.Property(x => x.SectionId).HasColumnName("section_id");
                b.Property(x => x.SenderAccount).HasColumnName("sender_account");
                b.Property(x => x.ReceiverAccount).HasColumnName("receiver_account");
                b.Property(x => x.CommissionAccount1).HasColumnName("commission_account1");
                b.Property(x => x.CommissionAccount2).HasColumnName("commission_account2");
                b.Property(x => x.TimeOfPayment).HasColumnName("time_of_payment");
                b.Property(x => x.Amount).HasColumnName("amount");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.Payments).HasForeignKey(x => x.StudentId).HasConstraintName("payments_student_id_fkey");
                b.HasOne(x => x.Course).WithMany(x => x.Payments).HasForeignKey(x => x.CourseId).HasConstraintName("payments_course_id_fkey");
                b.HasOne(x => x.Section).WithMany(x => x.Payments).HasForeignKey(x => x.SectionId).HasConstraintName("payments_section_id_fkey");
            });

            modelBuilder.Entity<StudentExamResult>(b =>
            {
                b.ToTable("student_exam_results");
                b.HasKey(x => x.Id).HasName("exam_results_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.ExamId).HasColumnName("exam_id");
                b.Property(x => x.StudentMark).HasColumnName("obtained_marks");
                b.Property(x => x.Status).HasColumnName("status").HasConversion<EnumToStringConverter<ExamStatus>>();
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.Student).WithMany(x => x.ExamResults).HasForeignKey(x => x.StudentId).HasConstraintName("exam_results_student_id_fkey");
                b.HasOne(x => x.Exam).WithMany(x => x.ExamResults).HasForeignKey(x => x.ExamId).HasConstraintName("exam_results_exam_id_fkey");
            });

            modelBuilder.Entity<StudentSubmission>(b =>
            {
                b.ToTable("student_submissions");
                b.HasKey(x => x.Id).HasName("student_submissions_pkey");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.QuestionId).HasColumnName("question_id");
                b.Property(x => x.StudentId).HasColumnName("student_id");
                b.Property(x => x.ExamResultId).HasColumnName("exam_result_id");
                b.Property(x => x.ChosenAnswerId).HasColumnName("chosen_answer_id");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.Property(x => x.IsDeleted).HasColumnName("is_deleted");
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasOne(x => x.ExamResult).WithMany(x => x.StudentSubmissions).HasForeignKey(x => x.ExamResultId).HasConstraintName("student_submissions_exam_result_id_fkey");
                b.HasOne(x => x.Question).WithMany(x => x.StudentSubmissions).HasForeignKey(x => x.QuestionId).HasConstraintName("student_submissions_question_id_fkey");
                b.HasOne(x => x.ChosenAnswer).WithMany().HasForeignKey(x => x.ChosenAnswerId).HasConstraintName("student_submissions_chosen_answer_id_fkey");
                //b.HasOne<Student>().WithMany().HasForeignKey(ss => ss.StudentId);
            });

            modelBuilder.Entity<RefreshToken>(t =>
            {
                t.ToTable("refresh_tokens");
                t.HasKey(x => x.Id).HasName("refresh_tokens_pkey");
                t.Property(x => x.Id).HasColumnName("id");
                t.Property(x => x.Token).HasColumnName("token").IsRequired();
                t.Property(x => x.UserId).HasColumnName("user_id");
                t.Property(x => x.ExpiresAt).HasColumnName("expires_at");
                //t.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId).HasConstraintName("refresh_tokens_user_id_fkey");
            });
        }
    }
}

