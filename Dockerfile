FROM mcr.microsoft.com/dotnet/sdk:8.0.100
LABEL MAINTAINER "kcduboiswiz"
LABEL PROJECT "dv-dotnet-api"

ENV ASPNETCORE_URLS=http://0.0.0.0:5000

COPY . /app

WORKDIR /app

RUN dotnet restore 

EXPOSE 5000

CMD ["dotnet", "watch", "run"]
