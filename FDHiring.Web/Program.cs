using FDHiring.Data;
using FDHiring.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Data
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<CandidateRepository>();
builder.Services.AddScoped<CandidateFileRepository>();  
builder.Services.AddScoped<PositionRepository>();
builder.Services.AddScoped<AgencyRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<WorkflowRepository>();
builder.Services.AddScoped<WorkflowStepRepository>();
builder.Services.AddScoped<InterviewRepository>();
builder.Services.AddScoped<InterviewTypeRepository>();
builder.Services.AddScoped<InterviewQuestionRepository>();
builder.Services.AddScoped<InterviewAnswerRepository>();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(300);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Candidate}/{action=Search}/{id?}");

app.Run();