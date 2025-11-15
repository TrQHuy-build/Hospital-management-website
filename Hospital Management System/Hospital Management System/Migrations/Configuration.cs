namespace Hospital_Management_System.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Hospital_Management_System.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(Hospital_Management_System.Models.ApplicationDbContext context)
        {
            // Tạo UserManager để quản lý users
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Tạo các roles nếu chưa tồn tại
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }
            if (!roleManager.RoleExists("Doctor"))
            {
                roleManager.Create(new IdentityRole("Doctor"));
            }

            // Tạo tài khoản Admin
            var adminUser = new ApplicationUser
            {
                UserName = "admin@hospital.com",
                Email = "admin@hospital.com",
                EmailConfirmed = true,
                UserRole = "Admin",
                RegisteredDate = DateTime.Now
            };

            // Thêm admin nếu chưa tồn tại
            if (userManager.FindByEmail("admin@hospital.com") == null)
            {
                var result = userManager.Create(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
            }

            // Tạo tài khoản Doctor
            var doctorUser = new ApplicationUser
            {
                UserName = "doctor@hospital.com",
                Email = "doctor@hospital.com",
                EmailConfirmed = true,
                UserRole = "Doctor",
                RegisteredDate = DateTime.Now
            };

            // Thêm doctor nếu chưa tồn tại
            if (userManager.FindByEmail("doctor@hospital.com") == null)
            {
                var result = userManager.Create(doctorUser, "Doctor@123");
                if (result.Succeeded)
                {
                    userManager.AddToRole(doctorUser.Id, "Doctor");
                }

                // Tạo Department mặc định nếu chưa có
                var department = context.Department.FirstOrDefault();
                if (department == null)
                {
                    department = new Department { Name = "General Medicine" };
                    context.Department.Add(department);
                    context.SaveChanges();
                }

                // Tạo thông tin bác sĩ
                var doctor = new Doctor
                {
                    FirstName = "John",
                    LastName = "Doe",
                    FullName = "John Doe",
                    ApplicationUserId = doctorUser.Id,
                    DepartmentId = department.Id,
                    ContactNo = "0123456789",
                    BloodGroup = "O+",
                    Education = "MBBS"
                };
                context.Doctors.Add(doctor);
            }

            if (!context.Articles.Any())
            {
                context.Articles.AddOrUpdate(a => a.Slug,
                    new Article
                    {
                        Title = "Chào mừng đến với Bệnh viện UTC2",
                        Slug = "chao-mung-benh-vien-utc2",
                        Thumbnail = "~/Content/Images/article1.jpg",
                        Content = "Bệnh viện UTC2 cam kết mang lại dịch vụ chăm sóc sức khỏe toàn diện với đội ngũ bác sĩ hàng đầu và trang thiết bị hiện đại.",
                        Author = "Admin",
                        CreatedAt = DateTime.UtcNow.AddDays(-30),
                        UpdatedAt = DateTime.UtcNow.AddDays(-30),
                        Status = true
                    },
                    new Article
                    {
                        Title = "Lịch khám bệnh tháng 11",
                        Slug = "lich-kham-benh-thang-11",
                        Thumbnail = "~/Content/Images/article2.jpg",
                        Content = "Theo dõi lịch khám tháng 11 để đặt lịch hẹn trực tuyến, giảm thời gian chờ đợi và tối ưu trải nghiệm khám chữa bệnh.",
                        Author = "Bác sĩ Nguyễn Văn A",
                        CreatedAt = DateTime.UtcNow.AddDays(-25),
                        UpdatedAt = DateTime.UtcNow.AddDays(-25),
                        Status = true
                    },
                    new Article
                    {
                        Title = "Cập nhật công nghệ chẩn đoán hình ảnh",
                        Slug = "cap-nhat-cong-nghe-chan-doan",
                        Thumbnail = "~/Content/Images/article3.jpg",
                        Content = "Hệ thống MRI và CT Scan thế hệ mới giúp phát hiện sớm các bệnh lý phức tạp, hỗ trợ bác sĩ chẩn đoán chính xác hơn.",
                        Author = "Khoa Chẩn đoán",
                        CreatedAt = DateTime.UtcNow.AddDays(-20),
                        UpdatedAt = DateTime.UtcNow.AddDays(-18),
                        Status = true
                    },
                    new Article
                    {
                        Title = "Hướng dẫn dinh dưỡng cho bệnh nhân tiểu đường",
                        Slug = "huong-dan-dinh-duong-tieu-duong",
                        Thumbnail = "~/Content/Images/article4.jpg",
                        Content = "Chế độ ăn cân bằng, hạn chế đường và tinh bột sẽ giúp bệnh nhân tiểu đường kiểm soát đường huyết hiệu quả.",
                        Author = "Chuyên gia dinh dưỡng",
                        CreatedAt = DateTime.UtcNow.AddDays(-18),
                        UpdatedAt = DateTime.UtcNow.AddDays(-17),
                        Status = true
                    },
                    new Article
                    {
                        Title = "Dịch vụ cấp cứu 24/7",
                        Slug = "dich-vu-cap-cuu-247",
                        Thumbnail = "~/Content/Images/article5.jpg",
                        Content = "Đội ngũ cấp cứu luôn túc trực để tiếp nhận và xử lý mọi tình huống khẩn cấp nhanh chóng, chính xác.",
                        Author = "Phòng Cấp cứu",
                        CreatedAt = DateTime.UtcNow.AddDays(-15),
                        UpdatedAt = DateTime.UtcNow.AddDays(-15),
                        Status = true
                    },
                    new Article
                    {
                        Title = "Chương trình khám sức khỏe định kỳ",
                        Slug = "chuong-trinh-kham-suc-khoe-dinh-ky",
                        Thumbnail = "~/Content/Images/article6.jpg",
                        Content = "Gói khám định kỳ toàn diện giúp phát hiện sớm bệnh lý và xây dựng lộ trình điều trị phù hợp cho từng bệnh nhân.",
                        Author = "Admin",
                        CreatedAt = DateTime.UtcNow.AddDays(-12),
                        UpdatedAt = DateTime.UtcNow.AddDays(-12),
                        Status = true
                    },
                    new Article
                    {
                        Title = "Lời khuyên chăm sóc sức khỏe mùa mưa",
                        Slug = "loi-khuyen-suc-khoe-mua-mua",
                        Thumbnail = "~/Content/Images/article7.jpg",
                        Content = "Giữ ấm cơ thể, bổ sung vitamin C và luyện tập thể dục nhẹ nhàng giúp tăng sức đề kháng mùa mưa.",
                        Author = "Bác sĩ Trần Thị B",
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UpdatedAt = DateTime.UtcNow.AddDays(-9),
                        Status = true
                    },
                    new Article
                    {
                        Title = "Tập huấn sơ cứu cho nhân viên",
                        Slug = "tap-huan-so-cuu-nhan-vien",
                        Thumbnail = "~/Content/Images/article8.jpg",
                        Content = "Chương trình tập huấn giúp nhân viên nâng cao kỹ năng sơ cứu, sẵn sàng hỗ trợ bệnh nhân trong mọi tình huống.",
                        Author = "Phòng Nhân sự",
                        CreatedAt = DateTime.UtcNow.AddDays(-7),
                        UpdatedAt = DateTime.UtcNow.AddDays(-6),
                        Status = true
                    },
                    new Article
                    {
                        Title = "Chia sẻ từ bệnh nhân hồi phục sau phẫu thuật",
                        Slug = "chia-se-benh-nhan-phau-thuat",
                        Thumbnail = "~/Content/Images/article9.jpg",
                        Content = "Bệnh nhân N.T.A đã hồi phục nhanh chóng nhờ tuân thủ hướng dẫn chăm sóc hậu phẫu và chế độ dinh dưỡng hợp lý.",
                        Author = "Truyền thông bệnh viện",
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-4),
                        Status = true
                    },
                    new Article
                    {
                        Title = "Thông báo bảo trì hệ thống",
                        Slug = "thong-bao-bao-tri-he-thong",
                        Thumbnail = "~/Content/Images/article10.jpg",
                        Content = "Hệ thống đặt lịch online sẽ bảo trì vào cuối tuần. Quý khách vui lòng đặt lịch qua hotline trong thời gian này.",
                        Author = "IT Support",
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1),
                        Status = true
                    }
                );
            }

            context.SaveChanges();
        }
    }
}
