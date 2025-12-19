using Application.DTOs.Exam;
using Application.Features.Exam.Command.SubmitExam;

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
    }
}
