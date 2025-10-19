using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAFABRICA.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLIENT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PHONE_NUMBER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MANAGER = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MANAGER_PHONE_NUMBER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LOCATION = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SPECIFIC_LOCATION = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QUANTITY_PURCHASE = table.Column<int>(type: "int", nullable: true),
                    IS_FREQUENT = table.Column<byte>(type: "tinyint", nullable: false),
                    IS_ACTIVE = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CLIENT__3214EC2706FAB57E", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MATERIAL",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PRICE_PURCHASE = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    PHOTO_URL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MATERIAL__3214EC27EDDBBA52", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CATEGORY = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PRICE_BASE = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    IS_CUSTOM = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    Complexity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PHOTO_URL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IS_ACTIVE = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PRODUCTS__3214EC270CA74CC2", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ROL",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ROL__3214EC27B04AB8DD", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SUPPLIERS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ADDRESS = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PHONE = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DATE_LAST_PURCHASE = table.Column<DateTime>(type: "datetime", nullable: true),
                    NOTES = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SUPPLIER__3214EC27FB64233E", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "INVENTORY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MINIMUN_QUANTITY = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    STATE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MATERIAL_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__INVENTOR__3214EC2756497961", x => x.ID);
                    table.ForeignKey(
                        name: "FK__INVENTORY__MATER__68487DD7",
                        column: x => x.MATERIAL_ID,
                        principalTable: "MATERIAL",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT_MATERIAL",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT_MATERIAL", x => new { x.ProductId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK__PRODUCT_MATERIAL__MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "MATERIAL",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__PRODUCT_MATERIAL__ProductId",
                        column: x => x.ProductId,
                        principalTable: "PRODUCTS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ADMINISTRATOR",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDENTIFICATION = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    NAME = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PASSWORD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISACTIVE = table.Column<byte>(type: "tinyint", nullable: true, defaultValue: (byte)1),
                    ROL_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ADMINIST__3214EC27261094B9", x => x.ID);
                    table.ForeignKey(
                        name: "FK__ADMINISTR__ROL_I__412EB0B6",
                        column: x => x.ROL_ID,
                        principalTable: "ROL",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDENTIFICATION = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    NAME = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PASSWORD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SPECIALITY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IS_ACTIVE = table.Column<byte>(type: "tinyint", nullable: true, defaultValue: (byte)1),
                    ROL_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EMPLOYEE__3214EC2784DED161", x => x.ID);
                    table.ForeignKey(
                        name: "FK__EMPLOYEE__ROL_ID__44FF419A",
                        column: x => x.ROL_ID,
                        principalTable: "ROL",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ROLE_PERMISSIONS",
                columns: table => new
                {
                    ROLE_ID = table.Column<int>(type: "int", nullable: false),
                    MODULE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CANVIEW = table.Column<byte>(type: "tinyint", nullable: false),
                    CANCREATE = table.Column<byte>(type: "tinyint", nullable: false),
                    CANDELETE = table.Column<byte>(type: "tinyint", nullable: false),
                    CANEDIT = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ROLE_PER__93B89656E74098C7", x => new { x.ROLE_ID, x.MODULE });
                    table.ForeignKey(
                        name: "FK__ROLE_PERM__ROLE___3D5E1FD2",
                        column: x => x.ROLE_ID,
                        principalTable: "ROL",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MATERIAL_SUPPLIER",
                columns: table => new
                {
                    MATERIAL_ID = table.Column<int>(type: "int", nullable: false),
                    SUPPLIER_ID = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATERIAL_SUPPLIER", x => new { x.MATERIAL_ID, x.SUPPLIER_ID });
                    table.ForeignKey(
                        name: "FK_MATERIAL_SUPPLIER_MATERIAL_MATERIAL_ID",
                        column: x => x.MATERIAL_ID,
                        principalTable: "MATERIAL",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MATERIAL_SUPPLIER_SUPPLIERS_SUPPLIER_ID",
                        column: x => x.SUPPLIER_ID,
                        principalTable: "SUPPLIERS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ORDERS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CREATION_DATE = table.Column<DateOnly>(type: "date", nullable: false),
                    DALIVERY_DATE = table.Column<DateOnly>(type: "date", nullable: false),
                    STATE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PRIORITY = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TOTAL_AMOUNT = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    DISCOUNT = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    ADVANCEMENT = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    RESUME_PATH = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IS_ACTIVE = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CLIENT_ID = table.Column<int>(type: "int", nullable: true),
                    ADMIN_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ORDERS__3214EC270D9964BC", x => x.ID);
                    table.ForeignKey(
                        name: "FK__ORDERS__ADMIN_ID__4D94879B",
                        column: x => x.ADMIN_ID,
                        principalTable: "ADMINISTRATOR",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__ORDERS__CLIENT_I__4CA06362",
                        column: x => x.CLIENT_ID,
                        principalTable: "CLIENT",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CLIENT_PAYMENT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AMOUNT = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    PAYMENT_DATE = table.Column<DateOnly>(type: "date", nullable: false),
                    PAYMENT_METHOD = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PHOTO_VOUCHER_URL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ORDER_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CLIENT_P__3214EC2724D43DD9", x => x.ID);
                    table.ForeignKey(
                        name: "FK__CLIENT_PA__ORDER__6C190EBB",
                        column: x => x.ORDER_ID,
                        principalTable: "ORDERS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT_ORDER",
                columns: table => new
                {
                    ID_PRODUCT = table.Column<int>(type: "int", nullable: false),
                    ID_ORDER = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PRODUCT___2491A4768A57FE4F", x => new { x.ID_PRODUCT, x.ID_ORDER });
                    table.ForeignKey(
                        name: "FK__PRODUCT_O__ID_OR__5AEE82B9",
                        column: x => x.ID_ORDER,
                        principalTable: "ORDERS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__PRODUCT_O__ID_PR__59FA5E80",
                        column: x => x.ID_PRODUCT,
                        principalTable: "PRODUCTS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ADMINISTRATOR_ROL_ID",
                table: "ADMINISTRATOR",
                column: "ROL_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_ADMIN_EMAIL",
                table: "ADMINISTRATOR",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ADMIN_IDENTIFICATION",
                table: "ADMINISTRATOR",
                column: "IDENTIFICATION",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_CLIENT_EMAIL",
                table: "CLIENT",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_CLIENT_MANAGER_PHONE_NUMBER",
                table: "CLIENT",
                column: "MANAGER_PHONE_NUMBER",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_CLIENT_PHONE_NUMBER",
                table: "CLIENT",
                column: "PHONE_NUMBER",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLIENT_PAYMENT_ORDER_ID",
                table: "CLIENT_PAYMENT",
                column: "ORDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_ROL_ID",
                table: "EMPLOYEE",
                column: "ROL_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_EMPLOYEE_EMAIL",
                table: "EMPLOYEE",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_EMPLOYEE_IDENTIFICATION",
                table: "EMPLOYEE",
                column: "IDENTIFICATION",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_INVENTORY_MATERIAL_ID",
                table: "INVENTORY",
                column: "MATERIAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MATERIAL_SUPPLIER_SUPPLIER_ID",
                table: "MATERIAL_SUPPLIER",
                column: "SUPPLIER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_ADMIN_ID",
                table: "ORDERS",
                column: "ADMIN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_CLIENT_ID",
                table: "ORDERS",
                column: "CLIENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_MATERIAL_MaterialId",
                table: "PRODUCT_MATERIAL",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_ORDER_ID_ORDER",
                table: "PRODUCT_ORDER",
                column: "ID_ORDER");

            migrationBuilder.CreateIndex(
                name: "UQ_SUPPLIER_EMAIL",
                table: "SUPPLIERS",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_SUPPLIER_PHONE",
                table: "SUPPLIERS",
                column: "PHONE",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CLIENT_PAYMENT");

            migrationBuilder.DropTable(
                name: "EMPLOYEE");

            migrationBuilder.DropTable(
                name: "INVENTORY");

            migrationBuilder.DropTable(
                name: "MATERIAL_SUPPLIER");

            migrationBuilder.DropTable(
                name: "PRODUCT_MATERIAL");

            migrationBuilder.DropTable(
                name: "PRODUCT_ORDER");

            migrationBuilder.DropTable(
                name: "ROLE_PERMISSIONS");

            migrationBuilder.DropTable(
                name: "SUPPLIERS");

            migrationBuilder.DropTable(
                name: "MATERIAL");

            migrationBuilder.DropTable(
                name: "ORDERS");

            migrationBuilder.DropTable(
                name: "PRODUCTS");

            migrationBuilder.DropTable(
                name: "ADMINISTRATOR");

            migrationBuilder.DropTable(
                name: "CLIENT");

            migrationBuilder.DropTable(
                name: "ROL");
        }
    }
}
