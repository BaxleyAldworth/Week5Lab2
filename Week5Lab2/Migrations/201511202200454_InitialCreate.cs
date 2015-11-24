namespace Week5Lab2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        DOB = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CheckOuts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CheckOutDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        Book_Id = c.Int(),
                        Student_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.Book_Id)
                .ForeignKey("dbo.Students", t => t.Student_Id)
                .Index(t => t.Book_Id)
                .Index(t => t.Student_Id);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Author = c.String(),
                        DateOfPublication = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CheckOuts", "Student_Id", "dbo.Students");
            DropForeignKey("dbo.CheckOuts", "Book_Id", "dbo.Books");
            DropIndex("dbo.CheckOuts", new[] { "Student_Id" });
            DropIndex("dbo.CheckOuts", new[] { "Book_Id" });
            DropTable("dbo.Books");
            DropTable("dbo.CheckOuts");
            DropTable("dbo.Students");
        }
    }
}
