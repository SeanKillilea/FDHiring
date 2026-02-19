using FDHiring.Data;
using FDHiring.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// SERVICES
// ============================================================
builder.Services.AddControllersWithViews();

// WebOptimizer — bundle & minify CSS
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddCssBundle("/css/bundle.css",
        "css/_vars.css",
        "css/_reset.css",
        "css/_layout.css",
        "css/_candidate.css",
        "css/_utilities.css",
        "css/_buttons.css",
        "css/_toast.css",
        "css/_badges.css",
        "css/_form.css",
        "css/_modal.css");
});

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Database connection factory
builder.Services.AddSingleton<DbConnectionFactory>();

// Repositories
builder.Services.AddScoped<CandidateRepository>();
builder.Services.AddScoped<PositionRepository>();
builder.Services.AddScoped<AgencyRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<WorkflowRepository>();
builder.Services.AddScoped<WorkflowStepRepository>();
builder.Services.AddScoped<WorkflowHistoryRepository>();
builder.Services.AddScoped<InterviewRepository>();
builder.Services.AddScoped<InterviewQuestionRepository>();
builder.Services.AddScoped<InterviewAnswerRepository>();
builder.Services.AddScoped<CandidateFileRepository>();
builder.Services.AddScoped<EmailTemplateRepository>();

// ============================================================
// PIPELINE
// ============================================================
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Candidate/Error");
}

app.UseWebOptimizer();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Candidate}/{action=Search}/{id?}");

app.Run();