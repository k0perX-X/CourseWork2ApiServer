using System.ComponentModel.DataAnnotations;

namespace CourseWork2ApiServer.Models
{
    public class Patient
    {
        //[Key]
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Surname { get; set; }
        public string? MiddleName { get; set; }
        [Required] public DateTime Birthdate { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        public virtual List<PatientProcedure> Procedures { get; set; }
        public virtual List<WellBeingRecord> WellBeingRecords { get; set; }
        public virtual List<TakenMedication> TakenMedications { get; set; }
        public virtual List<PatientsDrug> PatientsDrugs { get; set; }
        public virtual List<DoctorsAppointment> DoctorsAppointments { get; set; }
        public virtual List<OAuth> Tokens { get; set; }
    }

    public class OAuth
    {
        [Key] [Required] public string Token { get; set; }
        public string? OtherInformation { get; set; }
        [Required] public DateTime CreateTime { get; set; }
        [Required] public string DeviceInformation { get; set; }
        [Required] public virtual Patient Patient { get; set; }
        [Required] public int PatientId { get; set; }
    }

    public class PatientsDrug
    {
        [Required] public int Id { get; set; }

        //[Key]
        [Required] public virtual Patient Patient { get; set; }
        [Required] public int PatientId { get; set; }


        //[Key]
        [Required] public virtual Drug Drug { get; set; }
        [Required] public int DrugId { get; set; }

        [Required] public int Remaining { get; set; }

        //[Key]
        [Required] public DateTime DateOfManufacture { get; set; }
    }

    public class Drug
    {
        //[Key]
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        public string? Note { get; set; }
        [Required] public DateTime ExplorationDate { get; set; }
    }

    public class TakenMedication
    {
        [Required] public int Id { get; set; }

        //[Key]
        [Required] public virtual Patient Patient { get; set; }
        [Required] public int PatientId { get; set; }

        //[Key]
        [Required] public virtual Drug Drug { get; set; }
        [Required] public int DrugId { get; set; }

        //[Key]
        [Required] public DateTime DateTime { get; set; }
        [Required] public bool ReceptionTimeInTheMorning { get; set; }
        [Required] public bool ReceptionTimeDuringTheDay { get; set; }
        [Required] public bool ReceptionTimeInTheEvening { get; set; }
    }

    public class DoctorsAppointment
    {
        [Required] public int Id { get; set; }

        //[Key]
        [Required] public virtual Patient Patient { get; set; }
        [Required] public int PatientId { get; set; }

        //[Key]
        [Required] public virtual Doctor Doctor { get; set; }
        [Required] public int DoctorId { get; set; }
        public double? PatientTemperature { get; set; }

        public string? Note { get; set; }

        //[Key]
        [Required] public DateTime DateTime { get; set; }
        [Required] public bool Visited { get; set; }

        public virtual List<PrescribedMedication> PrescribedMedications { get; set; }
    }

    public class Doctor
    {
        //[Key]
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Surname { get; set; }
        public string MiddleName { get; set; }
        [Required] public DateTime Birthdate { get; set; }
        public string? Note { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }

    public class PrescribedMedication
    {
        [Required] public int Id { get; set; }

        //[Key]
        [Required] public virtual DoctorsAppointment DoctorsAppointment { get; set; }
        [Required] public int DoctorsAppointmentId { get; set; }

        //[Key]
        [Required] public virtual Drug Drug { get; set; }
        [Required] public int DrugId { get; set; }
        [Required] public bool ReceptionTimeInTheMorning { get; set; }
        [Required] public bool ReceptionTimeDuringTheDay { get; set; }
        [Required] public bool ReceptionTimeInTheEvening { get; set; }
        [Required] public bool TakeBeforeMeals { get; set; }
        [Required] public bool TakeAfterMeals { get; set; }
        [Required] public bool TakeWithMeals { get; set; }

        public string? Note { get; set; }

        [Required] public DateTime TakeMedicineBeforeTheDate { get; set; }
    }

    public class PatientProcedure
    {
        [Required] public int Id { get; set; }

        //[Key]
        [Required] public DateTime DateTime { get; set; }

        //[Key]
        [Required] public virtual Procedure Procedure { get; set; }
        [Required] public int ProcedureId { get; set; }

        public string? Note { get; set; }

        //[Key]
        [Required] public virtual Patient Patient { get; set; }
        [Required] public int PatientId { get; set; }
        [Required] public bool Visited { get; set; }
    }

    public class Procedure
    {
        //[Key]
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        public string? Note { get; set; }
    }

    public class WellBeingRecord
    {
        [Required] public int Id { get; set; }

        //[Key]
        [Required] public DateTime DateTime { get; set; }
        public double? Temperature { get; set; }

        public string? Note { get; set; }

        //[Key]
        [Required] public virtual Patient Patient { get; set; }
        [Required] public int PatientId { get; set; }
    }
}