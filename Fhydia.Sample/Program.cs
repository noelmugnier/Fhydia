using Fhydia.Sample;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFhydia();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseFhydia();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
