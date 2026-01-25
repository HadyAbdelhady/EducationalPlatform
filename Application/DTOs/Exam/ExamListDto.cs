using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.enums;
using Domain.Interfaces;

namespace Application.DTOs.Exam
{
    public class ExamListDto
    {
        public Guid ExamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TotalMark { get; set; }
        public int NumberOfQuestions { get; set; }
        public int? DurationInMinutes { get; set; }
        public bool IsRandomized { get; set; }
        public int PassMarkPercentage { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamType ExamType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamStatus ExamStatus { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamResultStatus StudentExamStatusResult { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public DateTimeOffset? TakenAt { get; set; }
        public decimal ObtainedMarks { get; set; }
        public bool IsTaken {get; set; } 
        //public int NotStartedCount { get; set; }
        //public int InProgressCount { get; set; }
        //public int PassedCount { get; set; }
        //public int FailedCount { get; set; }
        //public int CompletedCount { get; set; }
    }

}


