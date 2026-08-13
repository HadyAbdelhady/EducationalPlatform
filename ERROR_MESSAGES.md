# Educational Platform — Error Messages Catalog

> Catalog of error messages handled across the backend (Application, Infrastructure, Edu_Base).
> Generated: 2026-07-28
> Total occurrences: 358 | Unique static messages: 181 | Dynamic templates: 73

## Error Types (`Domain/enums/ErrorType.cs`)

| Type | HTTP Status |
|------|-------------|
| NotFound | 404 |
| BadRequest | 400 |
| UnAuthorized | 401 |
| Forbidden | 403 |
| Validation | 422 |
| Conflict | 409 |
| InternalServerError | 500 |

## Result.FailureStatusCode (77 unique)

- **A non-empty PDF file is required.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:34, Application/Features/AnswersSheet/Commands/UpdateAnswersSheet/UpdateAnswersSheetCommandHandler.cs:27
- **An education year with this name already exists.**
  - Sources: Application/Features/EducationYears/Commands/CreateEducationYear/CreateEducationYearCommandHandler.cs:24
- **Another education year with this name already exists.**
  - Sources: Application/Features/EducationYears/Commands/UpdateEducationYear/UpdateEducationYearCommandHandler.cs:36
- **Answers sheet not found**
  - Sources: Application/Features/AnswersSheet/Commands/ApproveAnswersSheet/ApproveAnswersSheetCommandHandler.cs:25, Application/Features/AnswersSheet/Commands/DeleteAnswersSheet/DeleteAnswersSheetCommandHandler.cs:25, Application/Features/AnswersSheet/Commands/UpdateAnswersSheet/UpdateAnswersSheetCommandHandler.cs:41
- **Cannot delete an approved submission.**
  - Sources: Application/Features/AnswersSheet/Commands/DeleteAnswersSheet/DeleteAnswersSheetCommandHandler.cs:37
- **Cannot delete education year. It has associated students or courses.**
  - Sources: Application/Features/EducationYears/Commands/DeleteEducationYear/DeleteEducationYearCommandHandler.cs:36
- **Cannot delete one or more sections because there are students enrolled.**
  - Sources: Application/Features/Sections/Commands/DeleteSection/BulkDeleteSectionCommandHandler.cs:40
- **Cannot delete this course because there are students enrolled.**
  - Sources: Application/Features/Courses/Commands/DeleteCourse/DeleteCourseCommandHandler.cs:30
- **Cannot delete this section because there are students enrolled.**
  - Sources: Application/Features/Sections/Commands/DeleteSection/DeleteSectionCommandHandler.cs:28
- **Cannot enroll in a course from a different education year.**
  - Sources: Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:83
- **Cannot enroll in a section from a different education year.**
  - Sources: Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:137
- **Cannot modify an approved submission.**
  - Sources: Application/Features/AnswersSheet/Commands/UpdateAnswersSheet/UpdateAnswersSheetCommandHandler.cs:53
- **Cannot update an exam that has already started.**
  - Sources: Application/Features/Exams/Command/UpdateExam/UpdateExamCommandHandler.cs:29
- **Center not found.**
  - Sources: Application/Features/Centers/Commands/AssignInstructorToCenter/AssignInstructorToCenterCommandHandler.cs:28, Application/Features/Centers/Commands/DeleteCenter/DeleteCenterCommandHandler.cs:22, Application/Features/Centers/Commands/UpdateCenter/UpdateCenterCommandHandler.cs:23, Application/Features/Centers/Queries/GetCenterById/GetCenterByIdQueryHandler.cs:21
- **Course not found**
  - Sources: Application/Features/Courses/Commands/UpdateCourse/UpdateCourseCommandHandler.cs:65, Application/Features/Sections/Commands/DeleteSection/BulkDeleteSectionCommandHandler.cs:25, Application/Features/Sheets/Commands/CreateSheet/CreateSheetCommandHandler.cs:57
- **Course not found.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:154, Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:78, Application/Features/Sections/Commands/CreateSection/BulkCreateSectionCommandHandler.cs:28, Infrastructure/Features/Reviews/ReviewService/ReviewServiceFactory.cs:102
- **Due date is required for question sheets.**
  - Sources: Application/Features/Sheets/Commands/CreateSheet/CreateSheetCommandHandler.cs:64
- **Education Year not found.**
  - Sources: Application/Features/Courses/Commands/CreateCourse/CreateCourseCommandHandler.cs:29, Application/Features/EducationYears/Commands/DeleteEducationYear/DeleteEducationYearCommandHandler.cs:21, Application/Features/EducationYears/Commands/UpdateEducationYear/UpdateEducationYearCommandHandler.cs:23, Application/Features/EducationYears/Queries/GetEducationYearById/GetEducationYearByIdQueryHandler.cs:22
- **End time must be in the future.**
  - Sources: Application/Features/Exams/Command/GenerateExam/GenerateExamCommandHandler.cs:52
- **Exam already started or completed by the student**
  - Sources: Application/Features/Exams/Command/StartExam/StartExamCommandHandler.cs:93
- **Exam has already been submitted**
  - Sources: Application/Features/Exams/Command/SubmitExam/SubmitExamCommandHandler.cs:49
- **Exam has ended**
  - Sources: Application/Features/Exams/Command/StartExam/StartExamCommandHandler.cs:57
- **Exam has not been started yet**
  - Sources: Application/Features/Exams/Command/SubmitExam/SubmitExamCommandHandler.cs:43
- **Exam has not started yet**
  - Sources: Application/Features/Exams/Command/StartExam/StartExamCommandHandler.cs:50
- **Exam not found**
  - Sources: Application/Features/Exams/Command/StartExam/StartExamCommandHandler.cs:41, Application/Features/Exams/Command/SubmitExam/SubmitExamCommandHandler.cs:34, Application/Features/Exams/Query/GetExamSubmissionsList/GetExamSubmissionsListQueryHandler.cs:34, Application/Features/Exams/Query/GetStudentExamResult/GetStudentExamResultQueryHandler.cs:27
- **Exam not found.**
  - Sources: Application/Features/Exams/Command/DeleteExam/DeleteExamCommandHandler.cs:29, Application/Features/Exams/Query/GetExamById/GetExamByIdQueryHandler.cs:20
- **Exam result not found for this student**
  - Sources: Application/Features/Exams/Query/GetStudentExamResult/GetStudentExamResultQueryHandler.cs:42
- **Failed to delete the video. No changes were persisted to the database.**
  - Sources: Application/Features/Videos/Commands/DeleteVideo/DeleteVideoCommandHandler.cs:26
- **Failed to initiate payment with the provider.**
  - Sources: Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:114, Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:169
- **Failed to save the video. No changes were persisted to the database.**
  - Sources: Application/Features/Videos/Commands/CreateVideo/CreateVideoCommandHandler.cs:51
- **Failed to save videos. No changes were persisted to the database.**
  - Sources: Application/Features/Videos/Commands/CreateVideo/BulkCreateVideosCommandHandler.cs:60
- **Instructor is already assigned to this center.**
  - Sources: Application/Features/Centers/Commands/AssignInstructorToCenter/AssignInstructorToCenterCommandHandler.cs:42
- **Instructor is not a member of this center.**
  - Sources: Application/Features/Centers/Commands/RemoveInstructorFromCenter/RemoveInstructorFromCenterCommandHandler.cs:25
- **Instructor not found**
  - Sources: Application/Features/Sheets/Commands/CreateSheet/CreateSheetCommandHandler.cs:28
- **Instructor not found.**
  - Sources: Application/Features/Centers/Commands/AssignInstructorToCenter/AssignInstructorToCenterCommandHandler.cs:35, Application/Features/Courses/Commands/CreateCourse/CreateCourseCommandHandler.cs:35, Infrastructure/Features/Reviews/ReviewService/ReviewServiceFactory.cs:68
- **Intention ID is missing**
  - Sources: Application/Features/Payment/PaymentWebhook/PaymentWebhookCommandHandler.cs:31
- **Invalid HMAC signature**
  - Sources: Application/Features/Payment/PaymentWebhook/PaymentWebhookCommandHandler.cs:25
- **Name is required.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:29, Application/Features/AnswersSheet/Commands/UpdateAnswersSheet/UpdateAnswersSheetCommandHandler.cs:22
- **No section IDs provided**
  - Sources: Application/Features/Sections/Commands/DeleteSection/BulkDeleteSectionCommandHandler.cs:19
- **Only PDF uploads are allowed.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:42, Application/Features/AnswersSheet/Commands/UpdateAnswersSheet/UpdateAnswersSheetCommandHandler.cs:35
- **Payment transaction not found**
  - Sources: Application/Features/Payment/PaymentWebhook/PaymentWebhookCommandHandler.cs:38
- **Question does not exist.**
  - Sources: Application/Features/Exams/Command/GenerateExam/GenerateExamCommandHandler.cs:30
- **Question not found.**
  - Sources: Application/Features/Answers/Command/AnswerQuestion/AddAnswerToQuestionCommandHandler.cs:22, Application/Features/Questions/Command/DeleteQuestion/DeleteQuestionCommandHandler.cs:23, Application/Features/Questions/Command/UpdateQuestion/UpdateQuestionCommandHandler.cs:28
- **Questions sheet is not associated with a course, section, or video.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:139
- **Questions sheet not found**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:48
- **Questions sheet not found.**
  - Sources: Application/Features/AnswersSheet/Commands/UpdateAnswersSheet/UpdateAnswersSheetCommandHandler.cs:60
- **Questions sheet section not found.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:102, Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:126
- **Questions sheet video not found.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:118
- **Review not found.**
  - Sources: Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:140
- **Section not found**
  - Sources: Application/Features/Sheets/Commands/CreateSheet/CreateSheetCommandHandler.cs:37, Application/Features/Videos/Commands/UpdateVideo/UpdateVideoCommandHandler.cs:25
- **Section not found.**
  - Sources: Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:132, Application/Features/Sections/Commands/UpdateSection/UpdateSectionCommandHandler.cs:22, Infrastructure/Features/Reviews/ReviewService/ReviewServiceFactory.cs:85
- **Sheet not found**
  - Sources: Application/Features/Sheets/Commands/DeleteSheet/DeleteSheetCommandHandler.cs:21, Application/Features/Sheets/Commands/UpdateSheet/UpdateSheetCommandHandler.cs:22
- **Start time must be in the future.**
  - Sources: Application/Features/Exams/Command/GenerateExam/GenerateExamCommandHandler.cs:46
- **Student has no education year assigned.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:160
- **Student is already enrolled.**
  - Sources: Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:94, Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:148
- **Student not found**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:67, Application/Features/Exams/Command/StartExam/StartExamCommandHandler.cs:22, Application/Features/Exams/Command/SubmitExam/SubmitExamCommandHandler.cs:29, Application/Features/Payment/PaymentWebhook/PaymentWebhookCommandHandler.cs:68, Infrastructure/Features/Sheets/SheetService/SheetServiceFactory.cs:146
- **Student not found or has no education year assigned.**
  - Sources: Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:46
- **Student not found.**
  - Sources: Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:134
- **Student user not found.**
  - Sources: Application/Features/Payment/CreatePaymentIntension/BuyingCommandHandler.cs:30
- **Submissions are only allowed for question sheets.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:53
- **The specified center does not exist.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandHandler.cs:36
- **The submission deadline has passed!**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:60, Application/Features/AnswersSheet/Commands/UpdateAnswersSheet/UpdateAnswersSheetCommandHandler.cs:65
- **The submission deadline has passed; this submission can no longer be deleted.**
  - Sources: Application/Features/AnswersSheet/Commands/DeleteAnswersSheet/DeleteAnswersSheetCommandHandler.cs:46
- **This sheet is not available for your education year.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:167
- **This SSN is already registered with another account.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandHandler.cs:57, Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandHandler.cs:50, Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandHandler.cs:49
- **User does not exist**
  - Sources: Application/Features/Auth/Queries/CheckUserExists/CheckUserExistsQueryHandler.cs:24
- **User is not a student**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:72, Application/Features/Exams/Command/StartExam/StartExamCommandHandler.cs:29
- **User not found**
  - Sources: Application/Features/Payment/PaymentWebhook/PaymentWebhookCommandHandler.cs:64
- **Video not found**
  - Sources: Application/Features/Sheets/Commands/CreateSheet/CreateSheetCommandHandler.cs:46, Application/Features/Videos/Commands/UpdateVideo/UpdateVideoCommandHandler.cs:47
- **Video not found.**
  - Sources: Application/Features/Videos/Queries/GetVideoById/GetVideoByIdQueryHandler.cs:24, Infrastructure/Features/Reviews/ReviewService/ReviewServiceFactory.cs:51
- **You are not enrolled in the content this sheet belongs to.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:146
- **You are not enrolled in the course or section that contains this video.**
  - Sources: Application/Features/Videos/Queries/GetVideoById/GetVideoByIdQueryHandler.cs:37
- **You can only delete your own submission.**
  - Sources: Application/Features/AnswersSheet/Commands/DeleteAnswersSheet/DeleteAnswersSheetCommandHandler.cs:30
- **You can only update your own submission.**
  - Sources: Application/Features/AnswersSheet/Commands/UpdateAnswersSheet/UpdateAnswersSheetCommandHandler.cs:46
- **You have already submitted a review.**
  - Sources: Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:32
- **You have already submitted for this sheet. Use the update endpoint to replace your submission.**
  - Sources: Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:80
- **You're unauthorized to approve this answers sheet**
  - Sources: Application/Features/AnswersSheet/Commands/ApproveAnswersSheet/ApproveAnswersSheetCommandHandler.cs:32

## FluentValidation (27 unique)

- **A question needs at least 2 options.**
  - Sources: Application/Features/Questions/Command/AddQuestion/AddQuestionValidator.cs:15
- **A valid email is required.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:22
- **At least one answer must be marked as correct.**
  - Sources: Application/Features/Questions/Command/AddQuestion/AddQuestionValidator.cs:20
- **CenterId is required.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:10
- **Date of birth is required.**
  - Sources: Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandValidator.cs:28
- **Device ID is required for student accounts.**
  - Sources: Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandValidator.cs:18
- **Education year ID is required.**
  - Sources: Application/Features/EducationYears/Commands/DeleteEducationYear/DeleteEducationYearCommandValidator.cs:10, Application/Features/EducationYears/Commands/UpdateEducationYear/UpdateEducationYearCommandValidator.cs:11, Application/Features/EducationYears/Queries/GetEducationYearById/GetEducationYearByIdQueryValidator.cs:10
- **Education year is required.**
  - Sources: Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandValidator.cs:40
- **Education year name can only contain letters, numbers, spaces, and hyphens.**
  - Sources: Application/Features/EducationYears/Commands/CreateEducationYear/CreateEducationYearCommandValidator.cs:13, Application/Features/EducationYears/Commands/UpdateEducationYear/UpdateEducationYearCommandValidator.cs:16
- **Education year name is required.**
  - Sources: Application/Features/EducationYears/Commands/CreateEducationYear/CreateEducationYearCommandValidator.cs:11, Application/Features/EducationYears/Commands/UpdateEducationYear/UpdateEducationYearCommandValidator.cs:14
- **Education year name must be between 2 and 100 characters.**
  - Sources: Application/Features/EducationYears/Commands/CreateEducationYear/CreateEducationYearCommandValidator.cs:12, Application/Features/EducationYears/Commands/UpdateEducationYear/UpdateEducationYearCommandValidator.cs:15
- **Education year/qualification is required.**
  - Sources: Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandValidator.cs:30
- **Email is required.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:21
- **Full Name is required.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:25
- **Gender is required.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:31, Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandValidator.cs:24, Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandValidator.cs:34
- **Gender must be Male, Female.**
  - Sources: Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandValidator.cs:26, Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandValidator.cs:36
- **Google ID token is required.**
  - Sources: Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandValidator.cs:14, Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandValidator.cs:14
- **Google user info is required.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:13
- **IdToken is required.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:18
- **Phone Number is required.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:28, Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandValidator.cs:18, Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandValidator.cs:22
- **Phone number must be in a valid format.**
  - Sources: Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandValidator.cs:20, Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandValidator.cs:24
- **Question text is required.**
  - Sources: Application/Features/Questions/Command/AddQuestion/AddQuestionValidator.cs:10
- **SSN is required.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:35
- **SSN must be exactly 14 characters long.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:36
- **SSN must contain only digits.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandValidator.cs:37
- **Student must be at least 10 years old.**
  - Sources: Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandValidator.cs:30
- **You must provide at least two answers.**
  - Sources: Application/Features/Questions/Command/AddQuestion/AddQuestionValidator.cs:14

## DataAnnotation (5 unique)

- **Education year name is required.**
  - Sources: Application/Features/EducationYears/DTOs/CreateEducationYearRequest.cs:7, Application/Features/EducationYears/DTOs/UpdateEducationYearRequest.cs:7
- **Education year name must be between 2 and 100 characters.**
  - Sources: Application/Features/EducationYears/DTOs/CreateEducationYearRequest.cs:8, Application/Features/EducationYears/DTOs/UpdateEducationYearRequest.cs:8
- **Instructor ID is required.**
  - Sources: Application/Features/EducationYears/Commands/CreateEducationYear/CreateEducationYearCommand.cs:12
- **Price must be greater than 0**
  - Sources: Application/Features/Sections/DTOs/BulkCreateSectionRequest.cs:14, Application/Features/Sections/DTOs/CreateSectionRequest.cs:10, Application/Features/Sections/DTOs/SectionUpdateRequest.cs:9
- **Rate Must be between 1 and 5.**
  - Sources: Application/Features/Reviews/DTOs/ReviewCreationRequest.cs:12, Application/Features/Reviews/DTOs/ReviewUpdateRequest.cs:16

## Exception (29 unique)

- **Course not found**
  - Sources: Application/Features/Courses/Commands/DeleteCourse/DeleteCourseCommandHandler.cs:20, Application/Features/Courses/Commands/UpdateCourse/UpdateCourseCommandHandler.cs:19
- **Course not found for this section**
  - Sources: Application/Features/Sections/Commands/DeleteSection/DeleteSectionCommandHandler.cs:34
- **Exam not found.**
  - Sources: Application/Features/Exams/Command/UpdateExam/UpdateExamCommandHandler.cs:21
- **ExamId cannot be empty.**
  - Sources: Infrastructure/Features/Exams/ExamRepository.cs:48
- **File is required and cannot be empty**
  - Sources: Infrastructure/Common/Services/CloudinaryService.cs:119, Infrastructure/Common/Services/CloudinaryService.cs:214, Infrastructure/Common/Services/CloudinaryService.cs:239
- **File name is required**
  - Sources: Infrastructure/Common/Services/CloudinaryService.cs:343
- **File path cannot be null or empty**
  - Sources: Infrastructure/Common/Services/CloudinaryService.cs:367
- **File size must be greater than 0.**
  - Sources: Infrastructure/Common/Services/CloudinaryService.cs:57
- **File stream is required and cannot be empty**
  - Sources: Infrastructure/Common/Services/CloudinaryService.cs:340
- **Google Client IDs not configured.**
  - Sources: Infrastructure/Features/Auth/GoogleAuthService.cs:21
- **Invalid Google token or email not verified.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandHandler.cs:29, Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandHandler.cs:28, Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandHandler.cs:28
- **Invalid refresh token**
  - Sources: Application/Features/Auth/Commands/UserLoginWithRefreshToken/LoginWithRefreshTokenCommandHandler.cs:21
- **JWT SecretKey not configured**
  - Sources: Edu_Base/Program.cs:178, Infrastructure/Features/Auth/JwtTokenService.cs:24
- **Login attempt detected from a different device.**
  - Sources: Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandHandler.cs:92
- **No Courses Found**
  - Sources: Application/Features/Exams/EventHandlers/ExamAddedEventHandler.cs:25, Application/Features/Exams/EventHandlers/ExamDeletedEventHandler.cs:25
- **Only PDF files are allowed.**
  - Sources: Infrastructure/Common/Services/CloudinaryService.cs:126
- **PDF file size exceeds 20MB limit.**
  - Sources: Infrastructure/Common/Services/CloudinaryService.cs:131
- **Public ID is required**
  - Sources: Infrastructure/Common/Services/CloudinaryService.cs:160, Infrastructure/Common/Services/CloudinaryService.cs:278, Infrastructure/Common/Services/CloudinaryService.cs:308
- **Refresh token has expired**
  - Sources: Application/Features/Auth/Commands/UserLoginWithRefreshToken/LoginWithRefreshTokenCommandHandler.cs:25
- **Section not found**
  - Sources: Application/Features/Sections/Commands/DeleteSection/DeleteSectionCommandHandler.cs:21
- **This account is already registered as an admin for a different center.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandHandler.cs:92
- **This email is registered as a Student account.**
  - Sources: Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandHandler.cs:38
- **This email is registered as an Instructor account.**
  - Sources: Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandHandler.cs:38
- **This email is registered with another role. Please use a different email for the Center Admin account.**
  - Sources: Application/Features/Auth/Commands/CenterAdminGoogleLogin/CenterAdminGoogleLoginCommandHandler.cs:46
- **Use AnswersSheet queries for student target.**
  - Sources: Infrastructure/Features/Sheets/SheetRepository.cs:21
- **Video not found**
  - Sources: Application/Features/Videos/Commands/DeleteVideo/DeleteVideoCommandHandler.cs:17, Application/Features/Videos/Commands/UpdateVideo/UpdateVideoCommandHandler.cs:19
- **You can only purchase content from instructors within your assigned Center.**
  - Sources: Infrastructure/Features/Centers/CenterContentScopeService.cs:65
- **You do not teach this course.**
  - Sources: Infrastructure/Features/Centers/InstructorContentScopeService.cs:36
- **You do not teach this section.**
  - Sources: Infrastructure/Features/Centers/InstructorContentScopeService.cs:49

## Exception (null-coalescing) (7 unique)

- **Course not found**
  - Sources: Application/Features/Courses/Commands/DeleteCourse/DeleteCourseCommandHandler.cs:20, Application/Features/Courses/Commands/UpdateCourse/UpdateCourseCommandHandler.cs:19
- **Course not found for this section**
  - Sources: Application/Features/Sections/Commands/DeleteSection/DeleteSectionCommandHandler.cs:34
- **Exam not found.**
  - Sources: Application/Features/Exams/Command/UpdateExam/UpdateExamCommandHandler.cs:21
- **Invalid refresh token**
  - Sources: Application/Features/Auth/Commands/UserLoginWithRefreshToken/LoginWithRefreshTokenCommandHandler.cs:21
- **JWT SecretKey not configured**
  - Sources: Edu_Base/Program.cs:178, Infrastructure/Features/Auth/JwtTokenService.cs:24
- **Section not found**
  - Sources: Application/Features/Sections/Commands/DeleteSection/DeleteSectionCommandHandler.cs:21
- **Video not found**
  - Sources: Application/Features/Videos/Commands/DeleteVideo/DeleteVideoCommandHandler.cs:17, Application/Features/Videos/Commands/UpdateVideo/UpdateVideoCommandHandler.cs:19

## Controller BadRequest (40 unique)

- **A PDF file is required.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:141, Edu_Base/Features/Sheets/SheetsController.cs:176
- **Answers sheet creation request can not be null.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:131
- **Answers sheet ID is required.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:171
- **Answers sheet update request can not be null.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:166
- **Cloudinary PublicId is required.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:114
- **Cloudinary SecureUrl is required.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:117
- **Course creation request cannot be null.**
  - Sources: Edu_Base/Features/Courses/CourseController.cs:27
- **Creation Request Of Videos Must Be Send**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:179
- **Due Date can not be null or in the past.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:52
- **Email parameter is required.**
  - Sources: Edu_Base/Features/Auth/SharedAuthController.cs:23
- **Entity ID and Student ID cannot be empty**
  - Sources: Edu_Base/Features/Reviews/ReviewController.cs:121
- **Entity ID cannot be empty**
  - Sources: Edu_Base/Features/Reviews/ReviewController.cs:103
- **FileName is required.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:72
- **FileSize must be greater than 0.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:75
- **Instructor Id can not be null.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:48
- **Invalid targetType.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:245
- **Progress must be between 0 and 100.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:247
- **Question ID in route does not match Question ID in body.**
  - Sources: Edu_Base/Features/Questions/QuestionController.cs:96
- **Questions sheet ID is required.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:136
- **Request cannot be null.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:239
- **Review creation request can not be null.**
  - Sources: Edu_Base/Features/Reviews/ReviewController.cs:27
- **Review deletion request can not be null**
  - Sources: Edu_Base/Features/Reviews/ReviewController.cs:68
- **Review ID cannot be empty**
  - Sources: Edu_Base/Features/Reviews/ReviewController.cs:86
- **Review update request can not be null.**
  - Sources: Edu_Base/Features/Reviews/ReviewController.cs:48
- **Save request is required.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:111
- **Section creation request cannot be null.**
  - Sources: Edu_Base/Features/Sections/SectionController.cs:25
- **Section Id can not be empty**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:277
- **Sheet creation request can not be null.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:44
- **Sheet must be associated with a course, section, or video. Please provide at least one valid identifier.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:56
- **Sheet must be associated with exactly one target: Course, Section, or Video.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:68
- **Sheet update request can not be null.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:94
- **sheetType must be TutorialSheet, QuestionSheet, or AnswersSheet.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:242
- **Signature request is required.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:69
- **targetId cannot be empty.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:248
- **Update request cannot be null.**
  - Sources: Edu_Base/Features/Sections/SectionController.cs:73
- **Update Request Of Video Must Be Send**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:196
- **Update video progress request must be sent.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:237
- **VideoId is required.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:244
- **When sheetType is AnswersSheet, targetType must be Student.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:253
- **When sheetType is TutorialSheet or QuestionSheet, targetType cannot be Student.**
  - Sources: Edu_Base/Features/Sheets/SheetsController.cs:258

## Controller Unauthorized (2 unique)

- **User id claim is missing or invalid.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:241
- **User id not found in token.**
  - Sources: Edu_Base/Features/Exams/ExamController.cs:135, Edu_Base/Features/Exams/ExamController.cs:160, Edu_Base/Features/Exams/ExamController.cs:184

## Middleware/Response (3 unique)

- **A server error occurred. For more details check server logs.**
  - Sources: Infrastructure/Common/Middleware/ExceptionLoggingMiddleware.cs:37
- **Invalid user ID**
  - Sources: Edu_Base/Features/Auth/InstructorAuthController.cs:64, Edu_Base/Features/Auth/StudentAuthController.cs:60
- **Your account has been restricted due to attempted screenshot violation**
  - Sources: Infrastructure/Common/Middleware/ScreenshotCheckMiddleware.cs:45

## Tuple message (3 unique)

- **Course not found**
  - Sources: Infrastructure/Features/Sheets/SheetService/SheetServiceFactory.cs:109
- **Section not found**
  - Sources: Infrastructure/Features/Sheets/SheetService/SheetServiceFactory.cs:113
- **Video not found**
  - Sources: Infrastructure/Features/Sheets/SheetService/SheetServiceFactory.cs:117

## Error field (4 unique)

- **Access denied**
  - Sources: Infrastructure/Common/Middleware/ScreenshotCheckMiddleware.cs:44
- **An internal server error occurred.**
  - Sources: Infrastructure/Common/Middleware/ExceptionLoggingMiddleware.cs:36
- **An unexpected error occurred while generating the upload signature.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:91
- **An unexpected error occurred while saving the video reference.**
  - Sources: Edu_Base/Features/Videos/VideoController.cs:135

## Dynamic / Template Messages (73 unique)

These messages include runtime placeholders such as `{ex.Message}`, entity IDs, or variable values.

- **A center with the name '{request.Request.Name}' already exists.** (Result.FailureStatusCode (dynamic)) — `Application/Features/Centers/Commands/CreateCenter/CreateCenterCommandHandler.cs:24`
- **An error occurred while checking if user exists: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Auth/Queries/CheckUserExists/CheckUserExistsQueryHandler.cs:54`
- **An error occurred while checking review existence: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:151`
- **An error occurred while creating the question: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Command/AddQuestion/AddQuestionCommandHandler.cs:58`
- **An error occurred while creating the review.{ex.Message}** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:64`
- **An error occurred while deleting the course: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Courses/Commands/DeleteCourse/DeleteCourseCommandHandler.cs:50`
- **An error occurred while deleting the section: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sections/Commands/DeleteSection/DeleteSectionCommandHandler.cs:57`
- **An error occurred while deleting the video: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Videos/Commands/DeleteVideo/DeleteVideoCommandHandler.cs:35`
- **An error occurred while processing the request: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Auth/Commands/UserLoginWithRefreshToken/LoginWithRefreshTokenCommandHandler.cs:56`
- **An error occurred while retrieving answers sheets: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Sheets/SheetService/SheetServiceFactory.cs:188`
- **An error occurred while retrieving courses: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Courses/Query/GetAllCourses/GetAllCoursesQueryHandler.cs:104`
- **An error occurred while retrieving exam questions: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Query/GetAllQuestionsWithAnswersInBank/GetAllQuestionsWithAnswersInBankQueryHandler.cs:43`
- **An error occurred while retrieving home screen data: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/HomeScreen/StudentHomeScreen/HomeScreenQueryHandler.cs:30`
- **An error occurred while retrieving instructor dashboard data: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/HomeScreen/InstructorDashboard/InstructorDashboardQueryHandler.cs:29`
- **An error occurred while retrieving questions: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Query/GetAllQuestionsInExam/GetAllQuestionsInExamQueryHandler.cs:39`
- **An error occurred while retrieving reviews: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Reviews/Query/GetAllReviews/GetAllReviewsQueryHandler.cs:36`
- **An error occurred while retrieving section details: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sections/Query/GetSectionDetails/GetSectionDetailsQueryHandler.cs:30`
- **An error occurred while retrieving sheets: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Sheets/SheetService/SheetServiceFactory.cs:93`
- **An error occurred while retrieving student progress: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/HomeScreen/StudentProgress/StudentProgressQueryHandler.cs:36`
- **An error occurred while retrieving the course: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Courses/Query/GetCourseById/GetCourseByIdQueryHandler.cs:30`
- **An error occurred while retrieving the question: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Query/GetQuestionById/GetQuestionByIdQueryHandler.cs:31`
- **An error occurred while retrieving the review: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:322`
- **An error occurred while retrieving videos: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Videos/Queries/GetAllVideos/GetAllVideosQueryHandler.cs:60`
- **An error occurred while updating the course: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Courses/Commands/UpdateCourse/UpdateCourseCommandHandler.cs:69`
- **An error occurred while updating the exam: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Exams/Command/UpdateExam/UpdateExamCommandHandler.cs:91`
- **An error occurred while updating the video: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Videos/Commands/UpdateVideo/UpdateVideoCommandHandler.cs:51`
- **Could not find course with {notification.CourseId} Found** (Exception (null-coalescing, dynamic)) — `Application/Features/Exams/EventHandlers/ExamDeletedEventHandler.cs:27`
- **Could not find Section with {notification.SectionId} Found** (Exception (null-coalescing, dynamic)) — `Application/Features/Exams/EventHandlers/ExamDeletedEventHandler.cs:29`
- **Course with ID {request.CourseId} not found.** (Exception (null-coalescing, dynamic)) — `Application/Features/Courses/Query/GetCourseById/GetCourseByIdQueryHandler.cs:20`
- **Error creating answers sheet: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/AnswersSheet/Commands/CreateAnswersSheet/CreateAnswersSheetCommandHandler.cs:202`
- **Error creating course: {ex.Message} {ex.InnerException?.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Courses/Commands/CreateCourse/CreateCourseCommandHandler.cs:76`
- **Error creating section: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sections/Commands/CreateSection/CreateSectionCommandHandler.cs:54`
- **Error creating sheet: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sheets/Commands/CreateSheet/CreateSheetCommandHandler.cs:108`
- **Error creating video: {ex.Message} {ex.InnerException?.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Videos/Commands/CreateVideo/CreateVideoCommandHandler.cs:60`
- **Error deleting answers sheet with Public Id: {answersSheet.SheetPublicId}** (Result.FailureStatusCode (dynamic)) — `Application/Features/AnswersSheet/Commands/DeleteAnswersSheet/DeleteAnswersSheetCommandHandler.cs:60`
- **Error deleting answers sheet: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/AnswersSheet/Commands/DeleteAnswersSheet/DeleteAnswersSheetCommandHandler.cs:68`
- **Error deleting question: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Command/DeleteQuestion/DeleteQuestionCommandHandler.cs:50`
- **Error deleting review: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:97`
- **Error deleting sheet with Public Id: {sheet.SheetPublicId}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sheets/Commands/DeleteSheet/DeleteSheetCommandHandler.cs:33`
- **Error deleting sheet: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sheets/Commands/DeleteSheet/DeleteSheetCommandHandler.cs:41`
- **Error during Google login: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandHandler.cs:143`
- **Error during Google login: {ex.Message}{innerMsg}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandHandler.cs:137`
- **Error in bulk create for videos: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Videos/Commands/CreateVideo/BulkCreateVideosCommandHandler.cs:66`
- **Error in bulk create: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sections/Commands/CreateSection/BulkCreateSectionCommandHandler.cs:63`
- **Error updating answers sheet: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/AnswersSheet/Commands/UpdateAnswersSheet/UpdateAnswersSheetCommandHandler.cs:92`
- **Error updating section: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sections/Commands/UpdateSection/UpdateSectionCommandHandler.cs:51`
- **Error updating sheet: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sheets/Commands/UpdateSheet/UpdateSheetCommandHandler.cs:53`
- **Error validating Google token: {ex.Message}** (Exception (dynamic)) — `Infrastructure/Features/Auth/GoogleAuthService.cs:43`
- **Failed to add answer: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Answers/Command/AnswerQuestion/AddAnswerToQuestionCommandHandler.cs:44`
- **Failed to assign instructor to center: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Centers/Commands/AssignInstructorToCenter/AssignInstructorToCenterCommandHandler.cs:63`
- **Failed to create center: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Centers/Commands/CreateCenter/CreateCenterCommandHandler.cs:53`
- **Failed to delete center: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Centers/Commands/DeleteCenter/DeleteCenterCommandHandler.cs:36`
- **Failed to remove instructor from center: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Centers/Commands/RemoveInstructorFromCenter/RemoveInstructorFromCenterCommandHandler.cs:42`
- **Failed to update center: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Centers/Commands/UpdateCenter/UpdateCenterCommandHandler.cs:56`
- **Failed to update question: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Command/UpdateQuestion/UpdateQuestionCommandHandler.cs:62`
- **File not found at path: {filePath}** (Exception (dynamic)) — `Infrastructure/Common/Services/CloudinaryService.cs:370`
- **File size ({request.FileSize / (1024 * 1024)}MB) exceeds the maximum allowed size of {maxFileSize / (1024 * 1024)}MB.** (Exception (dynamic)) — `Infrastructure/Common/Services/CloudinaryService.cs:60`
- **File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB** (Exception (dynamic)) — `Infrastructure/Common/Services/CloudinaryService.cs:251`
- **Invalid file type '{fileExtension}'. Allowed types: {string.Join(** (Exception (dynamic)) — `Infrastructure/Common/Services/CloudinaryService.cs:50`
- **Invalid file type. Allowed types: {string.Join(** (Exception (dynamic)) — `Infrastructure/Common/Services/CloudinaryService.cs:246`
- **Invalid JWT token: {ex.Message}** (Exception (dynamic)) — `Infrastructure/Features/Auth/GoogleAuthService.cs:39`
- **Invalid usage category: {imageType}** (Exception (dynamic)) — `Infrastructure/Common/Services/CloudinaryService.cs:445`
- **Logout failed: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Application/Features/Auth/Commands/GoogleLogout/GoogleLogoutCommandHandler.cs:39`
- **No questions found for bank with ID {request.BankId}.** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Query/GetAllQuestionsInExam/GetAllQuestionsInExamQueryHandler.cs:24`
- **No questions found for exam with ID {questionRequest.Id}.** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Query/GetAllQuestionsWithAnswersInBank/GetAllQuestionsWithAnswersInBankQueryHandler.cs:28`
- **No questions found for exam with ID {request.ExamId}.** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Query/GetAllQuestionsWithAnswersInExam/GetAllQuestionsWithAnswersInExamQueryHandler.cs:23`
- **No reviews found for entity with ID {request.EntityId}.** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:255`
- **Not enough questions available. Requested: {request.NumberOfQuestions}, Available: {question.Count()}.** (Result.FailureStatusCode (dynamic)) — `Application/Features/Exams/Command/GenerateExam/GenerateExamCommandHandler.cs:35`
- **Question with ID {request.QuestionId} not found.** (Result.FailureStatusCode (dynamic)) — `Application/Features/Questions/Query/GetQuestionById/GetQuestionByIdQueryHandler.cs:22`
- **Review not found** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:78`
- **Review with ID {reviewId} not found.** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:296`
- **Section with ID {request.SectionId} was not found.** (Result.FailureStatusCode (dynamic)) — `Application/Features/Sections/Query/GetSectionDetails/GetSectionDetailsQueryHandler.cs:22`
- **Unauthorized access: {ex.Message}** (Result.FailureStatusCode (dynamic)) — `Infrastructure/Features/Reviews/ReviewService/ReviewServiceBase.cs:60`

## Propagated Messages

Some handlers return exception messages directly without a fixed string:
- `auth.Message` / `authEx.Message` / `uaEx.Message` — from caught `UnauthorizedAccessException`
- `ex.Message` — from caught generic exceptions
- `existence.message` — from sheet existence checks in `SheetServiceFactory`
- `knfEx.Message` — from caught `KeyNotFoundException`

