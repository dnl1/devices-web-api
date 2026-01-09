FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /src

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

EXPOSE 8080

CMD ["dotnet", "watch", "run"]