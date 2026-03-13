# hm-appointment-service

Independent microservice repository for Hospital Management.

## Local run

`ash
dotnet restore
dotnet build
dotnet run --project src/AppointmentService.Api/AppointmentService.Api.csproj
`

## Docker

`ash
docker build -t hm-appointment-service:local .
docker run -p 8084:8080 hm-appointment-service:local
`

## GitHub setup later

`ash
git branch -M main
git remote add origin <your-github-repo-url>
git add .
git commit -m "Initial scaffold"
git push -u origin main
`
