﻿// <auto-generated />
using System;
using KwikMedical.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace KwikMedical.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230819132011_backup")]
    partial class backup
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("KwikMedical.Models.Ambulance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AssignedAmbulanceId")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentCity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CurrentEmergencyCallId")
                        .HasColumnType("int");

                    b.Property<int>("HospitalId")
                        .HasColumnType("int");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<int?>("MedicalRecordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssignedAmbulanceId");

                    b.HasIndex("HospitalId");

                    b.HasIndex("MedicalRecordId")
                        .IsUnique()
                        .HasFilter("[MedicalRecordId] IS NOT NULL");

                    b.ToTable("Ambulances");
                });

            modelBuilder.Entity("KwikMedical.Models.EmergencyCall", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AmbulanceId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("EmergencyCity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmergencyDetails")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmergencyStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EstimatedArrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsAccepted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("MedicalCondition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NearestHospital")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OperatorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AmbulanceId")
                        .IsUnique();

                    b.HasIndex("PatientId");

                    b.ToTable("EmergencyCalls");
                });

            modelBuilder.Entity("KwikMedical.Models.Hospital", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreparationStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Hospitals");
                });

            modelBuilder.Entity("KwikMedical.Models.MedicalRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AmbulanceId")
                        .HasColumnType("int");

                    b.Property<string>("ClinicalNotes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LaboratoryReports")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Letters")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<string>("PrescriptionCharts")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TelephoneCalls")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Xrays")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PatientId")
                        .IsUnique();

                    b.ToTable("MedicalRecords");
                });

            modelBuilder.Entity("KwikMedical.Models.Patient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NHSNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("KwikMedical.Models.Ambulance", b =>
                {
                    b.HasOne("KwikMedical.Models.Ambulance", "AssignedAmbulance")
                        .WithMany()
                        .HasForeignKey("AssignedAmbulanceId");

                    b.HasOne("KwikMedical.Models.Hospital", "Hospital")
                        .WithMany("Ambulances")
                        .HasForeignKey("HospitalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KwikMedical.Models.MedicalRecord", "MedicalRecord")
                        .WithOne("Ambulance")
                        .HasForeignKey("KwikMedical.Models.Ambulance", "MedicalRecordId");

                    b.Navigation("AssignedAmbulance");

                    b.Navigation("Hospital");

                    b.Navigation("MedicalRecord");
                });

            modelBuilder.Entity("KwikMedical.Models.EmergencyCall", b =>
                {
                    b.HasOne("KwikMedical.Models.Ambulance", "Ambulance")
                        .WithOne("CurrentEmergencyCall")
                        .HasForeignKey("KwikMedical.Models.EmergencyCall", "AmbulanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KwikMedical.Models.Patient", "Patient")
                        .WithMany("EmergencyCalls")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ambulance");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("KwikMedical.Models.MedicalRecord", b =>
                {
                    b.HasOne("KwikMedical.Models.Patient", "Patient")
                        .WithOne("MedicalRecord")
                        .HasForeignKey("KwikMedical.Models.MedicalRecord", "PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("KwikMedical.Models.Ambulance", b =>
                {
                    b.Navigation("CurrentEmergencyCall")
                        .IsRequired();
                });

            modelBuilder.Entity("KwikMedical.Models.Hospital", b =>
                {
                    b.Navigation("Ambulances");
                });

            modelBuilder.Entity("KwikMedical.Models.MedicalRecord", b =>
                {
                    b.Navigation("Ambulance")
                        .IsRequired();
                });

            modelBuilder.Entity("KwikMedical.Models.Patient", b =>
                {
                    b.Navigation("EmergencyCalls");

                    b.Navigation("MedicalRecord")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
