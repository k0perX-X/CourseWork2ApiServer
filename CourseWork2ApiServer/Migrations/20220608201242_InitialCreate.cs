using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWork2ApiServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drugs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExplorationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drugs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoctorsAppointments",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    PatientTemperature = table.Column<double>(type: "float", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visited = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorsAppointments", x => new { x.PatientId, x.DoctorId, x.DateTime });
                    table.ForeignKey(
                        name: "FK_DoctorsAppointments_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorsAppointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OAuths",
                columns: table => new
                {
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OtherInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceInformation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuths", x => x.Token);
                    table.ForeignKey(
                        name: "FK_OAuths_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientsDrugs",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DrugId = table.Column<int>(type: "int", nullable: false),
                    DateOfManufacture = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Remaining = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientsDrugs", x => new { x.PatientId, x.DrugId, x.DateOfManufacture });
                    table.ForeignKey(
                        name: "FK_PatientsDrugs_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientsDrugs_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TakenMedications",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DrugId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    ReceptionTimeInTheMorning = table.Column<bool>(type: "bit", nullable: false),
                    ReceptionTimeDuringTheDay = table.Column<bool>(type: "bit", nullable: false),
                    ReceptionTimeInTheEvening = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TakenMedications", x => new { x.PatientId, x.DrugId, x.DateTime });
                    table.ForeignKey(
                        name: "FK_TakenMedications_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TakenMedications_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WellBeingRecords",
                columns: table => new
                {
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WellBeingRecords", x => new { x.PatientId, x.DateTime });
                    table.ForeignKey(
                        name: "FK_WellBeingRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientProcedures",
                columns: table => new
                {
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcedureId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visited = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientProcedures", x => new { x.PatientId, x.ProcedureId, x.DateTime });
                    table.ForeignKey(
                        name: "FK_PatientProcedures_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientProcedures_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescribedMedications",
                columns: table => new
                {
                    DoctorsAppointmentId = table.Column<int>(type: "int", nullable: false),
                    DrugId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    DoctorsAppointmentPatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorsAppointmentDoctorId = table.Column<int>(type: "int", nullable: false),
                    DoctorsAppointmentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceptionTimeInTheMorning = table.Column<bool>(type: "bit", nullable: false),
                    ReceptionTimeDuringTheDay = table.Column<bool>(type: "bit", nullable: false),
                    ReceptionTimeInTheEvening = table.Column<bool>(type: "bit", nullable: false),
                    TakeBeforeMeals = table.Column<bool>(type: "bit", nullable: false),
                    TakeAfterMeals = table.Column<bool>(type: "bit", nullable: false),
                    TakeWithMeals = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TakeMedicineBeforeTheDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescribedMedications", x => new { x.DoctorsAppointmentId, x.DrugId });
                    table.ForeignKey(
                        name: "FK_PrescribedMedications_DoctorsAppointments_DoctorsAppointmentPatientId_DoctorsAppointmentDoctorId_DoctorsAppointmentDateTime",
                        columns: x => new { x.DoctorsAppointmentPatientId, x.DoctorsAppointmentDoctorId, x.DoctorsAppointmentDateTime },
                        principalTable: "DoctorsAppointments",
                        principalColumns: new[] { "PatientId", "DoctorId", "DateTime" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrescribedMedications_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorsAppointments_DoctorId",
                table: "DoctorsAppointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuths_PatientId",
                table: "OAuths",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProcedures_ProcedureId",
                table: "PatientProcedures",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientsDrugs_DrugId",
                table: "PatientsDrugs",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescribedMedications_DoctorsAppointmentPatientId_DoctorsAppointmentDoctorId_DoctorsAppointmentDateTime",
                table: "PrescribedMedications",
                columns: new[] { "DoctorsAppointmentPatientId", "DoctorsAppointmentDoctorId", "DoctorsAppointmentDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_PrescribedMedications_DrugId",
                table: "PrescribedMedications",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "IX_TakenMedications_DrugId",
                table: "TakenMedications",
                column: "DrugId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuths");

            migrationBuilder.DropTable(
                name: "PatientProcedures");

            migrationBuilder.DropTable(
                name: "PatientsDrugs");

            migrationBuilder.DropTable(
                name: "PrescribedMedications");

            migrationBuilder.DropTable(
                name: "TakenMedications");

            migrationBuilder.DropTable(
                name: "WellBeingRecords");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "DoctorsAppointments");

            migrationBuilder.DropTable(
                name: "Drugs");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
