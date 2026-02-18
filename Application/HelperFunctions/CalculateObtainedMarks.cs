using Application.DTOs.Exam;
using Application.Features.Exams.Command.SubmitExam;
using Domain.Entities;

namespace Application.HelperFunctions
{
    public static class CalculateObtainedMarks
    {
        public static decimal Calculate(ExamModelAnswer examModelAnswer, SubmitExamCommand submissionExam)
        {
            var correctAnswers = examModelAnswer.Questions
                                                                                .ToDictionary(q => q.QuestionId,
                                                                                            q => q);

            decimal obtainedMarks = 0;

            foreach (var studentAnswer in submissionExam.Answers)
            {
                if (correctAnswers.TryGetValue(studentAnswer.QuestionId, out QuestionModelAnswer? ModelAnswer))
                {
                    bool isCorrect = studentAnswer.ChosenAnswerId == ModelAnswer.CorrectAnswerId;

                    if (isCorrect)
                    {
                        obtainedMarks += ModelAnswer.QuestionMark;
                    }
                }
            }

            return obtainedMarks;
        }

        public static decimal Calculate(ExamModelAnswer examModelAnswer, IEnumerable<Domain.Entities.StudentAnswers> studentSubmissions)
        {
            var correctAnswers = examModelAnswer.Questions
                .ToDictionary(q => q.QuestionId, q => q);

            decimal obtainedMarks = 0;

            foreach (var submission in studentSubmissions)
            {
                if (correctAnswers.TryGetValue(submission.QuestionId, out QuestionModelAnswer? modelAnswer))
                {
                    bool isCorrect = submission.ChosenAnswerId == modelAnswer.CorrectAnswerId;

                    if (isCorrect)
                    {
                        obtainedMarks += modelAnswer.QuestionMark;
                    }
                }
            }

            return obtainedMarks;
        }
    }
}
