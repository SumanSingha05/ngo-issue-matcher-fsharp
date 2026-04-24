namespace backend.Services

open backend.Models

module Matcher =

    let scoreMatch (issue: Issue) (ngo: NGO) =
        if ngo.Specialization |> List.contains issue.Category then 10
        else 1

    let matchNGOs (issue: Issue) (ngos: NGO list) = 
        ngos
        |> List.map(fun ngo -> 
            {|
                name = ngo.Name
                score = scoreMatch issue ngo
            |})
        |> List.sortByDescending ( fun x -> x.score)


