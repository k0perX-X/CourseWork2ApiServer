using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientsDrug> PatientsDrugs { get; set; }
        public DbSet<Drug> Drugs { get; set; }
        public DbSet<TakenMedication> TakenMedications { get; set; }
        public DbSet<DoctorsAppointment> DoctorsAppointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<PrescribedMedication> PrescribedMedications { get; set; }
        public DbSet<PatientProcedure> PatientProcedures { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<WellBeingRecord> WellBeingRecords { get; set; }
        public DbSet<OAuth> OAuths { get; set; }

        //public string DbPath { get; }
        public MyDbContext()
        {
            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);
            //DbPath = System.IO.Path.Join(path, "Test.db");
            //Database.EnsureCreated();
            SaveChangesTask = new Task(() => SaveChanges());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientsDrug>().HasKey(c =>
                new { c.PatientId, c.DrugId, c.DateOfManufacture });
            modelBuilder.Entity<TakenMedication>().HasKey(c =>
                new { c.PatientId, c.DrugId, c.DateTime });
            modelBuilder.Entity<DoctorsAppointment>().HasKey(c =>
                new { c.PatientId, c.DoctorId, c.DateTime });
            modelBuilder.Entity<PrescribedMedication>().HasKey(c =>
                new { c.DoctorsAppointmentId, c.DrugId });
            modelBuilder.Entity<PatientProcedure>().HasKey(c =>
                new { c.PatientId, c.ProcedureId, c.DateTime });
            modelBuilder.Entity<WellBeingRecord>().HasKey(c =>
                new { c.PatientId, c.DateTime });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=True;");
            //optionsBuilder.UseSqlite($"Data Source={DbPath}");
            optionsBuilder.UseNpgsql(ConnectionStringClass.ConnectionString);
        }

        public Task SaveChangesTask;
    }
}
