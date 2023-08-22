using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DockerImpliciteTest.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaceReaders",
                columns: table => new
                {
                    FaceReaderId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceReaders", x => x.FaceReaderId);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.TestId);
                });

            migrationBuilder.CreateTable(
                name: "FaceReaderData",
                columns: table => new
                {
                    FaceReaderDataId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<int>(type: "INTEGER", nullable: false),
                    Neutral = table.Column<double>(type: "REAL", nullable: false),
                    Happy = table.Column<double>(type: "REAL", nullable: false),
                    Sad = table.Column<double>(type: "REAL", nullable: false),
                    Angry = table.Column<double>(type: "REAL", nullable: false),
                    Surprised = table.Column<double>(type: "REAL", nullable: false),
                    Scared = table.Column<double>(type: "REAL", nullable: false),
                    Disgusted = table.Column<double>(type: "REAL", nullable: false),
                    Contempt = table.Column<double>(type: "REAL", nullable: false),
                    FaceReaderId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceReaderData", x => x.FaceReaderDataId);
                    table.ForeignKey(
                        name: "FK_FaceReaderData_FaceReaders_FaceReaderId",
                        column: x => x.FaceReaderId,
                        principalTable: "FaceReaders",
                        principalColumn: "FaceReaderId");
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TestId = table.Column<int>(type: "INTEGER", nullable: true),
                    TestId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Categories_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestId");
                    table.ForeignKey(
                        name: "FK_Categories_Tests_TestId1",
                        column: x => x.TestId1,
                        principalTable: "Tests",
                        principalColumn: "TestId");
                });

            migrationBuilder.CreateTable(
                name: "Fases",
                columns: table => new
                {
                    FaseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    FaseType = table.Column<int>(type: "INTEGER", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    ImgAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    TestId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fases", x => x.FaseId);
                    table.ForeignKey(
                        name: "FK_Fases_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestId");
                });

            migrationBuilder.CreateTable(
                name: "ImageUploads",
                columns: table => new
                {
                    ImageUploadId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TestId = table.Column<int>(type: "INTEGER", nullable: true),
                    TestId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageUploads", x => x.ImageUploadId);
                    table.ForeignKey(
                        name: "FK_ImageUploads_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestId");
                    table.ForeignKey(
                        name: "FK_ImageUploads_Tests_TestId1",
                        column: x => x.TestId1,
                        principalTable: "Tests",
                        principalColumn: "TestId");
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Participant = table.Column<string>(type: "TEXT", nullable: false),
                    TestId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeLineResult = table.Column<string>(type: "TEXT", nullable: false),
                    FaceReaderId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_Results_FaceReaders_FaceReaderId",
                        column: x => x.FaceReaderId,
                        principalTable: "FaceReaders",
                        principalColumn: "FaceReaderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Results_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryImageUpload",
                columns: table => new
                {
                    CategoriesCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUploadsImageUploadId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryImageUpload", x => new { x.CategoriesCategoryId, x.ImageUploadsImageUploadId });
                    table.ForeignKey(
                        name: "FK_CategoryImageUpload_Categories_CategoriesCategoryId",
                        column: x => x.CategoriesCategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryImageUpload_ImageUploads_ImageUploadsImageUploadId",
                        column: x => x.ImageUploadsImageUploadId,
                        principalTable: "ImageUploads",
                        principalColumn: "ImageUploadId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUploadId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_Images_ImageUploads_ImageUploadId",
                        column: x => x.ImageUploadId,
                        principalTable: "ImageUploads",
                        principalColumn: "ImageUploadId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaseTypeImages",
                columns: table => new
                {
                    FaseTypeImageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FaseType = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageId = table.Column<int>(type: "INTEGER", nullable: false),
                    FaseId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaseTypeImages", x => x.FaseTypeImageId);
                    table.ForeignKey(
                        name: "FK_FaseTypeImages_Fases_FaseId",
                        column: x => x.FaseId,
                        principalTable: "Fases",
                        principalColumn: "FaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaseTypeImages_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name", "TestId", "TestId1" },
                values: new object[,]
                {
                    { -6, "Negative", null, null },
                    { -5, "Positive", null, null },
                    { -4, "Oud", null, null },
                    { -3, "Jong", null, null },
                    { -2, "Vrouwen", null, null },
                    { -1, "Mannen", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TestId",
                table: "Categories",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TestId1",
                table: "Categories",
                column: "TestId1");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryImageUpload_ImageUploadsImageUploadId",
                table: "CategoryImageUpload",
                column: "ImageUploadsImageUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceReaderData_FaceReaderId",
                table: "FaceReaderData",
                column: "FaceReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Fases_TestId",
                table: "Fases",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_FaseTypeImages_FaseId",
                table: "FaseTypeImages",
                column: "FaseId");

            migrationBuilder.CreateIndex(
                name: "IX_FaseTypeImages_ImageId",
                table: "FaseTypeImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ImageUploadId",
                table: "Images",
                column: "ImageUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageUploads_TestId",
                table: "ImageUploads",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageUploads_TestId1",
                table: "ImageUploads",
                column: "TestId1");

            migrationBuilder.CreateIndex(
                name: "IX_Results_FaceReaderId",
                table: "Results",
                column: "FaceReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_TestId",
                table: "Results",
                column: "TestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryImageUpload");

            migrationBuilder.DropTable(
                name: "FaceReaderData");

            migrationBuilder.DropTable(
                name: "FaseTypeImages");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Fases");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "FaceReaders");

            migrationBuilder.DropTable(
                name: "ImageUploads");

            migrationBuilder.DropTable(
                name: "Tests");
        }
    }
}
