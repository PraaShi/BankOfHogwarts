﻿using BankOfHogwarts.Models;
using BankOfHogwarts.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankOfHogwarts.DTOs
{
    public class LoanHistoryDto
    {
        public int AccountId { get; set; }
        public int LoanId { get; set; }
        public string LoanType { get; set; } // Human-readable loan type


        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int Tenure { get; set; } // Duration in months
        public string Purpose { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string LoanApplicationStatus { get; set; }
        public DateTime? ApprovedDate { get; set; }// Enum to string
        public string LoanStatus { get; set; } // Enum to string
        public DateTime? DisbursementDate { get; set; }
        public string? Remarks { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LoanFinalStatus LoanFinalStatus { get; set; }
        public DateTime? ClosedDate { get; set; }



    }
    }

