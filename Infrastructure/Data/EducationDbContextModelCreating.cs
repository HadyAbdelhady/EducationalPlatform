using Domain.Entities;
using Domain.enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data
{
    public static class EducationDbContextModelCreating
    {
        public static void Apply(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            // PostgreSQL Enum configurations
            modelBuilder.HasPostgresEnum(schema: "public", name: "exam_status", ["Submitted", "Graded", "Pending"]);
            modelBuilder.HasPostgresEnum(schema: "public", name: "payment_status", ["Pending", "Completed", "Failed"]);
            modelBuilder.HasPostgresEnum("public", "UsageCategory", ["ProfilePicture", "Thumbnail"]);

            // Query Filters (cannot be done with Data Annotations)
            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(x => x.Ssn).IsUnique();
                b.HasQueryFilter(x => !x.IsDeleted);
            });
            modelBuilder.Entity<EducationYear>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Course>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Section>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Video>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Sheet>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<AnswersSheet>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Question>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Answer>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<StudentCourse>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<StudentSection>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<StudentVideo>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<StudentExam>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<InstructorCourse>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<InstructorSection>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<InstructorExam>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<CourseReview>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<SectionReview>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<VideoReview>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<InstructorReview>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<ChatRoom>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<ChatMessage>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Payment>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<StudentExamResult>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<StudentSubmission>().HasQueryFilter(x => !x.IsDeleted);

            // Enum conversions (cannot be done with Data Annotations)
            modelBuilder.Entity<Sheet>()
                .Property(x => x.Type)
                .HasConversion<EnumToStringConverter<SheetType>>();

            modelBuilder.Entity<Exam>()
                .Property(x => x.ExamType)
                .HasConversion<EnumToStringConverter<ExamType>>();

            modelBuilder.Entity<StudentExamResult>()
                .Property(x => x.Status)
                .HasConversion<EnumToStringConverter<ExamStatus>>();

            // Complex relationship configurations
            modelBuilder.Entity<Student>(b =>
            {
                b.HasOne(x => x.EducationYear)
                    .WithMany(e => e.Students)
                    .HasForeignKey(s => s.EducationYearId)
                    .IsRequired();
                
                b.HasOne(x => x.User)
                    .WithOne(x => x.Student)
                    .HasForeignKey<Student>(x => x.UserId)
                    .HasConstraintName("students_user_id_fkey");
            });

            modelBuilder.Entity<Instructor>(b =>
            {
                b.HasOne(x => x.User)
                    .WithOne(x => x.Instructor)
                    .HasForeignKey<Instructor>(x => x.UserId)
                    .HasConstraintName("instructors_user_id_fkey");
            });

            // Cascade delete behavior
            modelBuilder.Entity<Sheet>(b =>
            {
                b.HasMany(x => x.AnswersSheets)
                    .WithOne(r => r.QuestionsSheet)
                    .HasForeignKey(r => r.QuestionsSheetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Foreign key constraint names (optional, but kept for consistency with existing database)
            modelBuilder.Entity<Section>(b =>
            {
                b.HasOne(x => x.Course)
                    .WithMany(x => x.Sections)
                    .HasForeignKey(x => x.CourseId)
                    .HasConstraintName("sections_course_id_fkey");
            });

            modelBuilder.Entity<Video>(b =>
            {
                b.HasOne(x => x.Section)
                    .WithMany(x => x.Videos)
                    .HasForeignKey(x => x.SectionId)
                    .HasConstraintName("videos_section_id_fkey");
            });

            modelBuilder.Entity<Sheet>(b =>
            {
                b.HasOne(x => x.Section)
                    .WithMany(x => x.Sheets)
                    .HasForeignKey(x => x.SectionId)
                    .HasConstraintName("sheets_section_id_fkey");
                
                b.HasOne(x => x.Video)
                    .WithMany(x => x.Sheets)
                    .HasForeignKey(x => x.VideoId)
                    .HasConstraintName("sheets_video_id_fkey");
                
                b.HasOne(x => x.Course)
                    .WithMany(x => x.Sheets)
                    .HasForeignKey(x => x.CourseId)
                    .HasConstraintName("sheets_course_id_fkey");
                
                b.HasOne(x => x.Instructor)
                    .WithMany(x => x.Sheets)
                    .HasForeignKey(x => x.InstructorId)
                    .HasConstraintName("sheets_instructor_id_fkey");
            });

            modelBuilder.Entity<AnswersSheet>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.AnswersSheets)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("answers_sheets_student_id_fkey");
                
                b.HasOne(x => x.QuestionsSheet)
                    .WithMany(x => x.AnswersSheets)
                    .HasForeignKey(x => x.QuestionsSheetId)
                    .HasConstraintName("answers_sheets_questions_sheet_id_fkey");
            });

            modelBuilder.Entity<Exam>(b =>
            {
                b.HasOne(x => x.Course)
                    .WithMany(x => x.Exams)
                    .HasForeignKey(x => x.CourseId)
                    .HasConstraintName("exams_course_id_fkey");
                
                b.HasOne(x => x.Section)
                    .WithMany(x => x.Exams)
                    .HasForeignKey(x => x.SectionId)
                    .HasConstraintName("exams_section_id_fkey");
            });

            modelBuilder.Entity<Answer>(b =>
            {
                b.HasOne(x => x.Question)
                    .WithMany(x => x.Answers)
                    .HasForeignKey(x => x.QuestionId)
                    .HasConstraintName("answers_question_id_fkey");
            });

            modelBuilder.Entity<StudentCourse>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.StudentCourses)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("student_courses_student_id_fkey");
                
                b.HasOne(x => x.Course)
                    .WithMany(x => x.StudentCourses)
                    .HasForeignKey(x => x.CourseId)
                    .HasConstraintName("student_courses_course_id_fkey");
            });

            modelBuilder.Entity<StudentSection>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.StudentSections)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("student_sections_student_id_fkey");
                
                b.HasOne(x => x.Section)
                    .WithMany(x => x.StudentSections)
                    .HasForeignKey(x => x.SectionId)
                    .HasConstraintName("student_sections_section_id_fkey");
            });

            modelBuilder.Entity<StudentVideo>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.StudentVideos)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("student_videos_student_id_fkey");
                
                b.HasOne(x => x.Video)
                    .WithMany(x => x.StudentVideos)
                    .HasForeignKey(x => x.VideoId)
                    .HasConstraintName("student_videos_video_id_fkey");
            });

            modelBuilder.Entity<StudentExam>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.StudentExams)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("student_exams_student_id_fkey");
                
                b.HasOne(x => x.Exam)
                    .WithMany(x => x.StudentExams)
                    .HasForeignKey(x => x.ExamId)
                    .HasConstraintName("student_exams_exam_id_fkey");
            });

            modelBuilder.Entity<InstructorCourse>(b =>
            {
                b.HasOne(x => x.Instructor)
                    .WithMany(x => x.InstructorCourses)
                    .HasForeignKey(x => x.InstructorId)
                    .HasConstraintName("instructor_courses_instructor_id_fkey");
                
                b.HasOne(x => x.Course)
                    .WithMany(x => x.InstructorCourses)
                    .HasForeignKey(x => x.CourseId)
                    .HasConstraintName("instructor_courses_course_id_fkey");
            });

            modelBuilder.Entity<InstructorSection>(b =>
            {
                b.HasOne(x => x.Instructor)
                    .WithMany(x => x.InstructorSections)
                    .HasForeignKey(x => x.InstructorId)
                    .HasConstraintName("instructor_sections_instructor_id_fkey");
                
                b.HasOne(x => x.Section)
                    .WithMany(x => x.InstructorSections)
                    .HasForeignKey(x => x.SectionId)
                    .HasConstraintName("instructor_sections_section_id_fkey");
            });

            modelBuilder.Entity<InstructorExam>(b =>
            {
                b.HasOne(x => x.Instructor)
                    .WithMany(x => x.InstructorExams)
                    .HasForeignKey(x => x.InstructorId)
                    .HasConstraintName("instructor_exams_instructor_id_fkey");
                
                b.HasOne(x => x.Exam)
                    .WithMany(x => x.InstructorExams)
                    .HasForeignKey(x => x.ExamId)
                    .HasConstraintName("instructor_exams_exam_id_fkey");
            });

            modelBuilder.Entity<CourseReview>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.CourseReviews)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("course_reviews_student_id_fkey");
                
                b.HasOne(x => x.Course)
                    .WithMany(x => x.CourseReviews)
                    .HasForeignKey(x => x.EntityId)
                    .HasConstraintName("course_reviews_course_id_fkey");
            });

            modelBuilder.Entity<SectionReview>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.SectionReviews)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("section_reviews_student_id_fkey");
                
                b.HasOne(x => x.Section)
                    .WithMany(x => x.SectionReviews)
                    .HasForeignKey(x => x.EntityId)
                    .HasConstraintName("section_reviews_section_id_fkey");
            });

            modelBuilder.Entity<VideoReview>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.VideoReviews)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("video_reviews_student_id_fkey");
                
                b.HasOne(x => x.Video)
                    .WithMany(x => x.VideoReviews)
                    .HasForeignKey(x => x.EntityId)
                    .HasConstraintName("video_reviews_video_id_fkey");
            });

            modelBuilder.Entity<InstructorReview>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.InstructorReviews)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("instructor_reviews_student_id_fkey");
                
                b.HasOne(x => x.Instructor)
                    .WithMany(x => x.InstructorReviews)
                    .HasForeignKey(x => x.EntityId)
                    .HasConstraintName("instructor_reviews_instructor_id_fkey");
            });

            modelBuilder.Entity<ChatRoom>(b =>
            {
                b.HasOne(x => x.User1)
                    .WithMany()
                    .HasForeignKey(x => x.User1Id)
                    .HasConstraintName("chat_rooms_user1_id_fkey");
                
                b.HasOne(x => x.User2)
                    .WithMany()
                    .HasForeignKey(x => x.User2Id)
                    .HasConstraintName("chat_rooms_user2_id_fkey");
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasOne(x => x.ChatRoom)
                    .WithMany(x => x.Messages)
                    .HasForeignKey(x => x.ChatRoomId)
                    .HasConstraintName("chat_messages_chat_room_id_fkey");
                
                b.HasOne(x => x.Sender)
                    .WithMany()
                    .HasForeignKey(x => x.SenderId)
                    .HasConstraintName("chat_messages_sender_id_fkey");
            });

            modelBuilder.Entity<Payment>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.Payments)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("payments_student_id_fkey");
                
                b.HasOne(x => x.Course)
                    .WithMany(x => x.Payments)
                    .HasForeignKey(x => x.CourseId)
                    .HasConstraintName("payments_course_id_fkey");
                
                b.HasOne(x => x.Section)
                    .WithMany(x => x.Payments)
                    .HasForeignKey(x => x.SectionId)
                    .HasConstraintName("payments_section_id_fkey");
            });

            modelBuilder.Entity<StudentExamResult>(b =>
            {
                b.HasOne(x => x.Student)
                    .WithMany(x => x.ExamResults)
                    .HasForeignKey(x => x.StudentId)
                    .HasConstraintName("exam_results_student_id_fkey");
                
                b.HasOne(x => x.Exam)
                    .WithMany(x => x.ExamResults)
                    .HasForeignKey(x => x.ExamId)
                    .HasConstraintName("exam_results_exam_id_fkey");
            });

            modelBuilder.Entity<StudentSubmission>(b =>
            {
                b.HasOne(x => x.ExamResult)
                    .WithMany(x => x.StudentSubmissions)
                    .HasForeignKey(x => x.ExamResultId)
                    .HasConstraintName("student_submissions_exam_result_id_fkey");
                
                b.HasOne(x => x.Question)
                    .WithMany(x => x.StudentSubmissions)
                    .HasForeignKey(x => x.QuestionId)
                    .HasConstraintName("student_submissions_question_id_fkey");
                
                b.HasOne(x => x.ChosenAnswer)
                    .WithMany()
                    .HasForeignKey(x => x.ChosenAnswerId)
                    .HasConstraintName("student_submissions_chosen_answer_id_fkey");
            });
        }
    }
}
