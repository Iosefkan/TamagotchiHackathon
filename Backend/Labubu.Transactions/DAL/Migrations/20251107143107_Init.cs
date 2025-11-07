using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:currency_code", "aed,afn,all,amd,ang,aoa,ars,aud,awg,azn,bam,bbd,bdt,bgn,bhd,bif,bmd,bnd,bob,bov,brl,bsd,btn,bwp,byn,bzd,cad,cdf,che,chf,chw,clf,clp,cop,cou,crc,cuc,cup,cve,czk,djf,dkk,dop,dzd,egp,ern,etb,eur,fjd,fkp,gbp,gel,ghs,gip,gmd,gnf,gtq,gyd,hkd,hnl,hrk,htg,huf,idr,ils,inr,iqd,irr,isk,jmd,jod,jpy,kes,kgs,khr,kmf,kpw,krw,kwd,kyd,kzt,lak,lbp,lkr,lrd,lsl,lyd,mad,mdl,mga,mkd,mmk,mnt,mop,mru,mur,mvr,mwk,mxn,mxv,myr,mzn,nad,ngn,nio,nok,npr,nzd,omr,pab,pen,pgk,php,pkr,pln,pyg,qar,ron,rsd,cny,rub,rwf,sar,sbd,scr,sdg,sek,sgd,shp,sll,sle,sos,srd,ssp,stn,svc,syp,szl,thb,tjs,tmt,tnd,top,try,ttd,twd,tzs,uah,ugx,usd,usn,uyi,uyu,uyw,uzs,ved,ves,vnd,vuv,wst,xaf,xag,xau,xba,xbb,xbc,xbd,xcd,xdr,xof,xpd,xpf,xpt,xsu,xts,xua,xxx,yer,zar,zmw,zwl");

            migrationBuilder.CreateTable(
                name: "Consents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankConsentId = table.Column<string>(type: "text", nullable: false),
                    RequestId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankAccountId = table.Column<string>(type: "text", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    InterimAvailable = table.Column<decimal>(type: "numeric", nullable: false),
                    InterimAvailableAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Consents_ConsentId",
                        column: x => x.ConsentId,
                        principalTable: "Consents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ConsentId",
                table: "Accounts",
                column: "ConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Consents");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
