namespace Hospital_Management_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateArticlesTable : DbMigration
    {
        public override void Up()
        {
            Sql("IF OBJECT_ID('dbo.Posts', 'U') IS NOT NULL DROP TABLE dbo.Posts;");

            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        ArticleId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Slug = c.String(nullable: false, maxLength: 255),
                        Thumbnail = c.String(maxLength: 500),
                        Content = c.String(nullable: false),
                        Author = c.String(maxLength: 150),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ArticleId);

            CreateIndex("dbo.Articles", "Slug", unique: true, name: "IX_Article_Slug");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Articles", "IX_Article_Slug");
            DropTable("dbo.Articles");

            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        PostId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Content = c.String(),
                        Author = c.String(maxLength: 100),
                        PublishedDate = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        ImageUrl = c.String(maxLength: 500),
                        Category = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PostId);
        }
    }
}
