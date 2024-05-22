﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OQS.CoreWebAPI.Database;

#nullable disable

namespace OQS.CoreWebAPI.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    partial class ApplicationDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ActiveQuiz.ActiveQuiz", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("QuizId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.HasIndex("UserId");

                    b.ToTable("ActiveQuizzes");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.QuestionBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AlocatedPoints")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("nvarchar(34)");

                    b.Property<Guid>("QuizId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TimeLimit")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.ToTable("Questions");

                    b.HasDiscriminator<string>("Discriminator").HasValue("QuestionBase");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.Quiz", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TimeLimitMinutes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Quizzes");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ChoiceQuestionBase", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.QuestionBase");

                    b.Property<string>("Choices")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("ChoiceQuestionBase");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ReviewNeededQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.QuestionBase");

                    b.HasDiscriminator().HasValue("ReviewNeededQuestion");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.TrueFalseQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.QuestionBase");

                    b.Property<bool>("TrueFalseAnswer")
                        .HasColumnType("bit");

                    b.HasDiscriminator().HasValue("TrueFalseQuestion");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.WrittenAnswerQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.QuestionBase");

                    b.Property<string>("WrittenAcceptedAnswers")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("WrittenAnswerQuestion");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.MultipleChoiceQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.ChoiceQuestionBase");

                    b.Property<string>("MultipleChoiceAnswers")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("MultipleChoiceQuestion");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.SingleChoiceQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.ChoiceQuestionBase");

                    b.Property<string>("SingleChoiceAnswer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("SingleChoiceQuestion");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ActiveQuiz.ActiveQuiz", b =>
                {
                    b.HasOne("OQS.CoreWebAPI.Entities.Quiz", "Quiz")
                        .WithMany()
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OQS.CoreWebAPI.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Quiz");

                    b.Navigation("User");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.QuestionBase", b =>
                {
                    b.HasOne("OQS.CoreWebAPI.Entities.Quiz", null)
                        .WithMany("Questions")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.Quiz", b =>
                {
                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
