namespace backend.Controllers

open Microsoft.AspNetCore.Mvc 
open backend.Models
open backend.Services.Matcher

[<ApiController>]
[<Route("/")>]
type MyController() =
    inherit ControllerBase()

    [<HttpGet("hello")>]
    member _.Hello() =
        "Hello from F# backend"

    [<HttpPost("match")>]
    member _.Match(issue: Issue) =

        let ngos = [
            { Name = "WaterHelp"; Specialization = ["water"] }
            { Name = "EduCare"; Specialization = ["education"] }
            { Name = "SafeHome"; Specialization = ["security"] }
        ]

        let result = matchNGOs issue ngos

        base.Ok(result)