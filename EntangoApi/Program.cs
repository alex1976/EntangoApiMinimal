using Microsoft.EntityFrameworkCore;
using EntangoApi;
using EntangoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Register HttpClient for outbound HTTP calls (used by OpenAI controller)
builder.Services.AddHttpClient();

// Configure PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure SMTP Mail Service
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IMailService, SmtpMailService>();

// Configure SMS (Twilio) Service
builder.Services.Configure<EntangoApi.Models.SmsSettings>(builder.Configuration.GetSection("Sms"));
builder.Services.AddHttpClient("twilio");
builder.Services.AddTransient<EntangoApi.Services.ISmsService, EntangoApi.Services.TwilioSmsService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
