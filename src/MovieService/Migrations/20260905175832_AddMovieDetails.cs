using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieService.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Budget",
                table: "Movies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Genres",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginalLanguage",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginalTitle",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Overview",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Popularity",
                table: "Movies",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Movies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Revenue",
                table: "Movies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Runtime",
                table: "Movies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tagline",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TmdbId",
                table: "Movies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "VoteAverage",
                table: "Movies",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "VoteCount",
                table: "Movies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Genres",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "OriginalLanguage",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "OriginalTitle",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Overview",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Popularity",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Runtime",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Tagline",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "TmdbId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "VoteAverage",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "VoteCount",
                table: "Movies");
        }
    }
}
