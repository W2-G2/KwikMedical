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
    [Migration("20230814123950_CallDecide")]
    partial class CallDecide
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

                    b.HasKey("Id");

                    b.HasIndex("HospitalId");

                    b.ToTable("Ambulances");
                });

            modelBuilder.Entity("KwikMedical.Models.EmergencyCall", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AmbulanceId")
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

                    b.Property<bool>("IsAccepted")
                        .HasColumnType("bit");

                    b.Property<string>("NearestHospital")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AmbulanceId")
                        .IsUnique()
                        .HasFilter("[AmbulanceId] IS NOT NULL");

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

                    b.HasKey("Id");

                    b.ToTable("Hospitals");
                });

            modelBuilder.Entity("KwikMedical.Models.MedicalRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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
                    b.HasOne("KwikMedical.Models.Hospital", "Hospital")
                        .WithMany("Ambulances")
                        .HasForeignKey("HospitalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hospital");
                });

            modelBuilder.Entity("KwikMedical.Models.EmergencyCall", b =>
                {
                    b.HasOne("KwikMedical.Models.Ambulance", "Ambulance")
                        .WithOne("CurrentEmergencyCall")
                        .HasForeignKey("KwikMedical.Models.EmergencyCall", "AmbulanceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("KwikMedical.Models.Patient", "Patient")
                        .WithMany()
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

            modelBuilder.Entity("KwikMedical.Models.Patient", b =>
                {
                    b.Navigation("MedicalRecord");
                });
#pragma warning restore 612, 618
        }
    }
}
