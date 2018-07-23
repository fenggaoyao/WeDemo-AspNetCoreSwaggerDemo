FROM microsoft/aspnetcore:2.0

WORKDIR /root/Swagger

# Copy the app file to root

COPY .  /root/Swagger/

# Configure the listening port to 8100
EXPOSE 9100/tcp

# Start the app
ENTRYPOINT dotnet AspNetCoreSwaggerDemo.dll