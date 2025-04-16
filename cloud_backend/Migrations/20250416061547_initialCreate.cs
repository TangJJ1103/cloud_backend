using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cloud_backend.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    productId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cost = table.Column<double>(type: "double", nullable: false),
                    price = table.Column<double>(type: "double", nullable: false),
                    stockQuantity = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    model = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    discountPercentage = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    soldQuantity = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.productId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_credentials",
                columns: table => new
                {
                    credentialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contactNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lastLogOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_credentials", x => x.credentialId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "manufacturing_request",
                columns: table => new
                {
                    requestId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    productId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    cost = table.Column<double>(type: "double", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_manufacturing_request", x => x.requestId);
                    table.ForeignKey(
                        name: "FK_manufacturing_request_products_productId",
                        column: x => x.productId,
                        principalTable: "products",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "customer_user",
                columns: table => new
                {
                    customerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    credentialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    verificationToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    isVerified = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_user", x => x.customerId);
                    table.ForeignKey(
                        name: "FK_customer_user_user_credentials_credentialId",
                        column: x => x.credentialId,
                        principalTable: "user_credentials",
                        principalColumn: "credentialId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    orderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    credentialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<double>(type: "double", nullable: false),
                    discountPercentage = table.Column<int>(type: "int", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fulfilledAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_orders_user_credentials_credentialId",
                        column: x => x.credentialId,
                        principalTable: "user_credentials",
                        principalColumn: "credentialId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "staff_user",
                columns: table => new
                {
                    staffId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    credentialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    isActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_user", x => x.staffId);
                    table.ForeignKey(
                        name: "FK_staff_user_user_credentials_credentialId",
                        column: x => x.credentialId,
                        principalTable: "user_credentials",
                        principalColumn: "credentialId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "store_user",
                columns: table => new
                {
                    storeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    credentialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    isActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_user", x => x.storeId);
                    table.ForeignKey(
                        name: "FK_store_user_user_credentials_credentialId",
                        column: x => x.credentialId,
                        principalTable: "user_credentials",
                        principalColumn: "credentialId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    orderItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    orderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    productId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<double>(type: "double", nullable: false),
                    discountPercentage = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.orderItemId);
                    table.ForeignKey(
                        name: "FK_order_items_orders_orderId",
                        column: x => x.orderId,
                        principalTable: "orders",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_items_products_productId",
                        column: x => x.productId,
                        principalTable: "products",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "receipts",
                columns: table => new
                {
                    receiptId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    credentialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    orderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    amount = table.Column<double>(type: "double", nullable: false),
                    paymentMethod = table.Column<int>(type: "int", nullable: false),
                    paymentType = table.Column<int>(type: "int", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipts", x => x.receiptId);
                    table.ForeignKey(
                        name: "FK_receipts_orders_orderId",
                        column: x => x.orderId,
                        principalTable: "orders",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_receipts_user_credentials_credentialId",
                        column: x => x.credentialId,
                        principalTable: "user_credentials",
                        principalColumn: "credentialId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quotation_request",
                columns: table => new
                {
                    quotationRequestId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    storeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    status = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotation_request", x => x.quotationRequestId);
                    table.ForeignKey(
                        name: "FK_quotation_request_store_user_storeId",
                        column: x => x.storeId,
                        principalTable: "store_user",
                        principalColumn: "storeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quotations",
                columns: table => new
                {
                    quotationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    storeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    orderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    status = table.Column<int>(type: "int", nullable: false),
                    discountPercentage = table.Column<int>(type: "int", nullable: false),
                    totalAmount = table.Column<double>(type: "double", nullable: false),
                    totalQuantity = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotations", x => x.quotationId);
                    table.ForeignKey(
                        name: "FK_quotations_orders_orderId",
                        column: x => x.orderId,
                        principalTable: "orders",
                        principalColumn: "orderId");
                    table.ForeignKey(
                        name: "FK_quotations_store_user_storeId",
                        column: x => x.storeId,
                        principalTable: "store_user",
                        principalColumn: "storeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quotation_request_items",
                columns: table => new
                {
                    quotationRequestItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    quotationRequestId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    productId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    unitPrice = table.Column<double>(type: "double", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    discountPercentage = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotation_request_items", x => x.quotationRequestItemId);
                    table.ForeignKey(
                        name: "FK_quotation_request_items_products_productId",
                        column: x => x.productId,
                        principalTable: "products",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quotation_request_items_quotation_request_quotationRequestId",
                        column: x => x.quotationRequestId,
                        principalTable: "quotation_request",
                        principalColumn: "quotationRequestId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quotation_items",
                columns: table => new
                {
                    quotationItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    quotationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    productId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<double>(type: "double", nullable: false),
                    discountPercentage = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotation_items", x => x.quotationItemId);
                    table.ForeignKey(
                        name: "FK_quotation_items_products_productId",
                        column: x => x.productId,
                        principalTable: "products",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quotation_items_quotations_quotationId",
                        column: x => x.quotationId,
                        principalTable: "quotations",
                        principalColumn: "quotationId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_customer_user_credentialId",
                table: "customer_user",
                column: "credentialId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_manufacturing_request_productId",
                table: "manufacturing_request",
                column: "productId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_items_orderId",
                table: "order_items",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_productId",
                table: "order_items",
                column: "productId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_credentialId",
                table: "orders",
                column: "credentialId");

            migrationBuilder.CreateIndex(
                name: "IX_quotation_items_productId",
                table: "quotation_items",
                column: "productId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quotation_items_quotationId",
                table: "quotation_items",
                column: "quotationId");

            migrationBuilder.CreateIndex(
                name: "IX_quotation_request_storeId",
                table: "quotation_request",
                column: "storeId");

            migrationBuilder.CreateIndex(
                name: "IX_quotation_request_items_productId",
                table: "quotation_request_items",
                column: "productId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quotation_request_items_quotationRequestId",
                table: "quotation_request_items",
                column: "quotationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_orderId",
                table: "quotations",
                column: "orderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quotations_storeId",
                table: "quotations",
                column: "storeId");

            migrationBuilder.CreateIndex(
                name: "IX_receipts_credentialId",
                table: "receipts",
                column: "credentialId");

            migrationBuilder.CreateIndex(
                name: "IX_receipts_orderId",
                table: "receipts",
                column: "orderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_staff_user_credentialId",
                table: "staff_user",
                column: "credentialId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_user_credentialId",
                table: "store_user",
                column: "credentialId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_user");

            migrationBuilder.DropTable(
                name: "manufacturing_request");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "quotation_items");

            migrationBuilder.DropTable(
                name: "quotation_request_items");

            migrationBuilder.DropTable(
                name: "receipts");

            migrationBuilder.DropTable(
                name: "staff_user");

            migrationBuilder.DropTable(
                name: "quotations");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "quotation_request");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "store_user");

            migrationBuilder.DropTable(
                name: "user_credentials");
        }
    }
}
