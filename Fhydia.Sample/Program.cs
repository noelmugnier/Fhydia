using Fhydia.Sample;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFhydia();

var app = builder.Build();

app.UseFhydia();
app.Run();
				