﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enities.Migrations
{
    /// <inheritdoc />
    public partial class GetPersons_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                Create PROCEDURE [dbo].[GetAllPersons]
                AS BEGIN
                SELECT * FROM [dbo].[Persons]
                END";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
            DROP PROCEDURE [dbo].[GetAllPersons]";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
