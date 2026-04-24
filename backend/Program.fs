module Program

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection

let builder = WebApplication.CreateBuilder()

// Adding the CORS
builder.Services.AddCors(fun options -> 
    options.AddPolicy("allowAll", fun policy -> 
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
        |> ignore )
) |> ignore

builder.Services.AddControllers() |> ignore

let app = builder.Build()

//Using the CORS
app.UseCors("allowAll")

app.MapControllers()

app.Run()