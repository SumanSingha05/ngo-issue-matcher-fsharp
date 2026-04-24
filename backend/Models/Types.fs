namespace backend.Models

type Issue = {
    Id: int
    Category: string
    Description: string
}

type NGO = {
    Name: string
    Specialization: string list
}